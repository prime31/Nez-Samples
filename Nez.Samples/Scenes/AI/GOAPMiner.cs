using Nez.AI.FSM;
using Nez.AI.GOAP;
using System.Collections.Generic;


namespace Nez.Samples
{
	/// <summary>
	/// Goal Oriented Action Planning miner bob. Sets up Actions and uses the ActionPlanner to pick the appropriate action based on the world
	/// and goal states. This example also uses a SimpleStateMachine to deal with exeucuting the plan that the ActionPlanner comes up with.
	/// </summary>
	public class GoapMiner : SimpleStateMachine<GoapMiner.MinerBobState>
	{
		public enum MinerBobState
		{
			Idle,
			GoTo,
			PerformAction
		}

		public MinerState MinerState = new MinerState();

		const string IsFatigued = "fatigued";
		const string IsThirsty = "thirsty";
		const string HasEnoughGold = "hasenoughgold";

		MinerState.Location _destinationLocation;
		int _distanceToNextLocation = 10;

		ActionPlanner _planner;
		Stack<Action> _actionPlan;


		public GoapMiner()
		{
			// we need an ActionPlanner before we can do anything
			_planner = new ActionPlanner();

			// setup our Actions and add them to the planner
			var sleep = new Action("sleep");
			sleep.SetPrecondition(IsFatigued, true);
			sleep.SetPostcondition(IsFatigued, false);
			_planner.AddAction(sleep);

			var drink = new Action("drink");
			drink.SetPrecondition(IsThirsty, true);
			drink.SetPostcondition(IsThirsty, false);
			_planner.AddAction(drink);

			var mine = new Action("mine");
			mine.SetPrecondition(HasEnoughGold, false);
			mine.SetPostcondition(HasEnoughGold, true);
			_planner.AddAction(mine);

			var depositGold = new Action("depositGold");
			depositGold.SetPrecondition(HasEnoughGold, true);
			depositGold.SetPostcondition(HasEnoughGold, false);
			_planner.AddAction(depositGold);

			// set our state machine to idle. When it is in idle it will ask the ActionPlanner for a new plan
			InitialState = MinerBobState.Idle;
		}


		public override void OnAddedToEntity()
		{
			// slow things down a bit. we dont need to tick every frame
			Entity.UpdateInterval = 10;
			Enabled = false;
		}


		WorldState GetWorldState()
		{
			var worldState = _planner.CreateWorldState();
			worldState.Set(IsFatigued, MinerState.Fatigue >= MinerState.MaxFatigue);
			worldState.Set(IsThirsty, MinerState.Thirst >= MinerState.MaxThirst);
			worldState.Set(HasEnoughGold, MinerState.Gold >= MinerState.MaxGold);

			return worldState;
		}


		WorldState GetGoalState()
		{
			var goalState = _planner.CreateWorldState();

			if (MinerState.Fatigue >= MinerState.MaxFatigue)
				goalState.Set(IsFatigued, false);
			else if (MinerState.Thirst >= MinerState.MaxThirst)
				goalState.Set(IsThirsty, false);
			else if (MinerState.Gold >= MinerState.MaxGold)
				goalState.Set(HasEnoughGold, false);
			else
				goalState.Set(HasEnoughGold, true);

			return goalState;
		}


		#region states

		void Idle_Enter()
		{
			// get a plan to run that will get us from our current state to our goal state
			_actionPlan = _planner.Plan(GetWorldState(), GetGoalState());

			if (_actionPlan != null && _actionPlan.Count > 0)
			{
				CurrentState = MinerBobState.GoTo;
				Debug.Log("got an action plan with {0} actions", _actionPlan.Count);
			}
			else
			{
				Debug.Log("no action plan satisfied our goals");
			}
		}


		void GoTo_Enter()
		{
			// figure out where we are going
			var action = _actionPlan.Peek().Name;
			switch (action)
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

			if (MinerState.CurrentLocation == _destinationLocation)
			{
				CurrentState = MinerBobState.PerformAction;
			}
			else
			{
				_distanceToNextLocation = Random.Range(2, 8);
				MinerState.CurrentLocation = MinerState.Location.InTransit;
			}
		}


		void GoTo_Tick()
		{
			Debug.Log("heading to {0}. its {1} miles away", _destinationLocation, _distanceToNextLocation);
			_distanceToNextLocation--;

			if (_distanceToNextLocation == 0)
			{
				MinerState.Fatigue++;
				MinerState.CurrentLocation = _destinationLocation;
				CurrentState = MinerBobState.PerformAction;
			}
		}


		void GoTo_Exit()
		{
			Debug.Log("made it to the " + MinerState.CurrentLocation);
		}


		void PerformAction_Tick()
		{
			var action = _actionPlan.Peek().Name;

			switch (action)
			{
				case "sleep":
					Debug.Log("getting some sleep. current fatigue {0}", MinerState.Fatigue);
					MinerState.Fatigue--;

					if (MinerState.Fatigue == 0)
						CurrentState = MinerBobState.Idle;
					break;
				case "drink":
					Debug.Log("getting my drink on. Thirst level {0}", MinerState.Thirst);
					MinerState.Thirst--;

					if (MinerState.Thirst == 0)
						CurrentState = MinerBobState.Idle;
					break;
				case "mine":
					Debug.Log("digging for gold. nuggets found {0}", MinerState.Gold);
					MinerState.Gold++;
					MinerState.Fatigue++;
					MinerState.Thirst++;

					if (MinerState.Gold >= MinerState.MaxGold)
						CurrentState = MinerBobState.Idle;
					break;
				case "depositGold":
					MinerState.GoldInBank += MinerState.Gold;
					MinerState.Gold = 0;

					Debug.Log("depositing gold at the bank. current wealth {0}", MinerState.GoldInBank);
					CurrentState = MinerBobState.Idle;
					break;
			}
		}

		#endregion
	}
}