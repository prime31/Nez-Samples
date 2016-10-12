using Microsoft.Xna.Framework;
using Nez.Tiled;


namespace Nez.Samples
{
	[SampleScene( "Platformer", 120, "Work in progress...\nArrows, d-pad or left stick to move, z key or a button to jump" )]
	public class PlatformerScene : SampleScene
	{
		public PlatformerScene() : base( true, true )
		{ }


		public override void initialize()
		{
			// setup a pixel perfect screen that fits our map
			setDesignResolution( 640, 480, Scene.SceneResolutionPolicy.ShowAllPixelPerfect );
			Screen.setSize( 640 * 2, 480 * 2 );


			var tiledEntity = createEntity( "tiled-map-entity" );
			var tiledMap = content.Load<TiledMap>( Content.Platformer.tiledMap );
			var objectLayer = tiledMap.getObjectGroup( "objects" );
			var spawnObject = objectLayer.objectWithName( "spawn" );
			var tiledMapComponent = tiledEntity.addComponent( new TiledMapComponent( tiledMap, "main" ) );


			var playerEntity = createEntity( "player", new Vector2( spawnObject.x, spawnObject.y ) );
			playerEntity.addComponent( new Caveman() );
			playerEntity.addCollider( new BoxCollider( -8, -16, 16, 32 ) );
			playerEntity.addComponent( new TiledMapMover( tiledMapComponent.collisionLayer ) );

			addPostProcessor( new VignettePostProcessor( 1 ) );
		}
	}
}

