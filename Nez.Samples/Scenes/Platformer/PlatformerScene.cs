using System;
using Nez.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez.Tiled;


namespace Nez.Samples
{
	[SampleScene( "Platformer", "Tiled map" )]
	public class PlatformerScene : SampleScene
	{
		public PlatformerScene() : base( true, true )
		{}


		public override void initialize()
		{
			base.initialize();

			// setup a pixel perfect screen that fits our map
			setDesignResolution( 512, 256, Scene.SceneResolutionPolicy.ShowAllPixelPerfect );
			Screen.setSize( 512 * 3, 256 * 3 );


			var tiledEntity = createEntity( "tiled-map-entity" );
			var tiledmap = contentManager.Load<TiledMap>( "Platformer/map" );
			tiledEntity.addComponent( new TiledMapComponent( tiledmap, "main" ) );


			var playerEntity = createEntity( "player", new Vector2( Screen.width / 2, Screen.height / 2 ) );
			playerEntity.addComponent( new Caveman() );
			playerEntity.transform.position = new Vector2( 150, 150 );
			playerEntity.addComponent( new FollowCamera( playerEntity ) );
			playerEntity.addCollider( new BoxCollider() );


			addPostProcessor( new VignettePostProcessor( 1 ) );
		}
	}
}

