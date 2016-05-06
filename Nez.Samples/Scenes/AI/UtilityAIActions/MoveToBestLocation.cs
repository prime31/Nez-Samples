using System;
using System.Collections.Generic;
using Nez.AI.UtilityAI;


namespace Nez.Samples
{
	public class MoveToBestLocation : ActionWithOptions<UtilityMiner, UtilityMiner.Location>
	{
		List<UtilityMiner.Location> _locations = new List<UtilityMiner.Location>()
		{
			UtilityMiner.Location.Bank,
			UtilityMiner.Location.Home,
			UtilityMiner.Location.Mine,
			UtilityMiner.Location.Saloon
		};


		public override void execute( UtilityMiner context )
		{
			var location = getBestOption( context, _locations );

			context.goToLocation( location );
		}

	}
}

