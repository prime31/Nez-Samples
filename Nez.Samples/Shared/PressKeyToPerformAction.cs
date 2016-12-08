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
		Action _action;


		public PressKeyToPerformAction( Keys key, Action action )
		{
			_key = key;
			_action = action;
		}


		void IUpdatable.update()
		{
			if( Input.isKeyPressed( _key ) )
				_action();
		}
	}
}
