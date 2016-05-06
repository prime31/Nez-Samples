using System;
using Nez.AI.UtilityAI;


namespace Nez.Samples
{
	/// <summary>
	/// Utility AI example of miner bob. Utility AI is the most complex of all the AI types to setup. The complexity comes with a lot of power
	/// though.
	/// </summary>
	public class UtilityMiner : Component, IUpdatable
	{
		public MinerState minerState = new MinerState();

		UtilityAI<UtilityMiner> _ai;
		MinerState.Location _destinationLocation;
		int _distanceToNextLocation = 10;


		public override void onAddedToEntity()
		{
			enabled = false;
			var reasoner = new FirstScoreReasoner<UtilityMiner>();

			// sleep is most important
			// AllOrNothingQualifier with required threshold of 1 so all scorers must score
			//  - we have to be home to sleep
			//  - we have to have some fatigue
			var fatigueConsideration = new AllOrNothingConsideration<UtilityMiner>( 1 )
				.addAppraisal( new ActionAppraisal<UtilityMiner>( c => c.minerState.currentLocation == MinerState.Location.Home ? 1 : 0 ) )
				.addAppraisal( new ActionAppraisal<UtilityMiner>( c => c.minerState.fatigue > 0 ? 1 : 0 ) );
			fatigueConsideration.action = new ActionExecutor<UtilityMiner>( c => c.sleep() );
			reasoner.addConsideration( fatigueConsideration );

			// thirst is next
			// AllOrNothingQualifier with required threshold of 1 so all scorers must score
			//  - we have to be at the saloon to drink
			//  - we have to be thirsty
			var thirstConsideration = new AllOrNothingConsideration<UtilityMiner>( 1 )
				.addAppraisal( new ActionAppraisal<UtilityMiner>( c => c.minerState.currentLocation == MinerState.Location.Saloon ? 1 : 0 ) )
				.addAppraisal( new ActionAppraisal<UtilityMiner>( c => c.minerState.thirst > 0 ? 1 : 0 ) );
			thirstConsideration.action = new ActionExecutor<UtilityMiner>( c => c.drink() );
			reasoner.addConsideration( thirstConsideration );

			// depositing gold is next
			// AllOrNothingQualifier with required threshold of 1 so all scorers must score
			//  - we have to be at the bank to deposit gold
			//  - we have to have gold to deposit
			var goldConsideration = new AllOrNothingConsideration<UtilityMiner>( 1 )
				.addAppraisal( new ActionAppraisal<UtilityMiner>( c => c.minerState.currentLocation == MinerState.Location.Bank ? 1 : 0 ) )
				.addAppraisal( new ActionAppraisal<UtilityMiner>( c => c.minerState.gold > 0 ? 1 : 0 ) );
			goldConsideration.action = new ActionExecutor<UtilityMiner>( c => c.depositGold() );
			reasoner.addConsideration( goldConsideration );

			// decide where to go. this will override mining and send us elsewhere if a scorer scores
			// AllOrNothingQualifier with required threshold of 0 so we get a sum of all scorers
			//  - if we are max fatigued score
			//  - if we are max thirsty score
			//  - if we are at max gold score
			//  - if we are not at the mine score
			// Action has a scorer to score all the locations. It then moves to the location that scored highest.
			var moveConsideration = new AllOrNothingConsideration<UtilityMiner>( 0 )
				.addAppraisal( new ActionAppraisal<UtilityMiner>( c => c.minerState.fatigue >= MinerState.MAX_FATIGUE ? 1 : 0 ) )
				.addAppraisal( new ActionAppraisal<UtilityMiner>( c => c.minerState.thirst >= MinerState.MAX_THIRST ? 1 : 0 ) )
				.addAppraisal( new ActionAppraisal<UtilityMiner>( c => c.minerState.gold >= MinerState.MAX_GOLD ? 1 : 0 ) )
				.addAppraisal( new ActionAppraisal<UtilityMiner>( c => c.minerState.currentLocation != MinerState.Location.Mine ? 1 : 0 ) );
			var moveAction = new MoveToBestLocation();
			moveAction.addScorer( new ChooseBestLocation() );
			moveConsideration.action = moveAction;
			reasoner.addConsideration( moveConsideration );

			// mining is last
			// AllOrNothingQualifier with required threshold of 1 so all scorers must score
			//  - we have to be at the mine to dig for gold
			//  - we have to not be at our max gold
			var mineConsideration = new AllOrNothingConsideration<UtilityMiner>( 1 )
				.addAppraisal( new ActionAppraisal<UtilityMiner>( c => c.minerState.currentLocation == MinerState.Location.Mine ? 1 : 0 ) )
				.addAppraisal( new ActionAppraisal<UtilityMiner>( c => c.minerState.gold >= MinerState.MAX_GOLD ? 0 : 1 ) );
			mineConsideration.action = new ActionExecutor<UtilityMiner>( c => c.digForGold() );
			reasoner.addConsideration( mineConsideration );

			// default, fall-through action is to head to the mine
			reasoner.defaultConsideration.action = new ActionExecutor<UtilityMiner>( c => c.goToLocation( MinerState.Location.Mine ) );

			_ai = new UtilityAI<UtilityMiner>( this, reasoner );
		}


		public void update()
		{
			_ai.tick();
		}


		public void sleep()
		{
			Debug.log( "getting some sleep. current fatigue {0}", minerState.fatigue );
			minerState.fatigue--;
		}


		public void drink()
		{
			Debug.log( "getting my drink on. Thirst level {0}", minerState.thirst );
			minerState.thirst--;
		}


		public void depositGold()
		{
			minerState.goldInBank += minerState.gold;
			minerState.gold = 0;

			Debug.log( "depositing gold at the bank. current wealth {0}", minerState.goldInBank );
		}


		public void digForGold()
		{
			Debug.log( "digging for gold. nuggets found {0}", minerState.gold );
			minerState.gold++;
			minerState.fatigue++;
			minerState.thirst++;
		}


		public void goToLocation( MinerState.Location location )
		{
			if( location == minerState.currentLocation )
				return;

			if( minerState.currentLocation == MinerState.Location.InTransit && location == _destinationLocation )
			{
				Debug.log( "heading to {0}. its {1} miles away", location, _distanceToNextLocation );
				_distanceToNextLocation--;
				if( _distanceToNextLocation == 0 )
				{
					minerState.fatigue++;
					minerState.currentLocation = _destinationLocation;
					_distanceToNextLocation = Nez.Random.range( 2, 8 );
				}
			}
			else
			{
				minerState.currentLocation = MinerState.Location.InTransit;
				_destinationLocation = location; 
				_distanceToNextLocation = Nez.Random.range( 2, 8 );
			}
		}

	}
}

