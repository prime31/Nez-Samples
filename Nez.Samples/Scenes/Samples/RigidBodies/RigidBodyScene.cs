using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez.Sprites;


namespace Nez.Samples
{
	[SampleScene("Rigid Bodies", 100,
		"ArcadeRigidBodies can be used for a game-like physics effect\nThis demo just applies some impulses and lets gravity do the rest")]
	public class RigidBodyScene : SampleScene
	{
		public override void Initialize()
		{
			base.Initialize();

			Screen.SetSize(1280, 720);

			var moonTexture = Content.Load<Texture2D>(Nez.Content.Shared.Moon);

			var friction = 0.3f;
			var elasticity = 0.4f;
			CreateEntity(new Vector2(50, 200), 50f, friction, elasticity, new Vector2(150, 0), moonTexture)
				.AddImpulse(new Vector2(10, 0));
			CreateEntity(new Vector2(800, 260), 5f, friction, elasticity, new Vector2(-180, 0), moonTexture);

			CreateEntity(new Vector2(50, 400), 50f, friction, elasticity, new Vector2(150, -40), moonTexture);
			CreateEntity(new Vector2(800, 460), 5f, friction, elasticity, new Vector2(-180, -40), moonTexture);


			CreateEntity(new Vector2(400, 0), 60f, friction, elasticity, new Vector2(10, 90), moonTexture);
			CreateEntity(new Vector2(500, 400), 4f, friction, elasticity, new Vector2(0, -270), moonTexture);


			var rb = CreateEntity(new Vector2(Screen.Width / 2, Screen.Height / 2 + 250), 0, friction, elasticity,
				new Vector2(0, -270), moonTexture);
			rb.Entity.GetComponent<SpriteRenderer>().Color = Color.DarkMagenta;

			rb = CreateEntity(new Vector2(Screen.Width / 2 - 200, Screen.Height / 2 + 250), 0, friction, elasticity,
				new Vector2(0, -270), moonTexture);
			rb.Entity.GetComponent<SpriteRenderer>().Color = Color.DarkMagenta;


			// bottom fellas
			CreateEntity(new Vector2(200, 700), 15f, friction, elasticity, new Vector2(150, -150), moonTexture);
			CreateEntity(new Vector2(800, 760), 15f, friction, elasticity, new Vector2(-180, -150), moonTexture);
			CreateEntity(new Vector2(1200, 700), 1f, friction, elasticity, new Vector2(0, 0), moonTexture)
				.AddImpulse(new Vector2(-5, -20));

			// top fellas
			CreateEntity(new Vector2(100, 100), 1f, friction, elasticity, new Vector2(100, 90), moonTexture)
				.AddImpulse(new Vector2(40, -10));
			CreateEntity(new Vector2(100, 700), 100f, friction, elasticity, new Vector2(200, -270), moonTexture);
		}


		ArcadeRigidbody CreateEntity(Vector2 position, float mass, float friction, float elasticity, Vector2 velocity,
		                             Texture2D texture)
		{
			var rigidbody = new ArcadeRigidbody()
				.SetMass(mass)
				.SetFriction(friction)
				.SetElasticity(elasticity)
				.SetVelocity(velocity);

			var entity = CreateEntity(Utils.RandomString(3));
			entity.Position = position;
			entity.AddComponent(new SpriteRenderer(texture));
			entity.AddComponent(rigidbody);
			entity.AddComponent<CircleCollider>();

			return rigidbody;
		}
	}
}