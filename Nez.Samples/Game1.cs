namespace Nez.Samples
{
	public class Game1 : Core
	{
		protected override void Initialize()
		{
			base.Initialize();

			Window.AllowUserResizing = true;
			Scene = new BasicScene();
		}
	}
}