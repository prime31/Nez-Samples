using Nez.UI;


namespace Nez.Samples
{
	/// <summary>
	/// component that displays a simple UI to enable/disable the different AI types
	/// </summary>
	public class AIUI : UICanvas
	{
		public override RectangleF bounds { get { return new RectangleF( 0, 0, 200, 200 ); } }

		BehaviorTreeMiner _miner;
		UtilityMiner _utilityMiner;
		GOAPMiner _goapMiner;


		public override void onAddedToEntity()
		{
			base.onAddedToEntity();

			// setup a Skin and a Table for our UI
			var skin = Skin.createDefaultSkin();
			var table = stage.addElement( new Table() );
			table.defaults().setPadTop( 10 ).setMinWidth( 170 ).setMinHeight( 30 );
			table.setFillParent( true ).center();

			// add a button for each of the actions/AI types we need
			table.add( new TextButton( "BT: LowerPriority Abort Tree", skin ) )
				 .getElement<TextButton>()
				 .onClicked += onClickBtLowerPriority;
			table.row();

			table.add( new TextButton( "BT: Self Abort Tree", skin ) )
				 .getElement<TextButton>()
				 .onClicked += onClickBtSelfAbort;
			table.row();

			table.add( new TextButton( "Utility AI", skin ) )
				 .getElement<TextButton>()
				 .onClicked += onClickUtilityAI;
			table.row();

			table.add( new TextButton( "GOAP", skin ) )
				 .getElement<TextButton>()
				 .onClicked += onClickGoap;
			table.row().setPadTop( 40 );

			table.add( new TextButton( "Stop All Running AI", skin ) )
				 .getElement<TextButton>()
				 .onClicked += onClickStopAllAi;

			// fetch our different AI Components
			_miner = entity.scene.findComponentOfType<BehaviorTreeMiner>();
			_utilityMiner = entity.scene.findComponentOfType<UtilityMiner>();
			_goapMiner = entity.scene.findComponentOfType<GOAPMiner>();
		}


		#region button click handlers

		void onClickBtLowerPriority( Button button )
		{
			Debug.log( "------ Enabled Behavior Tree LowerPriority Abort ------" );
			disableAllAI();
			_miner.buildLowerPriorityAbortTree();
			_miner.setEnabled( true );
		}


		void onClickBtSelfAbort( Button button )
		{
			Debug.log( "------ Enabled Behavior Tree Self Abort ------" );
			disableAllAI();
			_miner.buildSelfAbortTree();
			_miner.setEnabled( true );
		}


		void onClickUtilityAI( Button button )
		{
			Debug.log( "------ Enabled Utility AI ------" );
			disableAllAI();
			_utilityMiner.setEnabled( true );
		}


		void onClickGoap( Button button )
		{
			Debug.log( "------ Enabled GOAP ------" );
			disableAllAI();
			_goapMiner.setEnabled( true );
		}


		void onClickStopAllAi( Button button )
		{
			disableAllAI();
		}


		void disableAllAI()
		{
			_miner.setEnabled( false );
			_utilityMiner.setEnabled( false );
			_goapMiner.setEnabled( false );			
		}

		#endregion

	}
}

