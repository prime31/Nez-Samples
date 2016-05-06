using System;
using Nez.Tiled;


namespace Nez.Samples
{
	/// <summary>
	/// Tiled map import that includes animated tiles from multiple different tileset images
	/// </summary>
	[SampleScene( "Animated Tiles", "Tiled map import with animated tiles" )]
	public class AnimatedTilesScene : SampleScene
	{
		public AnimatedTilesScene() : base( true, true )
		{}


		public override void initialize()
		{
			base.initialize();

			// setup a pixel perfect screen that fits our map
			setDesignResolution( 256, 224, Scene.SceneResolutionPolicy.ShowAllPixelPerfect );
			Screen.setSize( 256 * 4, 224 * 4 );

			// load the TiledMap and display it with a TiledMapComponent
			var tiledEntity = createEntity( "tiled-map-entity" );
			var tiledmap = contentManager.Load<TiledMap>( "AnimatedTiles/desert-palace" );
			tiledEntity.addComponent( new TiledMapComponent( tiledmap ) );
		}
	}
}

