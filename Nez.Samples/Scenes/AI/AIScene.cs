using System;


namespace Nez.Samples
{
	[SampleScene( "AI", 120, "No visuals for this demo, everything is in the logs\nThe class 'miner bob' scenario is implemented with each AI type" )]
	public class AIScene : SampleScene
	{
		public override void initialize()
		{
			addRenderer( new DefaultRenderer() );

			createEntity( "ai-ui" )
				.addComponent<AIUI>();

			createEntity( "miner" )
				.addComponent<BehaviorTreeMiner>();

			createEntity( "utility-miner" )
				.addComponent<UtilityMiner>();

			createEntity( "goap-miner" )
				.addComponent<GOAPMiner>();
		}
	}
}

