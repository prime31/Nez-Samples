using Nez.AI.BehaviorTrees;


namespace Nez.Samples
{
	/// <summary>
	/// implments our friend miner bob with behavior trees. The same tree is built using self abort and lower priority types to illustrate
	/// different ways of using behavior trees.
	/// </summary>
	public class BehaviorTreeMiner : Component, IUpdatable
	{
		public MinerState minerState = new MinerState();

		BehaviorTree<BehaviorTreeMiner> _tree;
		int _distanceToNextLocation = 10;


		public void BuildSelfAbortTree()
		{
			var builder = BehaviorTreeBuilder<BehaviorTreeMiner>.Begin( this );

			builder.Selector( AbortTypes.Self );

			// sleep is most important
			builder.ConditionalDecorator( m => m.minerState.fatigue >= MinerState.MAX_FATIGUE, false );
			builder.Sequence()
				.LogAction( "--- tired! gotta go home" )
				.Action( m => m.goToLocation( MinerState.Location.Home ) )
				.LogAction( "--- prep me my bed!" )
				.Action( m => m.sleep() )
				.EndComposite();

			// thirst is next most important
			builder.ConditionalDecorator( m => m.minerState.thirst >= MinerState.MAX_THIRST, false );
			builder.Sequence()
				.LogAction( "--- thirsty! time for a drink" )
				.Action( m => m.goToLocation( MinerState.Location.Saloon ) )
				.LogAction( "--- get me a drink!" )
				.Action( m => m.drink() )
				.EndComposite();

			// dropping off gold is next
			builder.ConditionalDecorator( m => m.minerState.gold >= MinerState.MAX_GOLD, false );
			builder.Sequence()
				.LogAction( "--- bags are full! gotta drop this off at the bank." )
				.Action( m => m.goToLocation( MinerState.Location.Bank ) )
				.LogAction( "--- take me gold!" )
				.Action( m => m.depositGold() )
				.EndComposite();

			// fetching gold is last
			builder.Sequence()
				.Action( m => m.goToLocation( MinerState.Location.Mine ) )
				.LogAction( "--- time to get me some gold!" )
				.Action( m => m.digForGold() )
				.EndComposite();

			builder.EndComposite();

			_tree = builder.Build();
		}


		// the same tree is here, once with LowerPriority aborts and once with Self aborts and ConditionalDecorators
		public void BuildLowerPriorityAbortTree()
		{
			var builder = BehaviorTreeBuilder<BehaviorTreeMiner>.Begin( this );

			builder.Selector();

			// sleep is most important
			builder.Sequence( AbortTypes.LowerPriority )
				.Conditional( m => m.minerState.fatigue >= MinerState.MAX_FATIGUE )
				.LogAction( "--- tired! gotta go home" )
				.Action( m => m.goToLocation( MinerState.Location.Home ) )
				.LogAction( "--- prep me my bed!" )
				.Action( m => m.sleep() )
				.EndComposite();

			// thirst is next most important
			builder.Sequence( AbortTypes.LowerPriority )
				.Conditional( m => m.minerState.thirst >= MinerState.MAX_THIRST )
				.LogAction( "--- thirsty! time for a drink" )
				.Action( m => m.goToLocation( MinerState.Location.Saloon ) )
				.LogAction( "--- get me a drink!" )
				.Action( m => m.drink() )
				.EndComposite();

			// dropping off gold is next
			builder.Sequence( AbortTypes.LowerPriority )
				.Conditional( m => m.minerState.gold >= MinerState.MAX_GOLD )
				.LogAction( "--- bags are full! gotta drop this off at the bank." )
				.Action( m => m.goToLocation( MinerState.Location.Bank ) )
				.LogAction( "--- take me gold!" )
				.Action( m => m.depositGold() )
				.EndComposite();

			// fetching gold is last
			builder.Sequence()
				.Action( m => m.goToLocation( MinerState.Location.Mine ) )
				.LogAction( "--- time to get me some gold!" )
				.Action( m => m.digForGold() )
				.EndComposite();

			builder.EndComposite();

			_tree = builder.Build();
		}


		public void Update()
		{
			if( _tree != null )
				_tree.Tick();
		}


		TaskStatus goToLocation( MinerState.Location location )
		{
			Debug.Log( "heading to {0}. its {1} miles away", location, _distanceToNextLocation );

			if( location != minerState.currentLocation )
			{
				_distanceToNextLocation--;
				if( _distanceToNextLocation == 0 )
				{
					minerState.fatigue++;
					minerState.currentLocation = location;
					_distanceToNextLocation = Nez.Random.Range( 2, 8 );

					return TaskStatus.Success;
				}

				return TaskStatus.Running;
			}
			return TaskStatus.Success;
		}


		TaskStatus sleep()
		{
			Debug.Log( "getting some sleep. current fatigue {0}", minerState.fatigue );

			if( minerState.fatigue == 0 )
				return TaskStatus.Success;

			minerState.fatigue--;
			return TaskStatus.Running;
		}


		TaskStatus drink()
		{
			Debug.Log( "getting my drink on. Thirst level {0}", minerState.thirst );

			if( minerState.thirst == 0 )
				return TaskStatus.Success;

			minerState.thirst--;
			return TaskStatus.Running;
		}


		TaskStatus depositGold()
		{
			minerState.goldInBank += minerState.gold;
			minerState.gold = 0;

			Debug.Log( "depositing gold at the bank. current wealth {0}", minerState.goldInBank );

			return TaskStatus.Success;
		}


		TaskStatus digForGold()
		{
			Debug.Log( "digging for gold. nuggets found {0}", minerState.gold );
			minerState.gold++;
			minerState.fatigue++;
			minerState.thirst++;

			if( minerState.gold >= MinerState.MAX_GOLD )
				return TaskStatus.Failure;

			return TaskStatus.Running;
		}

	}
}

