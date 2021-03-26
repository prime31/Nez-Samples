using Nez.UI;


namespace Nez.Samples
{
	/// <summary>
	/// component that displays a simple UI to enable/disable the different AI types
	/// </summary>
	public class Aiui : UICanvas
	{
        public override RectangleF Bounds => new RectangleF(0, 0, 200, 200);

        BehaviorTreeMiner _miner;
		UtilityMiner _utilityMiner;
		GoapMiner _goapMiner;


		public override void OnAddedToEntity()
		{
			base.OnAddedToEntity();

			// setup a Skin and a Table for our UI
			var skin = Skin.CreateDefaultSkin();
			var table = Stage.AddElement(new Table());
			table.Defaults().SetPadTop(10).SetMinWidth(170).SetMinHeight(30);
			table.SetFillParent(true).Center();

			// add a button for each of the actions/AI types we need
			table.Add(new TextButton("BT: LowerPriority Abort Tree", skin))
				.GetElement<TextButton>()
				.OnClicked += OnClickBtLowerPriority;
			table.Row();

			table.Add(new TextButton("BT: Self Abort Tree", skin))
				.GetElement<TextButton>()
				.OnClicked += OnClickBtSelfAbort;
			table.Row();

			table.Add(new TextButton("Utility AI", skin))
				.GetElement<TextButton>()
				.OnClicked += OnClickUtilityAi;
			table.Row();

			table.Add(new TextButton("GOAP", skin))
				.GetElement<TextButton>()
				.OnClicked += OnClickGoap;
			table.Row().SetPadTop(40);

			table.Add(new TextButton("Stop All Running AI", skin))
				.GetElement<TextButton>()
				.OnClicked += OnClickStopAllAi;

			// fetch our different AI Components
			_miner = Entity.Scene.FindComponentOfType<BehaviorTreeMiner>();
			_utilityMiner = Entity.Scene.FindComponentOfType<UtilityMiner>();
			_goapMiner = Entity.Scene.FindComponentOfType<GoapMiner>();
		}


		#region button click handlers

		void OnClickBtLowerPriority(Button button)
		{
			Debug.Log("------ Enabled Behavior Tree LowerPriority Abort ------");
			DisableAllAi();
			_miner.BuildLowerPriorityAbortTree();
			_miner.SetEnabled(true);
		}


		void OnClickBtSelfAbort(Button button)
		{
			Debug.Log("------ Enabled Behavior Tree Self Abort ------");
			DisableAllAi();
			_miner.BuildSelfAbortTree();
			_miner.SetEnabled(true);
		}


		void OnClickUtilityAi(Button button)
		{
			Debug.Log("------ Enabled Utility AI ------");
			DisableAllAi();
			_utilityMiner.SetEnabled(true);
		}


		void OnClickGoap(Button button)
		{
			Debug.Log("------ Enabled GOAP ------");
			DisableAllAi();
			_goapMiner.SetEnabled(true);
		}


		void OnClickStopAllAi(Button button)
		{
			DisableAllAi();
		}


		void DisableAllAi()
		{
			_miner.SetEnabled(false);
			_utilityMiner.SetEnabled(false);
			_goapMiner.SetEnabled(false);
		}

		#endregion
	}
}