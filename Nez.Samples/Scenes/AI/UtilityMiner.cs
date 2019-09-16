using Nez.AI.UtilityAI;


namespace Nez.Samples
{
	/// <summary>
	/// Utility AI example of miner bob. Utility AI is the most complex of all the AI types to setup. The complexity comes with a lot of power
	/// though.
	/// </summary>
	public class UtilityMiner : Component, IUpdatable
	{
		public MinerState MinerState = new MinerState();

		UtilityAI<UtilityMiner> _ai;
		MinerState.Location _destinationLocation;
		int _distanceToNextLocation = 10;


		public override void OnAddedToEntity()
		{
			Enabled = false;
			var reasoner = new FirstScoreReasoner<UtilityMiner>();

			// sleep is most important
			// AllOrNothingQualifier with required threshold of 1 so all scorers must score
			//  - we have to be home to sleep
			//  - we have to have some fatigue
			var fatigueConsideration = new AllOrNothingConsideration<UtilityMiner>(1)
				.AddAppraisal(new ActionAppraisal<UtilityMiner>(c =>
					c.MinerState.CurrentLocation == MinerState.Location.Home ? 1 : 0))
				.AddAppraisal(new ActionAppraisal<UtilityMiner>(c => c.MinerState.Fatigue > 0 ? 1 : 0));
			fatigueConsideration.Action = new ActionExecutor<UtilityMiner>(c => c.Sleep());
			reasoner.AddConsideration(fatigueConsideration);

			// thirst is next
			// AllOrNothingQualifier with required threshold of 1 so all scorers must score
			//  - we have to be at the saloon to drink
			//  - we have to be thirsty
			var thirstConsideration = new AllOrNothingConsideration<UtilityMiner>(1)
				.AddAppraisal(new ActionAppraisal<UtilityMiner>(c =>
					c.MinerState.CurrentLocation == MinerState.Location.Saloon ? 1 : 0))
				.AddAppraisal(new ActionAppraisal<UtilityMiner>(c => c.MinerState.Thirst > 0 ? 1 : 0));
			thirstConsideration.Action = new ActionExecutor<UtilityMiner>(c => c.Drink());
			reasoner.AddConsideration(thirstConsideration);

			// depositing gold is next
			// AllOrNothingQualifier with required threshold of 1 so all scorers must score
			//  - we have to be at the bank to deposit gold
			//  - we have to have gold to deposit
			var goldConsideration = new AllOrNothingConsideration<UtilityMiner>(1)
				.AddAppraisal(new ActionAppraisal<UtilityMiner>(c =>
					c.MinerState.CurrentLocation == MinerState.Location.Bank ? 1 : 0))
				.AddAppraisal(new ActionAppraisal<UtilityMiner>(c => c.MinerState.Gold > 0 ? 1 : 0));
			goldConsideration.Action = new ActionExecutor<UtilityMiner>(c => c.DepositGold());
			reasoner.AddConsideration(goldConsideration);

			// decide where to go. this will override mining and send us elsewhere if a scorer scores
			// AllOrNothingQualifier with required threshold of 0 so we get a sum of all scorers
			//  - if we are max fatigued score
			//  - if we are max thirsty score
			//  - if we are at max gold score
			//  - if we are not at the mine score
			// Action has a scorer to score all the locations. It then moves to the location that scored highest.
			var moveConsideration = new AllOrNothingConsideration<UtilityMiner>(0)
				.AddAppraisal(new ActionAppraisal<UtilityMiner>(c =>
					c.MinerState.Fatigue >= MinerState.MaxFatigue ? 1 : 0))
				.AddAppraisal(
					new ActionAppraisal<UtilityMiner>(c => c.MinerState.Thirst >= MinerState.MaxThirst ? 1 : 0))
				.AddAppraisal(new ActionAppraisal<UtilityMiner>(c => c.MinerState.Gold >= MinerState.MaxGold ? 1 : 0))
				.AddAppraisal(new ActionAppraisal<UtilityMiner>(c =>
					c.MinerState.CurrentLocation != MinerState.Location.Mine ? 1 : 0));
			var moveAction = new MoveToBestLocation();
			moveAction.AddScorer(new ChooseBestLocation());
			moveConsideration.Action = moveAction;
			reasoner.AddConsideration(moveConsideration);

			// mining is last
			// AllOrNothingQualifier with required threshold of 1 so all scorers must score
			//  - we have to be at the mine to dig for gold
			//  - we have to not be at our max gold
			var mineConsideration = new AllOrNothingConsideration<UtilityMiner>(1)
				.AddAppraisal(new ActionAppraisal<UtilityMiner>(c =>
					c.MinerState.CurrentLocation == MinerState.Location.Mine ? 1 : 0))
				.AddAppraisal(new ActionAppraisal<UtilityMiner>(c => c.MinerState.Gold >= MinerState.MaxGold ? 0 : 1));
			mineConsideration.Action = new ActionExecutor<UtilityMiner>(c => c.DigForGold());
			reasoner.AddConsideration(mineConsideration);

			// default, fall-through action is to head to the mine
			reasoner.DefaultConsideration.Action =
				new ActionExecutor<UtilityMiner>(c => c.GoToLocation(MinerState.Location.Mine));

			_ai = new UtilityAI<UtilityMiner>(this, reasoner);
		}


		public void Update()
		{
			_ai.Tick();
		}


		public void Sleep()
		{
			Debug.Log("getting some sleep. current fatigue {0}", MinerState.Fatigue);
			MinerState.Fatigue--;
		}


		public void Drink()
		{
			Debug.Log("getting my drink on. Thirst level {0}", MinerState.Thirst);
			MinerState.Thirst--;
		}


		public void DepositGold()
		{
			MinerState.GoldInBank += MinerState.Gold;
			MinerState.Gold = 0;

			Debug.Log("depositing gold at the bank. current wealth {0}", MinerState.GoldInBank);
		}


		public void DigForGold()
		{
			Debug.Log("digging for gold. nuggets found {0}", MinerState.Gold);
			MinerState.Gold++;
			MinerState.Fatigue++;
			MinerState.Thirst++;
		}


		public void GoToLocation(MinerState.Location location)
		{
			if (location == MinerState.CurrentLocation)
				return;

			if (MinerState.CurrentLocation == MinerState.Location.InTransit && location == _destinationLocation)
			{
				Debug.Log("heading to {0}. its {1} miles away", location, _distanceToNextLocation);
				_distanceToNextLocation--;
				if (_distanceToNextLocation == 0)
				{
					MinerState.Fatigue++;
					MinerState.CurrentLocation = _destinationLocation;
					_distanceToNextLocation = Random.Range(2, 8);
				}
			}
			else
			{
				MinerState.CurrentLocation = MinerState.Location.InTransit;
				_destinationLocation = location;
				_distanceToNextLocation = Random.Range(2, 8);
			}
		}
	}
}