using Nez.AI.FSM;
using Nez.AI.GOAP;
using System.Collections.Generic;


namespace Nez.Samples
{
	/// <summary>
	/// Goal Oriented Action Planning miner bob. Sets up Actions and uses the ActionPlanner to pick the appropriate action based on the world
	/// and goal states. This example also uses a SimpleStateMachine to deal with exeucuting the plan that the ActionPlanner comes up with.
	/// </summary>
	public class GOAPMiner : SimpleStateMachine<GOAPMiner.MinerBobState>
	{
		public enum MinerBobState
		{
			Idle,
			GoTo,
			PerformAction
		}

		public MinerState minerState = new MinerState();

		const string IS_FATIGUED = "fatigued";
		const string IS_THIRSTY = "thirsty";
		const string HAS_ENOUGH_GOLD = "hasenoughgold";

		MinerState.Location _destinationLocation;
		int _distanceToNextLocation = 10;

		ActionPlanner _planner;
		Stack<Action> _actionPlan;


		public GOAPMiner()
		{
			// we need an ActionPlanner before we can do anything
			_planner = new ActionPlanner();

			// setup our Actions and add them to the planner
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

			// set our state machine to idle. When it is in idle it will ask the ActionPlanner for a new plan
			initialState = MinerBobState.Idle;
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
			worldState.set( IS_FATIGUED, minerState.fatigue >= MinerState.MAX_FATIGUE );
			worldState.set( IS_THIRSTY, minerState.thirst >= MinerState.MAX_THIRST );
			worldState.set( HAS_ENOUGH_GOLD, minerState.gold >= MinerState.MAX_GOLD );

			return worldState;
		}


		WorldState getGoalState()
		{
			var goalState = _planner.createWorldState();

			if( minerState.fatigue >= MinerState.MAX_FATIGUE )
				goalState.set( IS_FATIGUED, false );
			else if( minerState.thirst >= MinerState.MAX_THIRST )
				goalState.set( IS_THIRSTY, false );
			else if( minerState.gold >= MinerState.MAX_GOLD )
				goalState.set( HAS_ENOUGH_GOLD, false );
			else
				goalState.set( HAS_ENOUGH_GOLD, true );

			return goalState;
		}


		#region states

		void Idle_Enter()
		{
			// get a plan to run that will get us from our current state to our goal state
			_actionPlan = _planner.plan( getWorldState(), getGoalState() );

			if( _actionPlan != null && _actionPlan.Count > 0 )
			{
				currentState = MinerBobState.GoTo;
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
					_destinationLocation = MinerState.Location.Home;
				break;
				case "drink":
					_destinationLocation = MinerState.Location.Saloon;
				break;
				case "mine":
					_destinationLocation = MinerState.Location.Mine;
				break;
				case "depositGold":
					_destinationLocation = MinerState.Location.Bank;
				break;
			}

			if( minerState.currentLocation == _destinationLocation )
			{
				currentState = MinerBobState.PerformAction;
			}
			else
			{
				_distanceToNextLocation = Nez.Random.range( 2, 8 );
				minerState.currentLocation = MinerState.Location.InTransit;
			}
		}


		void GoTo_Tick()
		{
			Debug.log( "heading to {0}. its {1} miles away", _destinationLocation, _distanceToNextLocation );
			_distanceToNextLocation--;

			if( _distanceToNextLocation == 0 )
			{
				minerState.fatigue++;
				minerState.currentLocation = _destinationLocation;
				currentState = MinerBobState.PerformAction;
			}
		}


		void GoTo_Exit()
		{
			Debug.log( "made it to the " + minerState.currentLocation );
		}


		void PerformAction_Tick()
		{
			var action = _actionPlan.Peek().name;

			switch( action )
			{
				case "sleep":
					Debug.log( "getting some sleep. current fatigue {0}", minerState.fatigue );
					minerState.fatigue--;

					if( minerState.fatigue == 0 )
						currentState = MinerBobState.Idle;
				break;
				case "drink":
					Debug.log( "getting my drink on. Thirst level {0}", minerState.thirst );
					minerState.thirst--;

					if( minerState.thirst == 0 )
						currentState = MinerBobState.Idle;
				break;
				case "mine":
					Debug.log( "digging for gold. nuggets found {0}", minerState.gold );
					minerState.gold++;
					minerState.fatigue++;
					minerState.thirst++;

					if( minerState.gold >= MinerState.MAX_GOLD )
						currentState = MinerBobState.Idle;
				break;
				case "depositGold":
					minerState.goldInBank += minerState.gold;
					minerState.gold = 0;

					Debug.log( "depositing gold at the bank. current wealth {0}", minerState.goldInBank );
					currentState = MinerBobState.Idle;
				break;
			}
		}

		#endregion

	}
}

