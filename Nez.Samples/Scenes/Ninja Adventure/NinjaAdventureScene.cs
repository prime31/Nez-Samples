using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Nez.Sprites;
using Nez.Textures;
using Nez.Tiled;


namespace Nez.Samples
{
	[SampleScene( "Ninja Adventure", "Tiled map with multiple layers\nArrows to move" )]
	public class NinjaAdventureScene : SampleScene
	{
		public NinjaAdventureScene() : base( true, true )
		{}


		public override void initialize()
		{
			base.initialize();

			// setup a pixel perfect screen that fits our map
			setDesignResolution( 256, 256, Scene.SceneResolutionPolicy.ShowAllPixelPerfect );
			Screen.setSize( 256 * 4, 256 * 4 );


			// load the TiledMap and display it with a TiledMapComponent
			var tiledEntity = createEntity( "tiled-map-entity" );
			var tiledmap = contentManager.Load<TiledMap>( "NinjaAdventure/map/tilemap" );
			tiledEntity.addComponent( new TiledMapComponent( tiledmap, "collision" ) );

			// setup our camera bounds with a 1 tile border around the edges (for the outside collision tiles)
			tiledEntity.addComponent( new CameraBounds( new Vector2( tiledmap.tileWidth, tiledmap.tileWidth ), new Vector2( tiledmap.tileWidth * ( tiledmap.width - 1 ), tiledmap.tileWidth * ( tiledmap.height - 1 ) ) ) );

			var playerEntity = createEntity( "player", new Vector2( 256 / 2, 224 / 2 ) );
			playerEntity.addComponent( new Ninja() );
			playerEntity.addComponent( new FollowCamera( playerEntity ) );
		}
	}
}

