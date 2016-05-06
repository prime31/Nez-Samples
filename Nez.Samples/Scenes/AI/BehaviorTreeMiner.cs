using System;
using Nez.AI.BehaviorTrees;


namespace Nez.Samples
{
	public class BehaviorTreeMiner : Component, IUpdatable
	{
		public enum Location
		{
			Bank,
			Mine,
			Home,
			Saloon
		}

		const int MAX_FATIGUE = 10;
		const int MAX_GOLD = 8;
		const int MAX_THIRST = 5;

		int _fatigue;
		int _thirst;
		int _gold;
		int _goldInBank;

		BehaviorTree<BehaviorTreeMiner> _tree;
		Location _currentLocation = Location.Home;
		int _distanceToNextLocation = 10;


		public void buildSelfAbortTree()
		{
			var builder = BehaviorTreeBuilder<BehaviorTreeMiner>.begin( this );

			builder.selector( AbortTypes.Self );

			// sleep is most important
			builder.conditionalDecorator( m => m._fatigue >= MAX_FATIGUE, false );
			builder.sequence()
				.logAction( "--- tired! gotta go home" )
				.action( m => m.goToLocation( Location.Home ) )
				.logAction( "--- prep me my bed!" )
				.action( m => m.sleep() )
				.endComposite();

			// thirst is next most important
			builder.conditionalDecorator( m => m._thirst >= MAX_THIRST, false );
			builder.sequence()
				.logAction( "--- thirsty! time for a drink" )
				.action( m => m.goToLocation( Location.Saloon ) )
				.logAction( "--- get me a drink!" )
				.action( m => m.drink() )
				.endComposite();

			// dropping off gold is next
			builder.conditionalDecorator( m => m._gold >= MAX_GOLD, false );
			builder.sequence()
				.logAction( "--- bags are full! gotta drop this off at the bank." )
				.action( m => m.goToLocation( Location.Bank ) )
				.logAction( "--- take me gold!" )
				.action( m => m.depositGold() )
				.endComposite();

			// fetching gold is last
			builder.sequence()
				.action( m => m.goToLocation( Location.Mine ) )
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
				.conditional( m => m._fatigue >= MAX_FATIGUE )
				.logAction( "--- tired! gotta go home" )
				.action( m => m.goToLocation( Location.Home ) )
				.logAction( "--- prep me my bed!" )
				.action( m => m.sleep() )
				.endComposite();

			// thirst is next most important
			builder.sequence( AbortTypes.LowerPriority )
				.conditional( m => m._thirst >= MAX_THIRST )
				.logAction( "--- thirsty! time for a drink" )
				.action( m => m.goToLocation( Location.Saloon ) )
				.logAction( "--- get me a drink!" )
				.action( m => m.drink() )
				.endComposite();

			// dropping off gold is next
			builder.sequence( AbortTypes.LowerPriority )
				.conditional( m => m._gold >= MAX_GOLD )
				.logAction( "--- bags are full! gotta drop this off at the bank." )
				.action( m => m.goToLocation( Location.Bank ) )
				.logAction( "--- take me gold!" )
				.action( m => m.depositGold() )
				.endComposite();

			// fetching gold is last
			builder.sequence()
				.action( m => m.goToLocation( Location.Mine ) )
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


		TaskStatus goToLocation( Location location )
		{
			Debug.log( "heading to {0}. its {1} miles away", location, _distanceToNextLocation );

			if( location != _currentLocation )
			{
				_distanceToNextLocation--;
				if( _distanceToNextLocation == 0 )
				{
					_fatigue++;
					_currentLocation = location;
					_distanceToNextLocation = Nez.Random.range( 2, 8 );

					return TaskStatus.Success;
				}

				return TaskStatus.Running;
			}
			return TaskStatus.Success;
		}


		TaskStatus sleep()
		{
			Debug.log( "getting some sleep. current fatigue {0}", _fatigue );

			if( _fatigue == 0 )
				return TaskStatus.Success;

			_fatigue--;
			return TaskStatus.Running;
		}


		TaskStatus drink()
		{
			Debug.log( "getting my drink on. Thirst level {0}", _thirst );

			if( _thirst == 0 )
				return TaskStatus.Success;

			_thirst--;
			return TaskStatus.Running;
		}


		TaskStatus depositGold()
		{
			_goldInBank += _gold;
			_gold = 0;

			Debug.log( "depositing gold at the bank. current wealth {0}", _goldInBank );

			return TaskStatus.Success;
		}


		TaskStatus digForGold()
		{
			Debug.log( "digging for gold. nuggets found {0}", _gold );
			_gold++;
			_fatigue++;
			_thirst++;

			if( _gold >= MAX_GOLD )
				return TaskStatus.Failure;

			return TaskStatus.Running;
		}

	}
}

