using Nez.AI.FSM;
using Nez.AI.GOAP;
using System.Collections.Generic;


namespace Nez.Samples
{
	public enum MinerState
	{
		Idle,
		GoTo,
		PerformAction
	}

	public class GOAPMiner : SimpleStateMachine<MinerState>
	{
		public enum Location
		{
			InTransit,
			Bank,
			Mine,
			Home,
			Saloon
		}

		const string IS_FATIGUED = "fatigued";
		const string IS_THIRSTY = "thirsty";
		const string HAS_ENOUGH_GOLD = "hasenoughgold";

		const int MAX_FATIGUE = 10;
		const int MAX_GOLD = 8;
		const int MAX_THIRST = 5;

		int _fatigue;
		int _thirst;
		int _gold;
		int _goldInBank;

		Location _currentLocation = Location.Home;
		Location _destinationLocation;
		int _distanceToNextLocation = 10;

		ActionPlanner _planner;
		Stack<Action> _actionPlan;


		public GOAPMiner()
		{
			_planner = new ActionPlanner();

			var sleep = new Action( "sleep" );
			sleep.setPrecondition( IS_FATIGUED, true );
			sleep.setPostcondition( IS_FATIGUED, false );
			_planner.addAction( sleep );

			var drink = new Action( "drink" );
			drink.setPrecondition( IS_THIRSTY, true );
			drink.setPostcondition( IS_THIRSTY, false );
			_planner.addAction( drink );

			var mine = new Action( "mine" );
			mine.setPrecondition( HAS_ENOUGH_GOLD, false );
			mine.setPostcondition( HAS_ENOUGH_GOLD, true );
			_planner.addAction( mine );

			var depositGold = new Action( "depositGold" );
			depositGold.setPrecondition( HAS_ENOUGH_GOLD, true );
			depositGold.setPostcondition( HAS_ENOUGH_GOLD, false );
			_planner.addAction( depositGold );

			initialState = MinerState.Idle;
		}


		public override void onAddedToEntity()
		{
			// slow things down a bit. we dont need to tick every frame
			entity.updateInterval = 10;
			enabled = false;
		}


		WorldState getWorldState()
		{
			var worldState = _planner.createWorldState();
			worldState.set( IS_FATIGUED, _fatigue >= MAX_FATIGUE );
			worldState.set( IS_THIRSTY, _thirst >= MAX_THIRST );
			worldState.set( HAS_ENOUGH_GOLD, _gold >= MAX_GOLD );

			return worldState;
		}


		WorldState getGoalState()
		{
			var goalState = _planner.createWorldState();

			if( _fatigue >= MAX_FATIGUE )
			{
				goalState.set( IS_FATIGUED, false );
			}
			else if( _thirst >= MAX_THIRST )
			{
				goalState.set( IS_THIRSTY, false );
			}
			else if( _gold >= MAX_GOLD )
			{
				goalState.set( HAS_ENOUGH_GOLD, false );
			}
			else
			{
				goalState.set( HAS_ENOUGH_GOLD, true );
			}

			return goalState;
		}


		#region states

		void Idle_Enter()
		{
			_actionPlan = _planner.plan( getWorldState(), getGoalState() );

			if( _actionPlan != null && _actionPlan.Count > 0 )
			{
				currentState = MinerState.GoTo;
				Debug.log( "got an action plan with {0} actions", _actionPlan.Count );
			}
			else
			{
				Debug.log( "no action plan satisfied our goals" );
			}
		}


		void GoTo_Enter()
		{
			// figure out where we are going
			var action = _actionPlan.Peek().name;
			switch( action )
			{
				case "sleep":
					_destinationLocation = Location.Home;
				break;
				case "drink":
					_destinationLocation = Location.Saloon;
				break;
				case "mine":
					_destinationLocation = Location.Mine;
				break;
				case "depositGold":
					_destinationLocation = Location.Bank;
				break;
			}

			if( _currentLocation == _destinationLocation )
			{
				currentState = MinerState.PerformAction;
			}
			else
			{
				_distanceToNextLocation = Nez.Random.range( 2, 8 );
				_currentLocation = Location.InTransit;
			}
		}


		void GoTo_Tick()
		{
			Debug.log( "heading to {0}. its {1} miles away", _destinationLocation, _distanceToNextLocation );
			_distanceToNextLocation--;

			if( _distanceToNextLocation == 0 )
			{
				_fatigue++;
				_currentLocation = _destinationLocation;
				currentState = MinerState.PerformAction;
			}
		}


		void GoTo_Exit()
		{
			Debug.log( "made it to the " + _currentLocation );
		}


		void PerformAction_Tick()
		{
			var action = _actionPlan.Peek().name;

			switch( action )
			{
				case "sleep":
					Debug.log( "getting some sleep. current fatigue {0}", _fatigue );
					_fatigue--;

					if( _fatigue == 0 )
						currentState = MinerState.Idle;
				break;
				case "drink":
					Debug.log( "getting my drink on. Thirst level {0}", _thirst );
					_thirst--;

					if( _thirst == 0 )
						currentState = MinerState.Idle;
				break;
				case "mine":
					Debug.log( "digging for gold. nuggets found {0}", _gold );
					_gold++;
					_fatigue++;
					_thirst++;

					if( _gold >= MAX_GOLD )
						currentState = MinerState.Idle;
				break;
				case "depositGold":
					_goldInBank += _gold;
					_gold = 0;

					Debug.log( "depositing gold at the bank. current wealth {0}", _goldInBank );
					currentState = MinerState.Idle;
				break;
			}
		}

		#endregion

	}
}

