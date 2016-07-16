using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez.Sprites;


namespace Nez.Samples
{
	[SampleScene( "Rigid Bodies", "ArcadeRigidBodies can be used for a game-like physics effect\nThis demo just applies some impulses and lets gravity do the rest" )]
	public class RigidBodyScene : SampleScene
	{
		public override void initialize()
		{
			base.initialize();

			Screen.setSize( 1280, 720 );

			var moonTexture = content.Load<Texture2D>( Content.Shared.moon );

			var friction = 0.3f;
			var elasticity = 0.4f;
			createEntity( new Vector2( 50, 200 ), 50f, friction, elasticity, new Vector2( 150, 0 ), moonTexture )
				.addImpulse( new Vector2( 10, 0 ) );
			createEntity( new Vector2( 800, 260 ), 5f, friction, elasticity, new Vector2( -180, 0 ), moonTexture );

			createEntity( new Vector2( 50, 400 ), 50f, friction, elasticity, new Vector2( 150, -40 ), moonTexture );
			createEntity( new Vector2( 800, 460 ), 5f, friction, elasticity, new Vector2( -180, -40 ), moonTexture );


			createEntity( new Vector2( 400, 0 ), 60f, friction, elasticity, new Vector2( 10, 90 ), moonTexture );
			createEntity( new Vector2( 500, 400 ), 4f, friction, elasticity, new Vector2( 0, -270 ), moonTexture );


			var rb = createEntity( new Vector2( Screen.width / 2, Screen.height / 2 + 250 ), 0, friction, elasticity, new Vector2( 0, -270 ), moonTexture );
			rb.entity.getComponent<Sprite>().color = Color.DarkMagenta;

			rb = createEntity( new Vector2( Screen.width / 2 - 200, Screen.height / 2 + 250 ), 0, friction, elasticity, new Vector2( 0, -270 ), moonTexture );
			rb.entity.getComponent<Sprite>().color = Color.DarkMagenta;


			// bottom fellas
			createEntity( new Vector2( 200, 700 ), 15f, friction, elasticity, new Vector2( 150, -150 ), moonTexture );
			createEntity( new Vector2( 800, 760 ), 15f, friction, elasticity, new Vector2( -180, -150 ), moonTexture );
			createEntity( new Vector2( 1200, 700 ), 1f, friction, elasticity, new Vector2( 0, 0 ), moonTexture )
				.addImpulse( new Vector2( -5, -20 ) );
			
			// top fellas
			createEntity( new Vector2( 100, 100 ), 1f, friction, elasticity, new Vector2( 100, 90 ), moonTexture )
				.addImpulse( new Vector2( 40, -10 ) );
			createEntity( new Vector2( 100, 700 ), 100f, friction, elasticity, new Vector2( 200, -270 ), moonTexture );
		}


		ArcadeRigidbody createEntity( Vector2 position, float mass, float friction, float elasticity, Vector2 velocity, Texture2D texture )
		{
			var rigidbody = new ArcadeRigidbody()
				.setMass( mass )
				.setFriction( friction )
				.setElasticity( elasticity )
				.setVelocity( velocity );

			var entity = createEntity( Utils.randomString( 3 ) );
			entity.transform.position = position;
			entity.addComponent( new Sprite( texture ) );
			entity.addComponent( rigidbody );
			entity.addCollider( new CircleCollider() );

			return rigidbody;
		}
	}
}

