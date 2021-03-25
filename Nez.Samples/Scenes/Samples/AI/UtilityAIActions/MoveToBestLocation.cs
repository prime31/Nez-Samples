using System.Collections.Generic;
using Nez.AI.UtilityAI;


namespace Nez.Samples
{
	/// <summary>
	/// ActionWithOptions lets an Action setup an Appraisal that will score a list of options. In our miner bob example, the options
	/// are the locations and the Appraisal will score the best location to go to.
	/// </summary>
	public class MoveToBestLocation : ActionWithOptions<UtilityMiner, MinerState.Location>
	{
		List<MinerState.Location> _locations = new List<MinerState.Location>()
		{
			MinerState.Location.Bank,
			MinerState.Location.Home,
			MinerState.Location.Mine,
			MinerState.Location.Saloon
		};


		public override void Execute(UtilityMiner context)
		{
			var location = GetBestOption(context, _locations);
			context.GoToLocation(location);
		}
	}
}