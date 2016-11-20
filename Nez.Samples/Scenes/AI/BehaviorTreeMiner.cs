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


		public void buildSelfAbortTree()
		{
			var builder = BehaviorTreeBuilder<BehaviorTreeMiner>.begin( this );

			builder.selector( AbortTypes.Self );

			// sleep is most important
			builder.conditionalDecorator( m => m.minerState.fatigue >= MinerState.MAX_FATIGUE, false );
			builder.sequence()
				.logAction( "--- tired! gotta go home" )
				.action( m => m.goToLocation( MinerState.Location.Home ) )
				.logAction( "--- prep me my bed!" )
				.action( m => m.sleep() )
				.endComposite();

			// thirst is next most important
			builder.conditionalDecorator( m => m.minerState.thirst >= MinerState.MAX_THIRST, false );
			builder.sequence()
				.logAction( "--- thirsty! time for a drink" )
				.action( m => m.goToLocation( MinerState.Location.Saloon ) )
				.logAction( "--- get me a drink!" )
				.action( m => m.drink() )
				.endComposite();

			// dropping off gold is next
			builder.conditionalDecorator( m => m.minerState.gold >= MinerState.MAX_GOLD, false );
			builder.sequence()
				.logAction( "--- bags are full! gotta drop this off at the bank." )
				.action( m => m.goToLocation( MinerState.Location.Bank ) )
				.logAction( "--- take me gold!" )
				.action( m => m.depositGold() )
				.endComposite();

			// fetching gold is last
			builder.sequence()
				.action( m => m.goToLocation( MinerState.Location.Mine ) )
				.logAction( "--- time to get me some gold!" )
				.action( m => m.digForGold() )
				.endComposite();

			builder.endComposite();

			_tree = builder.build();
		}


		// the same tree is here, once with LowerPriority aborts and once with Self aborts and ConditionalDecorators
		public void buildLowerPriorityAbortTree()
		{
			var builder = BehaviorTreeBuilder<BehaviorTreeMiner>.begin( this );

			builder.selector();

			// sleep is most important
			builder.sequence( AbortTypes.LowerPriority )
				.conditional( m => m.minerState.fatigue >= MinerState.MAX_FATIGUE )
				.logAction( "--- tired! gotta go home" )
				.action( m => m.goToLocation( MinerState.Location.Home ) )
				.logAction( "--- prep me my bed!" )
				.action( m => m.sleep() )
				.endComposite();

			// thirst is next most important
			builder.sequence( AbortTypes.LowerPriority )
				.conditional( m => m.minerState.thirst >= MinerState.MAX_THIRST )
				.logAction( "--- thirsty! time for a drink" )
				.action( m => m.goToLocation( MinerState.Location.Saloon ) )
				.logAction( "--- get me a drink!" )
				.action( m => m.drink() )
				.endComposite();

			// dropping off gold is next
			builder.sequence( AbortTypes.LowerPriority )
				.conditional( m => m.minerState.gold >= MinerState.MAX_GOLD )
				.logAction( "--- bags are full! gotta drop this off at the bank." )
				.action( m => m.goToLocation( MinerState.Location.Bank ) )
				.logAction( "--- take me gold!" )
				.action( m => m.depositGold() )
				.endComposite();

			// fetching gold is last
			builder.sequence()
				.action( m => m.goToLocation( MinerState.Location.Mine ) )
				.logAction( "--- time to get me some gold!" )
				.action( m => m.digForGold() )
				.endComposite();

			builder.endComposite();

			_tree = builder.build();
		}


		public void update()
		{
			if( _tree != null )
				_tree.tick();
		}


		TaskStatus goToLocation( MinerState.Location location )
		{
			Debug.log( "heading to {0}. its {1} miles away", location, _distanceToNextLocation );

			if( location != minerState.currentLocation )
			{
				_distanceToNextLocation--;
				if( _distanceToNextLocation == 0 )
				{
					minerState.fatigue++;
					minerState.currentLocation = location;
					_distanceToNextLocation = Nez.Random.range( 2, 8 );

					return TaskStatus.Success;
				}

				return TaskStatus.Running;
			}
			return TaskStatus.Success;
		}


		TaskStatus sleep()
		{
			Debug.log( "getting some sleep. current fatigue {0}", minerState.fatigue );

			if( minerState.fatigue == 0 )
				return TaskStatus.Success;

			minerState.fatigue--;
			return TaskStatus.Running;
		}


		TaskStatus drink()
		{
			Debug.log( "getting my drink on. Thirst level {0}", minerState.thirst );

			if( minerState.thirst == 0 )
				return TaskStatus.Success;

			minerState.thirst--;
			return TaskStatus.Running;
		}


		TaskStatus depositGold()
		{
			minerState.goldInBank += minerState.gold;
			minerState.gold = 0;

			Debug.log( "depositing gold at the bank. current wealth {0}", minerState.goldInBank );

			return TaskStatus.Success;
		}


		TaskStatus digForGold()
		{
			Debug.log( "digging for gold. nuggets found {0}", minerState.gold );
			minerState.gold++;
			minerState.fatigue++;
			minerState.thirst++;

			if( minerState.gold >= MinerState.MAX_GOLD )
				return TaskStatus.Failure;

			return TaskStatus.Running;
		}

	}
}

