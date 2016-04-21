using System;


namespace Nez.Samples
{
	public class MouseFollow : Component, IUpdatable
	{
		public void update()
		{
			entity.transform.position = Input.scaledMousePosition;
		}
	}
}

