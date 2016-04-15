using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Nez.Sprites;
using Nez.Textures;
using Nez.Tiled;


namespace Nez.Samples
{
	[SampleScene( "Ninja Adventure", "Tiled map with multiple layers and virtual input\nArrows, d-pad or left stick to move, z key or a button to fire a projectile\nFind and kill the giant moon" )]
	public class NinjaAdventureScene : SampleScene
	{
		public NinjaAdventureScene() : base( true, true )
		{}


		public override void initialize()
		{
			base.initialize();

			// setup a pixel perfect screen that fits our map
			setDesignResolution( 512, 256, Scene.SceneResolutionPolicy.ShowAllPixelPerfect );
			Screen.setSize( 512 * 3, 256 * 3 );


			// load the TiledMap and display it with a TiledMapComponent
			var tiledEntity = createEntity( "tiled-map-entity" );
			var tiledmap = contentManager.Load<TiledMap>( "NinjaAdventure/map/tilemap" );
			var tiledMapComponent = tiledEntity.addComponent( new TiledMapComponent( tiledmap, "collision" ) );
			tiledMapComponent.setLayersToRender( new string[] { "tiles", "terrain", "details" } );
			// render below/behind everything else. our player is at 0 and projectile is at 1.
			tiledMapComponent.renderLayer = 10;

			// render our above-details layer after the player so the player is occluded by it when walking behind things
			var tiledMapDetailsComp = tiledEntity.addComponent( new TiledMapComponent( tiledmap ) );
			tiledMapDetailsComp.setLayerToRender( "above-details" );
			tiledMapDetailsComp.renderLayer = -1;
			// the details layer will write to the stencil buffer so we can draw a shadow when the player is behind it. we need an AlphaTestEffect
			// here as well
			tiledMapDetailsComp.material = Material.stencilWrite();
			tiledMapDetailsComp.material.effect = contentManager.loadNezEffect<SpriteAlphaTestEffect>();

			// setup our camera bounds with a 1 tile border around the edges (for the outside collision tiles)
			tiledEntity.addComponent( new CameraBounds( new Vector2( tiledmap.tileWidth, tiledmap.tileWidth ), new Vector2( tiledmap.tileWidth * ( tiledmap.width - 1 ), tiledmap.tileWidth * ( tiledmap.height - 1 ) ) ) );


			var playerEntity = createEntity( "player", new Vector2( 256 / 2, 224 / 2 ) );
			playerEntity.addComponent( new Ninja() );
			var collider = playerEntity.colliders.add( new CircleCollider() );
			// we only want to collider with the tilemap, which is on the default layer 0
			Flags.setFlagExclusive( ref collider.collidesWithLayers, 0 );
			// move ourself to layer 1 so that we dont get hit by the projectiles that we fire
			Flags.setFlagExclusive( ref collider.physicsLayer, 1 );

			// add a component to have the Camera follow the player
			camera.entity.addComponent( new FollowCamera( playerEntity ) );

			// stick something to shoot in the level
			var moonTexture = contentManager.Load<Texture2D>( "Shared/moon" );
			var moonEntity = createEntity( "moon", new Vector2( 412, 460 ) );
			moonEntity.addComponent( new Sprite( moonTexture ) );
			moonEntity.addComponent( new ProjectileHitDetector() );
			moonEntity.colliders.add( new CircleCollider() );
		}


		/// <summary>
		/// creates a projectile and sets it in motion
		/// </summary>
		/// <returns>The projectile.</returns>
		/// <param name="position">Position.</param>
		/// <param name="velocity">Velocity.</param>
		public Entity createProjectiles( Vector2 position, Vector2 velocity )
		{
			// create an Entity to house the projectile and its logic
			var entity = createEntity( "projectile" );
			entity.transform.position = position;
			entity.addComponent( new ProjectileMover() );
			entity.addComponent( new FireballProjectileController( velocity ) );

			// add a collider so we can detect intersections
			var collider = entity.colliders.add( new CircleCollider() );
			Flags.setFlagExclusive( ref collider.collidesWithLayers, 0 );
			Flags.setFlagExclusive( ref collider.physicsLayer, 1 );


			// load up a Texture that contains a fireball animation and setup the animation frames
			var texture = contentManager.Load<Texture2D>( "NinjaAdventure/plume" );
			var subtextures = Subtexture.subtexturesFromAtlas( texture, 16, 16 );

			var spriteAnimation = new SpriteAnimation( subtextures ) {
				loop = true,
				fps = 10
			};

			// add the Sprite to the Entity and play the animation after creating it
			var sprite = entity.addComponent( new Sprite<int>( subtextures[0] ) );
			// render after (under) our player who is on renderLayer 0, the default
			sprite.renderLayer = 1;
			sprite.addAnimation( 0, spriteAnimation );
			sprite.play( 0 );


			// clone the projectile and fire it off in the opposite direction
			var newEntity = entity.clone( entity.transform.position );
			newEntity.getComponent<FireballProjectileController>().velocity *= -1;
			addEntity( newEntity );


			return entity;
		}
	}
}

