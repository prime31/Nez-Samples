using System;
using Microsoft.Xna.Framework.Input;


namespace Nez.Samples
{
	/// <summary>
	/// simple Component that checks for a key press and runs an Action when it occurs.
	/// </summary>
	public class PressKeyToPerformAction : Component, IUpdatable
	{
		Keys _key;
		Action<Entity> _action;


		public PressKeyToPerformAction(Keys key, Action<Entity> action)
		{
			_key = key;
			_action = action;
		}


		void IUpdatable.Update()
		{
			if (Input.IsKeyPressed(_key))
				_action(Entity);
		}
	}
}