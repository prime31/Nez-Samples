using Nez.Sprites;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;


namespace Nez.Samples
{
	[SampleScene( "Line Casting Scene", 10000, "Scene to test line casting. Move the mouse around and press the left mouse button to move the start/end point. Press the right mouse button or SPACE to run the linecast." )]
	public class LineCastingScene : SampleScene
	{
		public override void initialize()
		{
			base.initialize();

			// default to 1280x720 with no SceneResolutionPolicy
			setDesignResolution( 1280, 720, Scene.SceneResolutionPolicy.None );
			Screen.setSize( 1280, 720 );

			var moonTex = content.Load<Texture2D>( Content.Shared.moon );
			var playerEntity = createEntity( "player", new Vector2( Screen.width / 2, Screen.height / 2 ) );
			playerEntity.addComponent( new Sprite( moonTex ) );
            var coll = new BoxCollider(moonTex.Width, moonTex.Height);
            playerEntity.addComponent(coll);
            playerEntity.transform.position = new Vector2(200, 100);

			var lineCaster = createEntity("linecaster")
                .addComponent(new LineCaster());
            lineCaster.transform.position = new Vector2(300, 100);

		}
	}
}

