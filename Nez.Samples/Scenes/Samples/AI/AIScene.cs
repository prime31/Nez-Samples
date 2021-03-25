namespace Nez.Samples
{
	[SampleScene("AI", 120,
		"No visuals for this demo, everything is in the logs\nThe class 'miner bob' scenario is implemented with each AI type")]
	public class AiScene : SampleScene
	{
		public override void Initialize()
		{
			AddRenderer(new DefaultRenderer());

			CreateEntity("ai-ui")
				.AddComponent<Aiui>();

			CreateEntity("miner")
				.AddComponent<BehaviorTreeMiner>();

			CreateEntity("utility-miner")
				.AddComponent<UtilityMiner>();

			CreateEntity("goap-miner")
				.AddComponent<GoapMiner>();
		}
	}
}