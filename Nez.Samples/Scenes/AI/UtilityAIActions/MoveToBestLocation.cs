using System;
using System.Collections.Generic;
using Nez.AI.UtilityAI;


namespace Nez.Samples
{
	public class MoveToBestLocation : ActionWithOptions<UtilityMiner,MinerState.Location>
	{
		List<MinerState.Location> _locations = new List<MinerState.Location>()
		{
			MinerState.Location.Bank,
			MinerState.Location.Home,
			MinerState.Location.Mine,
			MinerState.Location.Saloon
		};


		public override void execute( UtilityMiner context )
		{
			var location = getBestOption( context, _locations );

			context.goToLocation( location );
		}

	}
}

