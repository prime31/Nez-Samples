using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Nez.Sprites;
using Nez.Particles;


namespace Nez.Samples
{
	[SampleScene( "Particles", "Arrow keys to move. Exiting the Camera view will cull the particles.\nQ/W changes the particle system being played" )]
	public class ParticlesScene : SampleScene
	{
		public ParticlesScene() : base( true, true )
		{}


		public override void initialize()
		{
			clearColor = Color.Black;
			setDesignResolution( 1280, 720, Scene.SceneResolutionPolicy.None );
			Screen.setSize( 1280, 720 );

			// add the ParticleSystemSelector which handles input for the scene and a SimpleMover to move it around with the keyboard
			var particlesEntity = createEntity( "particles" );
			particlesEntity.transform.setPosition( Screen.center - new Vector2( 0, 200 ) );
			particlesEntity.addComponent( new ParticleSystemSelector() );
			particlesEntity.addComponent( new SimpleMover() );


			// create a couple moons for playing with particle collisions
			var moonTex = content.Load<Texture2D>( "Shared/moon" );

			var moonEntity = createEntity( "moon" );
			moonEntity.transform.position = new Vector2( Screen.backBufferWidth / 2, Screen.backBufferHeight / 2 + 100 );
			moonEntity.addComponent( new Sprite( moonTex ) );
			moonEntity.colliders.add( new CircleCollider() );

			// clone the first moonEntity to create the second
			var moonEntityTwo = moonEntity.clone( new Vector2( Screen.backBufferWidth / 2 - 100, Screen.backBufferHeight / 2 + 100 ) );
			addEntity( moonEntityTwo );
		}
	}
}

