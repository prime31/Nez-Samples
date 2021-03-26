using Nez.DeferredLighting;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez.Sprites;
using Nez.Textures;


namespace Nez.Samples
{
	[SampleScene("Deferred Lighting", 80, "", true)]
	public class DeferredLightingDocumentationScene : SampleScene
	{
		const int RenderablesLayer = 5;
		const int LightLayer = 10;

		public DeferredLightingDocumentationScene() : base(false, false)
		{
		}

        public const int SpriteLightRenderLayer = 1;
        RenderLayerRenderer _lightRenderer;

        public override void Initialize()
        {
            base.Initialize();

            // setup screen that fits our background
            SetDesignResolution(1280, 720, SceneResolutionPolicy.ShowAll);
            Screen.SetSize(1280, 720);
            AddRenderer(new RenderLayerExcludeRenderer(0, ScreenSpaceRenderLayer, SpriteLightRenderLayer));
            _lightRenderer = AddRenderer(new RenderLayerRenderer(-1, SpriteLightRenderLayer));
            _lightRenderer.RenderTexture = new RenderTexture();
            _lightRenderer.RenderTargetClearColor = new Color(10, 10, 10, 255);

            AddPostProcessor(new SpriteLightPostProcessor(0, _lightRenderer.RenderTexture));
            
            var bgTexture = Content.Load<Texture2D>(Nez.Content.DeferredLighting.Bg);
            var bgEntity = CreateEntity("bg");

            bgEntity.Position = Screen.Center;
            bgEntity.Scale = new Vector2(9.4f);
            bgEntity.AddComponent(new SpriteRenderer(bgTexture));

            var lightTexture = Content.Load<Texture2D>(Nez.Content.SpriteLights.Spritelight);
            var lightEntity = CreateEntity("light");
            lightEntity.Position = new Vector2(Screen.Width / 2f, Screen.Height / 2f);
            lightEntity.Scale = new Vector2(4);

            var lightSprite = lightEntity.AddComponent(new SpriteRenderer(lightTexture));
            lightSprite.RenderLayer = SpriteLightRenderLayer;
        }
    }
}