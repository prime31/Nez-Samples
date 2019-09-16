using Microsoft.Xna.Framework;
using Nez.Textures;
using Nez.Sprites;
using Nez.Shadows;


namespace Nez.Samples
{
	[SampleScene("Destructable Tilemap", 20,
		"Demonstrates more advanced Tiled map usage including custom object layers,\nfetching attributes and managing colliders\nArrow keys to move")]
	public class DestructableTilemapScene : SampleScene
	{
		public DestructableTilemapScene() : base(true, true)
		{
		}


		public override void Initialize()
		{
			ClearColor = Color.Black;
			SetDesignResolution(640, 368, SceneResolutionPolicy.ShowAllPixelPerfect);
			Screen.SetSize(1280, 736);

			// load a TiledMap and move it back so is drawn before other entities
			var map = Content.LoadTiledMap("Content/DestructableMap/destructable-map.tmx");
			var tiledEntity = CreateEntity("tiled-map");
			tiledEntity.AddComponent(new TiledMapRenderer(map, "main"));

			var objectGroup = map.GetObjectGroup("objects");
			var spawn = objectGroup.Objects["spawn"];
			var ball = objectGroup.Objects["ball"];

			var atlas = Content.LoadTexture("Content/DestructableMap/desert-palace-tiles2x.png");
			var atlasParts = Sprite.SpritesFromAtlas(atlas, 16, 16);
			var playerSubtexture = atlasParts[96];

			var playerEntity = CreateEntity("player");
			playerEntity.Position = new Vector2(spawn.X + 8, spawn.Y + 8);
			playerEntity.AddComponent(new SpriteRenderer(playerSubtexture));
			playerEntity.AddComponent(new PlayerDashMover());
			playerEntity.AddComponent(new CameraShake());
			playerEntity.AddComponent(new PolyLight(100)
			{
				CollidesWithLayers = 1 << 0,
				Color = Color.Yellow * 0.5f
			});

			var trail = playerEntity.AddComponent(new SpriteTrail(playerEntity.GetComponent<SpriteRenderer>()));
			trail.FadeDelay = 0;
			trail.FadeDuration = 0.2f;
			trail.MinDistanceBetweenInstances = 10f;
			trail.InitialColor = Color.White * 0.5f;

			// add a collider and put it on layer 2 but make it only collide with layer 0. This will make the player only collide with the tilemap
			var collider = playerEntity.AddComponent<BoxCollider>();
			Flags.SetFlagExclusive(ref collider.PhysicsLayer, 2);
			Flags.SetFlagExclusive(ref collider.CollidesWithLayers, 0);


			// create an object at the location set on our Tiled object layer that only collides with tiles and not the player
			var ballSubtexture = atlasParts[96];
			var ballEntity = CreateEntity("ball");
			ballEntity.Position = new Vector2(ball.X + 8, ball.Y + 8);
			ballEntity.AddComponent(new SpriteRenderer(ballSubtexture));
			ballEntity.AddComponent(new ArcadeRigidbody());

			// add a collider and put it on layer 1. Make it only collide with layer 0 (the tilemap) so it doesnt interact with the player.
			var circleCollider = ballEntity.AddComponent<CircleCollider>();
			Flags.SetFlagExclusive(ref circleCollider.PhysicsLayer, 1);
			Flags.SetFlagExclusive(ref circleCollider.CollidesWithLayers, 0);
		}
	}
}