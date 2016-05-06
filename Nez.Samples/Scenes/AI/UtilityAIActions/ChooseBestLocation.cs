using System;
using Nez.AI.UtilityAI;


namespace Nez.Samples
{
	public class ChooseBestLocation : IActionOptionAppraisal<UtilityMiner, UtilityMiner.Location>
	{
		float IActionOptionAppraisal<UtilityMiner,UtilityMiner.Location>.getScore( UtilityMiner context, UtilityMiner.Location option )
		{
			if( option == UtilityMiner.Location.Home )
				return context.fatigue >= UtilityMiner.MAX_FATIGUE ? 20 : 0;

			if( option == UtilityMiner.Location.Saloon )
				return context.thirst >= UtilityMiner.MAX_THIRST ? 15 : 0;

			if( option == UtilityMiner.Location.Bank )
			{
				if( context.gold >= UtilityMiner.MAX_GOLD )
					return 10;

				// if we are scoring the bank and we are not at the mine we'll use a curve. the main gist of this is that if we are not at the mine
				// and we are carrying a decent amount of gold drop it off at the bank before heading to the mine again.
				if( context.currentLocation != UtilityMiner.Location.Mine )
				{
					// normalize our current gold value to 0-1
					var gold = Mathf.map01( context.gold, 0, UtilityMiner.MAX_GOLD );
					var score = Mathf.pow( gold, 2 );
					return score * 10;
				}

				return 0;
			}

			return 5;
		}

	}
}

