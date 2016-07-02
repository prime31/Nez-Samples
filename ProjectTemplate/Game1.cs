using Microsoft.Xna.Framework;
using Nez;


namespace ProjectTemplate
{
	public class Game1 : Core
	{
		protected override void Initialize()
		{
			base.Initialize();

			Window.AllowUserResizing = true;

			// load up your initial scene here
			scene = Scene.createWithDefaultRenderer( Color.MonoGameOrange );
		}
	}
}

