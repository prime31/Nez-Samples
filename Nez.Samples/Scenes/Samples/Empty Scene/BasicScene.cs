using Nez.Sprites;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;


namespace Nez.Samples
{
	[SampleScene("Basic Scene", 9999, "Scene with a single Entity. The minimum to have something to show")]
	public class BasicScene : SampleScene
	{
		public override void Initialize()
		{
			base.Initialize();

			// default to 1280x720 with no SceneResolutionPolicy
			SetDesignResolution(1280, 720, SceneResolutionPolicy.None);
			Screen.SetSize(1280, 720);

			var moonTex = Content.Load<Texture2D>(Nez.Content.Shared.Moon);
			var playerEntity = CreateEntity("player", new Vector2(Screen.Width / 2, Screen.Height / 2));
			playerEntity.AddComponent(new SpriteRenderer(moonTex));
		}
	}
}