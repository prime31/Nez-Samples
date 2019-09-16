using Nez.Sprites;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;


namespace Nez.Samples
{
	[SampleScene("Line Casting Scene", 10000,
		"Scene to test line casting. Move the mouse around and press the left mouse button to move the start/end point. Press the right mouse button or SPACE to run the linecast.")]
	public class LineCastingScene : SampleScene
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
			var coll = new BoxCollider(moonTex.Width, moonTex.Height);
			playerEntity.AddComponent(coll);
			playerEntity.Position = new Vector2(200, 100);

			var lineCaster = CreateEntity("linecaster")
				.AddComponent(new LineCaster());
			lineCaster.Transform.Position = new Vector2(300, 100);
		}
	}
}