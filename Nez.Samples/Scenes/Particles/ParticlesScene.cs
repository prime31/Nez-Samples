using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Nez.Sprites;


namespace Nez.Samples
{
	[SampleScene("Particles", 90,
		"Arrow keys to move. Exiting the Camera view will cull the particles.\nQ/W changes the particle system being played")]
	public class ParticlesScene : SampleScene
	{
		public override void Initialize()
		{
			ClearColor = Color.Black;
			SetDesignResolution(1280, 720, SceneResolutionPolicy.None);
			Screen.SetSize(1280, 720);

			// add the ParticleSystemSelector which handles input for the scene and a SimpleMover to move it around with the keyboard
			var particlesEntity = CreateEntity("particles");
			particlesEntity.SetPosition(Screen.Center - new Vector2(0, 200));
			particlesEntity.AddComponent(new ParticleSystemSelector());
			particlesEntity.AddComponent(new SimpleMover());


			// create a couple moons for playing with particle collisions
			var moonTex = Content.Load<Texture2D>("Shared/moon");

			var moonEntity = CreateEntity("moon");
			moonEntity.Position = new Vector2(Screen.Width / 2, Screen.Height / 2 + 100);
			moonEntity.AddComponent(new SpriteRenderer(moonTex));
			moonEntity.AddComponent<CircleCollider>();

			// clone the first moonEntity to create the second
			var moonEntityTwo = moonEntity.Clone(new Vector2(Screen.Width / 2 - 100, Screen.Height / 2 + 100));
			AddEntity(moonEntityTwo);
		}
	}
}