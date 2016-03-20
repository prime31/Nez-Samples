using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez.Sprites;


namespace Nez.Samples
{
	/// <summary>
	/// demos the SpringGrid with a SimpleMover entity that applies forces to the grid. vignette and bloom post processors are also
	/// added to give the scene some life.
	/// </summary>
	[SampleScene( "Spring Grid", "SpringGrid component with vignette and bloom\nArrow keys to move\nSpace to apply an explosive force" )]
	public class SpringGridScene : SampleScene
	{
		public override void initialize()
		{
			base.initialize();

			Screen.setSize( 1280, 720 );
			clearColor = Color.Black;
			var moonTex = contentManager.Load<Texture2D>( "Shared/moon" );

			var gridEntity = createEntity( "grid" );
			gridEntity.addComponent( new SpringGrid( new Rectangle( 0, 0, Screen.width, Screen.height ), new Vector2( 30 ) ) );


			var playerEntity = createEntity( "player", new Vector2( Screen.width / 2, Screen.height / 2 ) );
			playerEntity.transform.scale *= 0.5f;
			playerEntity.addComponent( new SimpleMover() );
			playerEntity.addComponent( new GridModifier() );
			playerEntity.addComponent( new Sprite( moonTex ) );


			addPostProcessor( new VignettePostProcessor( 1 ) );
			addPostProcessor( new BloomPostProcessor( 3 ) ).settings = BloomSettings.presetSettings[0];
		}
	}
}

