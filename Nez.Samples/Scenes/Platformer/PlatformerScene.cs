using Microsoft.Xna.Framework;
using Nez.Tiled;


namespace Nez.Samples
{
	[SampleScene( "Platformer", 120, "Work in progress...\nArrows, d-pad or left stick to move, z key or a button to jump" )]
	public class PlatformerScene : SampleScene
	{
		public PlatformerScene() : base( true, true )
		{ }


		public override void Initialize()
		{
			// setup a pixel perfect screen that fits our map
			SetDesignResolution( 640, 480, Scene.SceneResolutionPolicy.ShowAllPixelPerfect );
			Screen.SetSize( 640 * 2, 480 * 2 );


			// load up our TiledMap
			var tiledMap = Content.Load<TiledMap>( Nez.Content.Platformer.tiledMap );
			var objectLayer = tiledMap.GetObjectGroup( "objects" );
			var spawnObject = objectLayer.ObjectWithName( "spawn" );
			var tiledEntity = CreateEntity( "tiled-map-entity" );
			var tiledMapComponent = tiledEntity.AddComponent( new TiledMapComponent( tiledMap, "main" ) );


			var playerEntity = CreateEntity( "player", new Vector2( spawnObject.X, spawnObject.Y ) );
			playerEntity.AddComponent( new Caveman() );
			playerEntity.AddComponent( new BoxCollider( -8, -16, 16, 32 ) );
			playerEntity.AddComponent( new TiledMapMover( tiledMapComponent.CollisionLayer ) );

			AddPostProcessor( new VignettePostProcessor( 1 ) );
		}
	}
}

