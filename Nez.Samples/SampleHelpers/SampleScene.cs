using System;
using Microsoft.Xna.Framework;


namespace Nez.Samples
{
	public abstract class SampleScene : Scene
	{
		public const int SCREEN_SPACE_RENDER_LAYER = 999;


		public SampleScene( string instructions ) : base()
		{
			var instructionsEntity = createEntity( "instructions" );
			instructionsEntity.addComponent( new Text( Graphics.instance.bitmapFont, instructions, Vector2.Zero, Color.White ) );
		}


		public override void initialize()
		{
			addRenderer( new ScreenSpaceRenderer( 100, SCREEN_SPACE_RENDER_LAYER ) );
			addRenderer( new RenderLayerExcludeRenderer( 0, SCREEN_SPACE_RENDER_LAYER ) );
		}
	}
}

