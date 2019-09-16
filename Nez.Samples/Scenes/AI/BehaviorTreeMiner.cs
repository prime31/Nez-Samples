using Nez.AI.BehaviorTrees;


namespace Nez.Samples
{
	/// <summary>
	/// implments our friend miner bob with behavior trees. The same tree is built using self abort and lower priority types to illustrate
	/// different ways of using behavior trees.
	/// </summary>
	public class BehaviorTreeMiner : Component, IUpdatable
	{
		public MinerState MinerState = new MinerState();

		BehaviorTree<BehaviorTreeMiner> _tree;
		int _distanceToNextLocation = 10;


		public void BuildSelfAbortTree()
		{
			var builder = BehaviorTreeBuilder<BehaviorTreeMiner>.Begin(this);

			builder.Selector(AbortTypes.Self);

			// sleep is most important
			builder.ConditionalDecorator(m => m.MinerState.Fatigue >= MinerState.MaxFatigue, false);
			builder.Sequence()
				.LogAction("--- tired! gotta go home")
				.Action(m => m.GoToLocation(MinerState.Location.Home))
				.LogAction("--- prep me my bed!")
				.Action(m => m.Sleep())
				.EndComposite();

			// thirst is next most important
			builder.ConditionalDecorator(m => m.MinerState.Thirst >= MinerState.MaxThirst, false);
			builder.Sequence()
				.LogAction("--- thirsty! time for a drink")
				.Action(m => m.GoToLocation(MinerState.Location.Saloon))
				.LogAction("--- get me a drink!")
				.Action(m => m.Drink())
				.EndComposite();

			// dropping off gold is next
			builder.ConditionalDecorator(m => m.MinerState.Gold >= MinerState.MaxGold, false);
			builder.Sequence()
				.LogAction("--- bags are full! gotta drop this off at the bank.")
				.Action(m => m.GoToLocation(MinerState.Location.Bank))
				.LogAction("--- take me gold!")
				.Action(m => m.DepositGold())
				.EndComposite();

			// fetching gold is last
			builder.Sequence()
				.Action(m => m.GoToLocation(MinerState.Location.Mine))
				.LogAction("--- time to get me some gold!")
				.Action(m => m.DigForGold())
				.EndComposite();

			builder.EndComposite();

			_tree = builder.Build();
		}


		// the same tree is here, once with LowerPriority aborts and once with Self aborts and ConditionalDecorators
		public void BuildLowerPriorityAbortTree()
		{
			var builder = BehaviorTreeBuilder<BehaviorTreeMiner>.Begin(this);

			builder.Selector();

			// sleep is most important
			builder.Sequence(AbortTypes.LowerPriority)
				.Conditional(m => m.MinerState.Fatigue >= MinerState.MaxFatigue)
				.LogAction("--- tired! gotta go home")
				.Action(m => m.GoToLocation(MinerState.Location.Home))
				.LogAction("--- prep me my bed!")
				.Action(m => m.Sleep())
				.EndComposite();

			// thirst is next most important
			builder.Sequence(AbortTypes.LowerPriority)
				.Conditional(m => m.MinerState.Thirst >= MinerState.MaxThirst)
				.LogAction("--- thirsty! time for a drink")
				.Action(m => m.GoToLocation(MinerState.Location.Saloon))
				.LogAction("--- get me a drink!")
				.Action(m => m.Drink())
				.EndComposite();

			// dropping off gold is next
			builder.Sequence(AbortTypes.LowerPriority)
				.Conditional(m => m.MinerState.Gold >= MinerState.MaxGold)
				.LogAction("--- bags are full! gotta drop this off at the bank.")
				.Action(m => m.GoToLocation(MinerState.Location.Bank))
				.LogAction("--- take me gold!")
				.Action(m => m.DepositGold())
				.EndComposite();

			// fetching gold is last
			builder.Sequence()
				.Action(m => m.GoToLocation(MinerState.Location.Mine))
				.LogAction("--- time to get me some gold!")
				.Action(m => m.DigForGold())
				.EndComposite();

			builder.EndComposite();

			_tree = builder.Build();
		}


		public void Update()
		{
			if (_tree != null)
				_tree.Tick();
		}


		TaskStatus GoToLocation(MinerState.Location location)
		{
			Debug.Log("heading to {0}. its {1} miles away", location, _distanceToNextLocation);

			if (location != MinerState.CurrentLocation)
			{
				_distanceToNextLocation--;
				if (_distanceToNextLocation == 0)
				{
					MinerState.Fatigue++;
					MinerState.CurrentLocation = location;
					_distanceToNextLocation = Random.Range(2, 8);

					return TaskStatus.Success;
				}

				return TaskStatus.Running;
			}

			return TaskStatus.Success;
		}


		TaskStatus Sleep()
		{
			Debug.Log("getting some sleep. current fatigue {0}", MinerState.Fatigue);

			if (MinerState.Fatigue == 0)
				return TaskStatus.Success;

			MinerState.Fatigue--;
			return TaskStatus.Running;
		}


		TaskStatus Drink()
		{
			Debug.Log("getting my drink on. Thirst level {0}", MinerState.Thirst);

			if (MinerState.Thirst == 0)
				return TaskStatus.Success;

			MinerState.Thirst--;
			return TaskStatus.Running;
		}


		TaskStatus DepositGold()
		{
			MinerState.GoldInBank += MinerState.Gold;
			MinerState.Gold = 0;

			Debug.Log("depositing gold at the bank. current wealth {0}", MinerState.GoldInBank);

			return TaskStatus.Success;
		}


		TaskStatus DigForGold()
		{
			Debug.Log("digging for gold. nuggets found {0}", MinerState.Gold);
			MinerState.Gold++;
			MinerState.Fatigue++;
			MinerState.Thirst++;

			if (MinerState.Gold >= MinerState.MaxGold)
				return TaskStatus.Failure;

			return TaskStatus.Running;
		}
	}
}