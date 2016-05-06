using System;


namespace Nez.Samples
{
	/// <summary>
	/// component that displays a simple UI to enable/disable the different AI types
	/// </summary>
	public class AIUI : RenderableComponent
	{
		public override RectangleF bounds { get { return new RectangleF( 0, 0, 200, 200 ); } }

		BehaviorTreeMiner _miner;
		UtilityMiner _utilityMiner;
		GOAPMiner _goapMiner;


		public override void onAddedToEntity()
		{
			// prep IMGUI for use
			IMGUI.init( Graphics.instance.bitmapFont );

			_miner = entity.scene.findObjectOfType<BehaviorTreeMiner>();
			_utilityMiner = entity.scene.findObjectOfType<UtilityMiner>();
			_goapMiner = entity.scene.findObjectOfType<GOAPMiner>();
		}


		public override void render( Graphics graphics, Camera camera )
		{
			IMGUI.beginWindow( ( Screen.width / 2 ) - 100, ( Screen.height / 2 ) - 90, 200, 180 );

			if( IMGUI.button( "BT: LowerPriority Abort Tree" ) )
			{
				Debug.log( "------ Enabled Behavior Tree LowerPriority Abort ------" );
				disableAllAI();
				_miner.buildLowerPriorityAbortTree();
				_miner.setEnabled( true );
			}

			if( IMGUI.button( "BT: Self Abort Tree" ) )
			{
				Debug.log( "------ Enabled Behavior Tree Self Abort ------" );
				disableAllAI();
				_miner.buildSelfAbortTree();
				_miner.setEnabled( true );
			}

			if( IMGUI.button( "Utility AI" ) )
			{
				Debug.log( "------ Enabled Utility AI ------" );
				disableAllAI();
				_utilityMiner.setEnabled( true );
			}

			if( IMGUI.button( "GOAP" ) )
			{
				Debug.log( "------ Enabled GOAP ------" );
				disableAllAI();
				_goapMiner.setEnabled( true );
			}

			IMGUI.space( 20 );
			if( IMGUI.button( "Stop AI" ) )
				disableAllAI();

			IMGUI.endWindow();
		}


		void disableAllAI()
		{
			_miner.setEnabled( false );
			_utilityMiner.setEnabled( false );
			_goapMiner.setEnabled( false );			
		}
	}
}

