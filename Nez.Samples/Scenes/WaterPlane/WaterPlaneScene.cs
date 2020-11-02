using Nez.Sprites;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Nez.Samples
{
	/// <summary>
	/// Tiled map import that includes animated tiles from multiple different tileset images
	/// </summary>
	[SampleScene("Water Reflection Effect", 70, "Water Reflection Effect")]
	public class WaterPlaneScene : SampleScene
	{
		const int _waterPaneRenderLayer = -50;

		public WaterPlaneScene() : base(false, true)
		{
			AddRenderer(new RenderLayerExcludeRenderer(0, new[] { _waterPaneRenderLayer, ScreenSpaceRenderLayer }));

			var waterrenderer = new RenderLayerRenderer(-1, _waterPaneRenderLayer);
			waterrenderer.WantsToRenderAfterPostProcessors = true;
			AddRenderer(waterrenderer);

		}


		public override void Initialize()
		{
			AddRenderer(new RenderLayerExcludeRenderer(0, new[] { _waterPaneRenderLayer, ScreenSpaceRenderLayer }));

			var waterrenderer = new RenderLayerRenderer(-1, _waterPaneRenderLayer);
			waterrenderer.WantsToRenderAfterPostProcessors = true;
			AddRenderer(waterrenderer);
		}

		public override void OnStart()
		{

			var moonTex = Content.Load<Texture2D>(Nez.Content.Shared.Moon);
			var playerEntity = CreateEntity("player", new Vector2(Screen.Width / 2, Screen.Height * .7f));
			playerEntity.AddComponent(new SpriteRenderer(moonTex));

			var reflectiveEntity = Core.Scene.CreateEntity("Reflection");
			var reflectionPlane = new WaterReflectionPlane(Screen.Width, Screen.Height / 5) { RenderLayer = 3 };
			var material = reflectionPlane.GetMaterial<WaterReflectionMaterial>();
			material.Effect.NormalMap = Core.Scene.Content.LoadTexture("Content/WaterPane/waterNormalMap.png");
			material.Effect.PerspectiveCorrectionIntensity = .01f;
			material.Effect.NormalMagnitude = 0.01f;
			reflectiveEntity.AddComponent(reflectionPlane);
			reflectiveEntity.Position = new Vector2(0, Screen.Height - Screen.Height / 5);
		}
	}
}