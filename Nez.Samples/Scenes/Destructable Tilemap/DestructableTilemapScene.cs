using System;
using Microsoft.Xna.Framework;
using Nez.Tiled;
using Microsoft.Xna.Framework.Graphics;
using Nez.Textures;
using Nez.Sprites;


namespace Nez.Samples
{
	[SampleScene( "Destructable Tilemap", "Demonstrates more advanced Tiled map usage including custom object layers,\nfetching attributes and managing colliders\nArrow keys to move" )]
	public class DestructableTilemapScene : SampleScene
	{
		public DestructableTilemapScene() : base( true, true )
		{}


		public override void initialize()
		{
			clearColor = Color.Black;
			setDesignResolution( 640, 368, Scene.SceneResolutionPolicy.ShowAllPixelPerfect );
			Screen.setSize( 1280, 736 );

			// load a TiledMap and move it back so is drawn before other entities
			var tiledEntity = createEntity( "tiled-map" );
			var tiledmap = contentManager.Load<TiledMap>( Content.DestructableMap.destructablemap );
			tiledEntity.addComponent( new TiledMapComponent( tiledmap, "main" ) );

			var objects = tiledmap.getObjectGroup( "objects" );
			var spawn = objects.objectWithName( "spawn" );
			var ball = objects.objectWithName( "ball" );

			var atlas = contentManager.Load<Texture2D>( Content.DestructableMap.desertpalacetiles2x );
			var atlasParts = Subtexture.subtexturesFromAtlas( atlas, 16, 16 );
			var playerSubtexture = atlasParts[96];

			var playerEntity = createEntity( "player" );
			playerEntity.transform.position = new Vector2( spawn.x + 8, spawn.y + 8 );
			playerEntity.addComponent( new Sprite( playerSubtexture ) );
			playerEntity.addComponent( new PlayerDashMover() );
			playerEntity.addComponent( new CameraShake() );
			playerEntity.addComponent( new Nez.Shadows.PointLight( 100 )
			{
				collidesWithLayers = 1 << 0,
				color = Color.Yellow * 0.5f
			});

			var trail = playerEntity.addComponent( new SpriteTrail( playerEntity.getComponent<Sprite>() ) );
			trail.fadeDelay = 0;
			trail.fadeDuration = 0.2f;
			trail.minDistanceBetweenInstances = 10f;
			trail.initialColor = Color.White * 0.5f;

			// add a collider and put it on layer 2 but make it only collide with layer 0. This will make the player only collider with the tilemap
			var collider = playerEntity.colliders.add( new BoxCollider() );
			Flags.setFlagExclusive( ref collider.physicsLayer, 2 );
			Flags.setFlagExclusive( ref collider.collidesWithLayers, 0 );


			// create an object at the location set on our Tiled object layer that only collides with tiles and not the player
			var ballSubtexture = atlasParts[96];
			var ballEntity = createEntity( "ball" );
			ballEntity.transform.position = new Vector2( ball.x + 8, ball.y + 8 );
			ballEntity.addComponent( new Sprite( ballSubtexture ) );
			ballEntity.addComponent( new ArcadeRigidbody() );

			// add a collider and put it on layer 1. Make it only collider with layer 0 (the tilemap) to it doesnt interact with the player.
			var circleCollider = ballEntity.colliders.add( new CircleCollider() );
			Flags.setFlagExclusive( ref circleCollider.physicsLayer, 1 );
			Flags.setFlagExclusive( ref circleCollider.collidesWithLayers, 0 );
		}
	}
}

