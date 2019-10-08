using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez.PhysicsShapes;
using Nez.Sprites;
using Nez.Textures;

namespace Nez.Samples
{
	[SampleScene("Stencil Shadows", 41, "2D Stencil shadow system\nArrow keys to move")]
	public class StencilShadowsScene : SampleScene
	{
		const int LIGHT_LAYER = 5;
		const int SATANS_LAYER = 666;
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

			// create a Renderer that renders all but the light layer and screen space layer
			AddRenderer(new RenderLayerExcludeRenderer(5, LIGHT_LAYER, ScreenSpaceRenderLayer, SATANS_LAYER, BG_LAYER, 1, -1));
			//AddRenderer(new RenderLayerRenderer(0, BG_LAYER));
			AddRenderer(new RenderLayerRenderer(1, SATANS_LAYER));

			// create a Renderer that renders only the light layer into a render target
			var lightRenderer = AddRenderer(new StencilLightRenderer(-1, LIGHT_LAYER));
			lightRenderer.RenderTargetClearColor = new Color(0, 0, 0, 255);
			lightRenderer.RenderTexture = new RenderTexture();

			//AddPostProcessor(new SpriteLightPostProcessor(0, lightRenderer.RenderTexture));


			var bgTexture = Content.Load<Texture2D>(Nez.Content.SpriteLights.Bg);
			CreateEntity("bg")
				.SetPosition(Screen.Center)
				.SetScale(9.4f)
				.AddComponent(new SpriteRenderer(bgTexture))
				.SetRenderLayer(BG_LAYER);

			var blockTexture = Content.Load<Texture2D>(Nez.Content.Shadows.Block);
			CreateEntity("block1")
				.SetPosition(Screen.Center)
				.AddComponent(new SpriteRenderer(blockTexture))
				.AddComponent<BoxCollider>();

			CreateEntity("block2")
				.SetPosition(Screen.Center - new Vector2(200, 200))
				.AddComponent(new SpriteRenderer(blockTexture))
				.AddComponent<BoxCollider>();


			var lightTex = Content.Load<Texture2D>(Nez.Content.SpriteLights.Spritelight);
			CreateEntity("light1")
				.SetPosition(Screen.Center + new Vector2(200, 200))
				.SetScale(12)
				.AddComponent(new SpriteRenderer(lightTex))
				.SetRenderLayer(LIGHT_LAYER)
				.SetColor(Color.GreenYellow)//.SetColor(Color.White)
				.AddComponent<LiveLightController>();

			CreateEntity("light2")
				.SetPosition(Screen.Center + new Vector2(200, -200))
				.SetScale(12)
				.AddComponent(new SpriteRenderer(lightTex))
				.SetColor(Color.Firebrick).SetColor(Color.White)
				.SetRenderLayer(LIGHT_LAYER);


			var effect = Content.LoadEffect<Effect>("spriteLightMultiply", EffectResource.GetFileResourceBytes("Content/nez/effects/SpriteLightMultiply.mgfxo"));
			CreateEntity("poop")
				.SetPosition(Screen.Center)
				.AddComponent(new SpriteRenderer(lightRenderer.RenderTexture))
				.SetRenderLayer(SATANS_LAYER)
				.SetMaterial(new Material(effect)).SetMaterial(Material.BlendDarken());
			effect.Parameters["_lightTexture"].SetValue(lightRenderer.RenderTexture);
			effect.Parameters["_multiplicativeFactor"].SetValue(0.4f);

			CreatePolygons();
		}

		void CreatePolygons()
		{
			var trianglePoints = new Vector2[] { new Vector2(-100, -150), new Vector2(100, -100), new Vector2(0, 0) };
			var triangleEntity = CreateEntity("triangle");
			triangleEntity.SetPosition(100, 300)
				.AddComponent(new PolygonMesh(trianglePoints, false).SetColor(Color.LightGreen))
				.AddComponent(new PolygonCollider(trianglePoints));


			var circleEntity = CreateEntity("circle");
			circleEntity.SetPosition(1000, 250);
			circleEntity.AddComponent(new SpriteRenderer(Content.Load<Texture2D>(Nez.Content.Shared.Moon)))
				.SetColor(Color.LightGreen)
				.AddComponent(new CircleCollider(64));


			var polyPoints = Polygon.BuildSymmetricalPolygon(5, 140);
			var polygonEntity = CreateEntity("polygon");
			polygonEntity.SetPosition(460, 450)
				.AddComponent(new PolygonMesh(polyPoints)).SetColor(Color.LightGreen)
				.AddComponent(new PolygonCollider(polyPoints));

			polygonEntity.TweenRotationDegreesTo(180, 3f)
				.SetLoops(Tweens.LoopType.PingPong, 50)
				.SetEaseType(Tweens.EaseType.Linear)
				.Start();
		}
	}
}
