using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Input;


namespace Nez.Samples
{
	public class Game1 : Game
	{
		GraphicsDeviceManager graphics;


		public Game1()
		{
			graphics = new GraphicsDeviceManager( this );
			Content.RootDirectory = "Content";
			IsMouseVisible = true;
		}


		protected override void Update( GameTime gameTime )
		{
			#if !__IOS__ &&  !__TVOS__
			if( GamePad.GetState( PlayerIndex.One ).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown( Keys.Escape ) )
				Exit();
			#endif

			base.Update( gameTime );
		}


		protected override void Draw( GameTime gameTime )
		{
			graphics.GraphicsDevice.Clear( Color.CornflowerBlue );
            
		
            
			base.Draw( gameTime );
		}
	}
}

