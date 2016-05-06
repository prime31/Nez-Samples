using System;
using Nez.AI.UtilityAI;


namespace Nez.Samples
{
	public class ChooseBestLocation : IActionOptionAppraisal<UtilityMiner,MinerState.Location>
	{
		/// <summary>
		/// Action Appraisal that will score locations providing the highest score to the best location to visit
		/// </summary>
		/// <returns>The score.</returns>
		/// <param name="context">Context.</param>
		/// <param name="option">Option.</param>
		float IActionOptionAppraisal<UtilityMiner,MinerState.Location>.getScore( UtilityMiner context, MinerState.Location option )
		{
			if( option == MinerState.Location.Home )
				return context.minerState.fatigue >= MinerState.MAX_FATIGUE ? 20 : 0;

			if( option == MinerState.Location.Saloon )
				return context.minerState.thirst >= MinerState.MAX_THIRST ? 15 : 0;

			if( option == MinerState.Location.Bank )
			{
				if( context.minerState.gold >= MinerState.MAX_GOLD )
					return 10;

				// if we are scoring the bank and we are not at the mine we'll use a curve. the main gist of this is that if we are not at the mine
				// and we are carrying a decent amount of gold drop it off at the bank before heading to the mine again.
				if( context.minerState.currentLocation != MinerState.Location.Mine )
				{
					// normalize our current gold value to 0-1
					var gold = Mathf.map01( context.minerState.gold, 0, MinerState.MAX_GOLD );
					var score = Mathf.pow( gold, 2 );
					return score * 10;
				}

				return 0;
			}

			return 5;
		}

	}
}

