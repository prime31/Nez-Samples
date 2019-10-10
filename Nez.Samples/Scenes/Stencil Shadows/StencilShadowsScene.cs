using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez.PhysicsShapes;
using Nez.Sprites;
using Nez.Textures;

namespace Nez.Samples
{
	[SampleScene("Stencil Shadows", 41, "2D Stencil shadow system with a mouse controller light\nHold a/d to cycle color\nHold w/s to change radius\nClick to clone the light")]
	public class StencilShadowsScene : SampleScene
	{
		const int LIGHT_LAYER = 5;
		const int LIGHT_MAP_LAYER = 10;
		const int BG_LAYER = 15;

		public StencilShadowsScene() : base(false)
		{ }

		public override void Initialize()
		{
			base.Initialize();

			SetDesignResolution(1280, 720, SceneResolutionPolicy.ShowAll);
			Screen.SetSize(1280, 720);

			// render layer for all lights
			ClearColor = Color.White;

			// create the StencilLightRenderer that renders only the light layer into a render target. Remember, when we render into a RenderTexture
			// we need to render before other renderers so we set our renderOrder to -1
			var lightRenderer = AddRenderer(new StencilLightRenderer(-1, LIGHT_LAYER, new RenderTexture()));

			// the clear color acts as ambient light. We set it to a dark color to make the lights stand out a bit.
			lightRenderer.RenderTargetClearColor = new Color(20, 20, 20, 255);

			// renderer for the background image and the light map (created by StencilLightRenderer)
			AddRenderer(new RenderLayerRenderer(0, BG_LAYER));
			AddRenderer(new RenderLayerRenderer(1, LIGHT_MAP_LAYER));

			// lastly, we render all the obstacles AFTER the light map. This keeps them above it so they are always visible.
			AddRenderer(new RenderLayerRenderer(2, 0));

			CreateLights();
			CreateBoxes();
			CreateObstacles();

			// create the background texture, settig it to the correct RenderLayer
			var bgTexture = Content.Load<Texture2D>(Nez.Content.SpriteLights.Bg);
			CreateEntity("bg")
				.SetPosition(Screen.Center)
				.SetScale(9.4f)
				.AddComponent(new SpriteRenderer(bgTexture))
				.SetRenderLayer(BG_LAYER);

			// the light-map will render the lightmap that our StencilLightRenderer creates for us with multiplicative blending
			CreateEntity("light-map")
				.SetPosition(Screen.Center)
				.AddComponent(new SpriteRenderer(lightRenderer.RenderTexture))
				.SetMaterial(Material.BlendMultiply())
				.SetRenderLayer(LIGHT_MAP_LAYER);
		}

		/// <summary>
		/// creates some StencilLights and a SpriteRenderer light
		/// </summary>
		void CreateLights()
		{
			var lightTex = Content.Load<Texture2D>(Nez.Content.SpriteLights.Spritelight);
			CreateEntity("texture-light")
				.SetPosition(Screen.Center + new Vector2(200, 200))
				.SetScale(8)
				.AddComponent(new SpriteRenderer(lightTex))
				.SetRenderLayer(LIGHT_LAYER)
				.SetColor(Color.GreenYellow);

			CreateEntity("light2")
				.SetPosition(Screen.Center + new Vector2(200, -200))
				.AddComponent(new StencilLight(400, Color.AntiqueWhite))
				.SetRenderLayer(LIGHT_LAYER);

			CreateEntity("light3")
				.SetPosition(300, 600)
				.AddComponent(new StencilLight(400, new Color(200, 20, 20)))
				.SetRenderLayer(LIGHT_LAYER)
				.AddComponent<LiveLightController>();
		}

		/// <summary>
		/// creates some textures boxes
		/// </summary>
		void CreateBoxes()
		{
			var blockTexture = Content.Load<Texture2D>(Nez.Content.Shadows.Block);
			CreateEntity("block1")
				.SetPosition(Screen.Center)
				.AddComponent(new SpriteRenderer(blockTexture))
				.AddComponent<BoxCollider>();

			CreateEntity("block2")
				.SetPosition(Screen.Center - new Vector2(200, 200))
				.AddComponent(new SpriteRenderer(blockTexture))
				.AddComponent<BoxCollider>();
		}

		/// <summary>
		/// create some polygons and a circle so the lights have something interesting to interact with
		/// </summary>
		void CreateObstacles()
		{
			var trianglePoints = new Vector2[] { new Vector2(0, 0), new Vector2(100, -100), new Vector2(-100, -150) };
			CreateEntity("triangle")
				.SetPosition(100, 300)
				.AddComponent(new PolygonMesh(trianglePoints, false).SetColor(Color.LightGreen))
				.AddComponent(new PolygonCollider(trianglePoints));

			CreateEntity("circle")
				.SetPosition(1000, 250)
				.AddComponent(new SpriteRenderer(Content.Load<Texture2D>(Nez.Content.Shared.Moon)))
				.SetColor(Color.LightGreen)
				.AddComponent(new CircleCollider(64));

			var polyPoints = Polygon.BuildSymmetricalPolygon(5, 140);
			var polygonEntity = CreateEntity("polygon");
			polygonEntity.SetPosition(460, 450)
				.AddComponent(new PolygonMesh(polyPoints)).SetTexture(Content.Load<Texture2D>(Nez.Content.Shared.Moon))
				.AddComponent(new PolygonCollider(polyPoints));

			polygonEntity.TweenRotationDegreesTo(180, 3f)
				.SetLoops(Tweens.LoopType.PingPong, 50)
				.SetEaseType(Tweens.EaseType.Linear)
				.Start();
		}
	}
}
