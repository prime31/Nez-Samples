using System;
using Nez.Tiled;


namespace Nez.Samples
{
	/// <summary>
	/// Tiled map import that includes animated tiles from multiple different tileset images
	/// </summary>
	[SampleScene("Animated Tiles", 70, "Tiled map import with animated tiles")]
	public class AnimatedTilesScene : SampleScene
	{
		public AnimatedTilesScene() : base(true, true)
		{
		}


		public override void Initialize()
		{
			base.Initialize();

			// setup a pixel perfect screen that fits our map
			SetDesignResolution(256, 224, Scene.SceneResolutionPolicy.ShowAllPixelPerfect);
			Screen.SetSize(256 * 4, 224 * 4);

			// load the TiledMap and display it with a TiledMapComponent
			var tiledEntity = CreateEntity("tiled-map-entity");
			var tiledmap = Content.Load<TiledMap>(Nez.Content.AnimatedTiles.desertpalace);
			tiledEntity.AddComponent(new TiledMapComponent(tiledmap));
		}
	}
}