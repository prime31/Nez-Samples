using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Input;


namespace Nez.Samples
{
	public class Game1 : Core
	{
		protected override void Initialize()
		{
			base.Initialize();

			Window.ClientSizeChanged += Core.onClientSizeChanged;
			Window.AllowUserResizing = true;
			scene = new BasicScene();
		}
	}
}

