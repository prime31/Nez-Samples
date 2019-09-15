using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez.Sprites;


namespace Nez.Samples
{
	/// <summary>
	/// demos the SpringGrid with a SimpleMover entity that applies forces to the grid. vignette and bloom post processors are also
	/// added to give the scene some life.
	/// </summary>
	[SampleScene("Spring Grid", 30,
		"SpringGrid component with vignette and bloom\nArrow keys to move\nSpace to apply an explosive force")]
	public class SpringGridScene : SampleScene
	{
		public override void Initialize()
		{
			base.Initialize();

			Screen.SetSize(1280, 720);
			ClearColor = Color.Black;
			var moonTex = Content.Load<Texture2D>(Nez.Content.Shared.Moon);

			var gridEntity = CreateEntity("grid");
			gridEntity.AddComponent(new SpringGrid(new Rectangle(0, 0, Screen.Width, Screen.Height), new Vector2(30)));


			var playerEntity = CreateEntity("player", new Vector2(Screen.Width / 2, Screen.Height / 2));
			playerEntity.Scale *= 0.5f;
			playerEntity.AddComponent(new SimpleMover());
			playerEntity.AddComponent(new GridModifier());
			playerEntity.AddComponent(new SpriteRenderer(moonTex));


			AddPostProcessor(new VignettePostProcessor(1));
			AddPostProcessor(new BloomPostProcessor(3)).Settings = BloomSettings.PresetSettings[0];
		}
	}
}