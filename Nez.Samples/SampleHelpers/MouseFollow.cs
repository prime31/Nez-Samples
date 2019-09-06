namespace Nez.Samples
{
	public class MouseFollow : Component, IUpdatable
	{
		public void Update()
		{
			Entity.SetPosition(Input.ScaledMousePosition);
		}
	}
}