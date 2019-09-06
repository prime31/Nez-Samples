using Nez.UI;


namespace Nez.Samples
{
	/// <summary>
	/// component that displays a simple UI to enable/disable the different AI types
	/// </summary>
	public class AIUI : UICanvas
	{
		public override RectangleF Bounds { get { return new RectangleF( 0, 0, 200, 200 ); } }

		BehaviorTreeMiner _miner;
		UtilityMiner _utilityMiner;
		GOAPMiner _goapMiner;


		public override void OnAddedToEntity()
		{
			base.OnAddedToEntity();

			// setup a Skin and a Table for our UI
			var skin = Skin.CreateDefaultSkin();
			var table = Stage.AddElement( new Table() );
			table.Defaults().SetPadTop( 10 ).SetMinWidth( 170 ).SetMinHeight( 30 );
			table.SetFillParent( true ).Center();

			// add a button for each of the actions/AI types we need
			table.Add( new TextButton( "BT: LowerPriority Abort Tree", skin ) )
				 .GetElement<TextButton>()
				 .OnClicked += onClickBtLowerPriority;
			table.Row();

			table.Add( new TextButton( "BT: Self Abort Tree", skin ) )
				 .GetElement<TextButton>()
				 .OnClicked += onClickBtSelfAbort;
			table.Row();

			table.Add( new TextButton( "Utility AI", skin ) )
				 .GetElement<TextButton>()
				 .OnClicked += onClickUtilityAI;
			table.Row();

			table.Add( new TextButton( "GOAP", skin ) )
				 .GetElement<TextButton>()
				 .OnClicked += onClickGoap;
			table.Row().SetPadTop( 40 );

			table.Add( new TextButton( "Stop All Running AI", skin ) )
				 .GetElement<TextButton>()
				 .OnClicked += onClickStopAllAi;

			// fetch our different AI Components
			_miner = Entity.Scene.FindComponentOfType<BehaviorTreeMiner>();
			_utilityMiner = Entity.Scene.FindComponentOfType<UtilityMiner>();
			_goapMiner = Entity.Scene.FindComponentOfType<GOAPMiner>();
		}


		#region button click handlers

		void onClickBtLowerPriority( Button button )
		{
			Debug.Log( "------ Enabled Behavior Tree LowerPriority Abort ------" );
			disableAllAI();
			_miner.BuildLowerPriorityAbortTree();
			_miner.SetEnabled( true );
		}


		void onClickBtSelfAbort( Button button )
		{
			Debug.Log( "------ Enabled Behavior Tree Self Abort ------" );
			disableAllAI();
			_miner.BuildSelfAbortTree();
			_miner.SetEnabled( true );
		}


		void onClickUtilityAI( Button button )
		{
			Debug.Log( "------ Enabled Utility AI ------" );
			disableAllAI();
			_utilityMiner.SetEnabled( true );
		}


		void onClickGoap( Button button )
		{
			Debug.Log( "------ Enabled GOAP ------" );
			disableAllAI();
			_goapMiner.SetEnabled( true );
		}


		void onClickStopAllAi( Button button )
		{
			disableAllAI();
		}


		void disableAllAI()
		{
			_miner.SetEnabled( false );
			_utilityMiner.SetEnabled( false );
			_goapMiner.SetEnabled( false );			
		}

		#endregion

	}
}

