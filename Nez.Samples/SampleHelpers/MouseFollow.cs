

namespace Nez.Samples
{
	public class MouseFollow : Component, IUpdatable
	{
		public void update()
		{
			entity.setPosition( Input.scaledMousePosition );
		}
	}
}

