using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Nez.Sprites;
using Nez.Textures;


namespace Nez.Samples
{
	[SampleScene("Ninja Adventure", 10,
		"Tiled map with multiple layers, virtual input and stencil shadows\nArrows, d-pad or left stick to move, z key or controller a button to fire a projectile\nFind and kill the giant moon")]
	public class NinjaAdventureScene : SampleScene
	{
		public NinjaAdventureScene() : base(true, true)
		{
		}


		public override void Initialize()
		{
			base.Initialize();

			// setup a pixel perfect screen that fits our map
			SetDesignResolution(512, 256, SceneResolutionPolicy.ShowAllPixelPerfect);
			Screen.SetSize(512 * 3, 256 * 3);


			// load the TiledMap and display it with a TiledMapComponent
			var tiledEntity = CreateEntity("tiled-map-entity");
			var map = Content.LoadTiledMap("Content/NinjaAdventure/map/tilemap.tmx");
			var tiledMapRenderer = tiledEntity.AddComponent(new TiledMapRenderer(map, "collision"));
			tiledMapRenderer.SetLayersToRender(new[] { "tiles", "terrain", "details" });

			// render below/behind everything else. our player is at 0 and projectile is at 1.
			tiledMapRenderer.RenderLayer = 10;

			// render our above-details layer after the player so the player is occluded by it when walking behind things
			var tiledMapDetailsComp = tiledEntity.AddComponent(new TiledMapRenderer(map));
			tiledMapDetailsComp.SetLayerToRender("above-details");
			tiledMapDetailsComp.RenderLayer = -1;

			// the details layer will write to the stencil buffer so we can draw a shadow when the player is behind it. we need an AlphaTestEffect
			// here as well
			tiledMapDetailsComp.Material = Material.StencilWrite();
			tiledMapDetailsComp.Material.Effect = Content.LoadNezEffect<SpriteAlphaTestEffect>();

			// setup our camera bounds with a 1 tile border around the edges (for the outside collision tiles)
			var topLeft = new Vector2(map.TileWidth, map.TileWidth);
			var bottomRight = new Vector2(map.TileWidth * (map.Width - 1),
				map.TileWidth * (map.Height - 1));
			tiledEntity.AddComponent(new CameraBounds(topLeft, bottomRight));


			var playerEntity = CreateEntity("player", new Vector2(256 / 2, 224 / 2));
			playerEntity.AddComponent(new Ninja());
			var collider = playerEntity.AddComponent<CircleCollider>();

			// we only want to collide with the tilemap, which is on the default layer 0
			Flags.SetFlagExclusive(ref collider.CollidesWithLayers, 0);

			// move ourself to layer 1 so that we dont get hit by the projectiles that we fire
			Flags.SetFlagExclusive(ref collider.PhysicsLayer, 1);

			// add a component to have the Camera follow the player
			Camera.Entity.AddComponent(new FollowCamera(playerEntity));

			// stick something to shoot in the level
			var moonTexture = Content.Load<Texture2D>(Nez.Content.Shared.Moon);
			var moonEntity = CreateEntity("moon", new Vector2(412, 460));
			moonEntity.AddComponent(new SpriteRenderer(moonTexture));
			moonEntity.AddComponent(new ProjectileHitDetector());
			moonEntity.AddComponent<CircleCollider>();
		}


		/// <summary>
		/// creates a projectile and sets it in motion
		/// </summary>
		public Entity CreateProjectiles(Vector2 position, Vector2 velocity)
		{
			// create an Entity to house the projectile and its logic
			var entity = CreateEntity("projectile");
			entity.Position = position;
			entity.AddComponent(new ProjectileMover());
			entity.AddComponent(new FireballProjectileController(velocity));

			// add a collider so we can detect intersections
			var collider = entity.AddComponent<CircleCollider>();
			Flags.SetFlagExclusive(ref collider.CollidesWithLayers, 0);
			Flags.SetFlagExclusive(ref collider.PhysicsLayer, 1);


			// load up a Texture that contains a fireball animation and setup the animation frames
			var texture = Content.Load<Texture2D>(Nez.Content.NinjaAdventure.Plume);
			var sprites = Sprite.SpritesFromAtlas(texture, 16, 16);

			// add the Sprite to the Entity and play the animation after creating it
			var animator = entity.AddComponent(new SpriteAnimator());

			// render after (under) our player who is on renderLayer 0, the default
			animator.RenderLayer = 1;

			animator.AddAnimation("default", sprites.ToArray());
			animator.Play("default");


			// clone the projectile and fire it off in the opposite direction
			var newEntity = entity.Clone(entity.Position);
			newEntity.GetComponent<FireballProjectileController>().Velocity *= -1;
			AddEntity(newEntity);

			return entity;
		}
	}
}