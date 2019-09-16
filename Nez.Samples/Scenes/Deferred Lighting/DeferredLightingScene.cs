using Nez.DeferredLighting;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez.Sprites;


namespace Nez.Samples
{
	[SampleScene("Deferred Lighting", 80,
		"Press the number keys to change the light that is currently being controlled\nPressing f toggles the rendering of the individual buffers used by the deferred lighting system")]
	public class DeferredLightingScene : SampleScene
	{
		const int RenderablesLayer = 5;
		const int LightLayer = 10;


		public DeferredLightingScene() : base(false, false)
		{
		}


		public override void Initialize()
		{
			base.Initialize();

			// setup screen that fits our map based on the bg size
			SetDesignResolution(137 * 9, 89 * 9, SceneResolutionPolicy.ShowAllPixelPerfect);
			Screen.SetSize(137 * 9, 89 * 9);
			ClearColor = Color.DarkGray;

			// add our renderer setting the renderLayers we will use for lights and for renderables
			var deferredRenderer = AddRenderer(new DeferredLightingRenderer(0, LightLayer, RenderablesLayer))
				.SetClearColor(Color.DarkGray);
			deferredRenderer.EnableDebugBufferRender = false;

			// prep our textures. we have diffuse and normal maps to interact with the lights.
			var moonTex = Content.Load<Texture2D>(Nez.Content.DeferredLighting.Moon);
			var moonNorm = Content.Load<Texture2D>(Nez.Content.DeferredLighting.MoonNorm);
			var orangeTexture = Content.Load<Texture2D>(Nez.Content.DeferredLighting.Orange);
			var orangeNormalMap = Content.Load<Texture2D>(Nez.Content.DeferredLighting.OrangeNorm);
			var bgTexture = Content.Load<Texture2D>(Nez.Content.DeferredLighting.Bg);
			var bgNormalMap = Content.Load<Texture2D>(Nez.Content.DeferredLighting.BgNorm);

			// prep our Materials. Deferred lighting requires a Material that is normal map aware. We can also leave our Material null and
			// a default Material will be used that is diffuse lighting only (no normal map).
			var moonMaterial = new DeferredSpriteMaterial(moonNorm);
			var orangeMaterial = new DeferredSpriteMaterial(orangeNormalMap);
			var bgMaterial = new DeferredSpriteMaterial(bgNormalMap);

			// create some Entities. When we add the Renderable (Sprite in this case) we need to be sure to set the renderLayer and Material
			var bgEntity = CreateEntity("bg");
			bgEntity.SetPosition(Screen.Center).SetScale(9);
			bgEntity.AddComponent(new SpriteRenderer(bgTexture)).SetRenderLayer(RenderablesLayer).SetMaterial(bgMaterial)
				.SetLayerDepth(1);
			bgEntity.AddComponent(new DeferredLightingController());

			var orangeEntity = CreateEntity("orange");
			orangeEntity.SetPosition(Screen.Center).SetScale(0.5f);
			orangeEntity.AddComponent(new SpriteRenderer(orangeTexture)).SetRenderLayer(RenderablesLayer)
				.SetMaterial(orangeMaterial);
			orangeEntity.AddComponent(new SpotLight()).SetRenderLayer(LightLayer);

			var moonEntity = CreateEntity("moon");
			moonEntity.SetPosition(new Vector2(100, 400));
			moonEntity.AddComponent(new SpriteRenderer(moonTex)).SetRenderLayer(RenderablesLayer).SetMaterial(moonMaterial);
			moonEntity.AddComponent(new DirLight(Color.Red)).SetRenderLayer(LightLayer).SetEnabled(true);

			var clone = orangeEntity.Clone(new Vector2(200, 200));
			AddEntity(clone);

			var mouseFollowEntity = CreateEntity("mouse-follow");
			mouseFollowEntity.AddComponent(new MouseFollow());
			mouseFollowEntity.AddComponent(new PointLight(new Color(0.8f, 0.8f, 0.9f))).SetRadius(200).SetIntensity(2)
				.SetRenderLayer(LightLayer);
		}
	}
}