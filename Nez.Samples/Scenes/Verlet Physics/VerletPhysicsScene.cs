using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez.PhysicsShapes;
using Nez.Sprites;
using Nez.Verlet;


namespace Nez.Samples
{
	[SampleScene( "Verlet Physics", 55, "Verlet physics objects interacting with standard Nez Colliders\nClick and drag any particles to interact with them\nPress g to toggle gravity and z to toggle zero gravity" )]
	public class VerletPhysicsScene : SampleScene
	{
		public VerletPhysicsScene() : base( true, true )
		{}


		public override void initialize()
		{
			clearColor = Color.Black;
			setDesignResolution( 1280, 720, Scene.SceneResolutionPolicy.ShowAll );
			Screen.setSize( 1280, 720 );

			// create an Entity and Component to manage the Verlet World and tick its update method
			var verletSystem = createEntity( "verlet-system" )
				.addComponent<VerletSystem>();

			// first, we'll create some standard Nez Colliders for the Verlet objects to interact with
			createPolygons();

			// add a rope, which is just a series of points connected by constraints
			createRope( verletSystem.world );

			// add some of the included Composite objects
			verletSystem.world.addComposite( new Tire( new Vector2( 350, 64 ), 64, 32, 0.3f, 0.5f ) );
			verletSystem.world.addComposite( new Tire( new Vector2( 600, 32 ), 50, 4, 0.2f, 0.7f ) );
			verletSystem.world.addComposite( new Tire( new Vector2( 900, 128 ), 64, 7, 0.1f, 0.3f ) );

			verletSystem.world.addComposite( new Cloth( new Vector2( 900, 10 ), 200, 200, 20, 0.25f, 50 ) );

			verletSystem.world.addComposite( new Ragdoll( 400, 20, Random.range( 140, 240 ) ) );
			verletSystem.world.addComposite( new Ragdoll( 500, 20, Random.range( 140, 240 ) ) );
			verletSystem.world.addComposite( new Ragdoll( 600, 20, Random.range( 140, 240 ) ) );

			verletSystem.world.addComposite( new Ball( new Vector2( 100, 60 ), Random.range( 10, 50 ) ) );
			verletSystem.world.addComposite( new Ball( new Vector2( 150, 60 ), Random.range( 10, 50 ) ) );
			verletSystem.world.addComposite( new Ball( new Vector2( 200, 60 ), Random.range( 10, 50 ) ) );
		}


		void createPolygons()
		{
			var trianglePoints = new Vector2[] { new Vector2( 0, 0 ), new Vector2( 100, -100 ), new Vector2( -100, -150 ) };
			var triangleEntity = createEntity( "triangle" );
			triangleEntity.setPosition( 100, 300 );
			triangleEntity.addComponent( new PolygonMesh( trianglePoints, false ).setColor( Color.LightGreen ) );
			triangleEntity.addCollider( new PolygonCollider( trianglePoints ) );


			var circleEntity = createEntity( "circle" );
			circleEntity.setPosition( 1000, 250 );
			circleEntity.addComponent( new Sprite( content.Load<Texture2D>( Content.Shared.moon ) ) )
			            .setColor( Color.LightGreen );
			circleEntity.addCollider( new CircleCollider( 64 ) );


			var polyPoints = Polygon.buildSymmetricalPolygon( 5, 140 );
			var polygonEntity = createEntity( "boxCollider" );
			polygonEntity.setPosition( 460, 450 );
			polygonEntity.addComponent( new PolygonMesh( polyPoints ) ).setColor( Color.LightGreen );
			polygonEntity.addCollider( new PolygonCollider( polyPoints ) );

			polygonEntity.tweenRotationDegreesTo( 180, 3f )
				 .setLoops( Nez.Tweens.LoopType.PingPong, 50 )
				 .setEaseType( Nez.Tweens.EaseType.Linear )
				 .start();
		}


		void createRope( VerletWorld world )
		{
			// create an array of points for our rope
			var linePoints = new Vector2[10];
			for( var i = 0; i < 10; i++ )
				linePoints[i] = new Vector2( 30 * i + 50, 10 );

			var line = new LineSegments( linePoints, 0.3f )
				.pinParticleAtIndex( 0 );
			world.addComposite( line );
		}

	}
}
