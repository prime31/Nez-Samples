using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Input;
using Nez;


namespace ProjectTemplate
{
	public class Game1 : Core
	{
		protected override void Initialize()
		{
			base.Initialize();

			Window.ClientSizeChanged += Core.onClientSizeChanged;
			Window.AllowUserResizing = true;

			// load up your initial scene here
			scene = Scene.createWithDefaultRenderer( Color.MonoGameOrange );
		}
	}
}

