using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez.PhysicsShapes;
using Nez.Sprites;
using Nez.Verlet;


namespace Nez.Samples
{
	[SampleScene("Verlet Physics", 55,
		"Verlet physics objects interacting with standard Nez Colliders\nClick and drag any particles to interact with them\nPress g to toggle gravity and z to toggle zero gravity")]
	public class VerletPhysicsScene : SampleScene
	{
		public VerletPhysicsScene() : base(true, true)
		{ }

		public override void Initialize()
		{
			ClearColor = Color.Black;
			SetDesignResolution(1280, 720, SceneResolutionPolicy.ShowAll);
			Screen.SetSize(1280, 720);

			// create an Entity and Component to manage the Verlet World and tick its update method
			var verletSystem = CreateEntity("verlet-system")
				.AddComponent<VerletSystem>();

			// first, we'll create some standard Nez Colliders for the Verlet objects to interact with
			CreatePolygons();

			// add a rope, which is just a series of points connected by constraints
			CreateRope(verletSystem.World);

			// add some of the included Composite objects
			verletSystem.World.AddComposite(new Tire(new Vector2(350, 64), 64, 32, 0.3f, 0.5f));
			verletSystem.World.AddComposite(new Tire(new Vector2(600, 32), 50, 4, 0.2f, 0.7f));
			verletSystem.World.AddComposite(new Tire(new Vector2(900, 128), 64, 7, 0.1f, 0.3f));

			verletSystem.World.AddComposite(new Cloth(new Vector2(900, 10), 200, 200, 20, 0.25f, 50));

			verletSystem.World.AddComposite(new Ragdoll(400, 20, Random.Range(140, 240)));
			verletSystem.World.AddComposite(new Ragdoll(500, 20, Random.Range(140, 240)));
			verletSystem.World.AddComposite(new Ragdoll(600, 20, Random.Range(140, 240)));

			verletSystem.World.AddComposite(new Ball(new Vector2(100, 60), Random.Range(10, 50)));
			verletSystem.World.AddComposite(new Ball(new Vector2(150, 60), Random.Range(10, 50)));
			verletSystem.World.AddComposite(new Ball(new Vector2(200, 60), Random.Range(10, 50)));
		}

		void CreatePolygons()
		{
			var trianglePoints = new Vector2[] { new Vector2(0, 0), new Vector2(100, -100), new Vector2(-100, -150) };
			var triangleEntity = CreateEntity("triangle");
			triangleEntity.SetPosition(100, 300);
			triangleEntity.AddComponent(new PolygonMesh(trianglePoints, false).SetColor(Color.LightGreen));
			triangleEntity.AddComponent(new PolygonCollider(trianglePoints));


			var circleEntity = CreateEntity("circle");
			circleEntity.SetPosition(1000, 250);
			circleEntity.AddComponent(new SpriteRenderer(Content.Load<Texture2D>(Nez.Content.Shared.Moon)))
				.SetColor(Color.LightGreen);
			circleEntity.AddComponent(new CircleCollider(64));


			var polyPoints = Polygon.BuildSymmetricalPolygon(5, 140);
			var polygonEntity = CreateEntity("boxCollider");
			polygonEntity.SetPosition(460, 450);
			polygonEntity.AddComponent(new PolygonMesh(polyPoints)).SetColor(Color.LightGreen);
			polygonEntity.AddComponent(new PolygonCollider(polyPoints));

			polygonEntity.TweenRotationDegreesTo(180, 3f)
				.SetLoops(Tweens.LoopType.PingPong, 50)
				.SetEaseType(Tweens.EaseType.Linear)
				.Start();
		}

		void CreateRope(VerletWorld world)
		{
			// create an array of points for our rope
			var linePoints = new Vector2[10];
			for (var i = 0; i < 10; i++)
				linePoints[i] = new Vector2(30 * i + 50, 10);

			var line = new LineSegments(linePoints, 0.3f)
				.PinParticleAtIndex(0);
			world.AddComposite(line);
		}
	}
}