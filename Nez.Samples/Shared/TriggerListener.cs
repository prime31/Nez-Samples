using System;


namespace Nez.Samples
{
	/// <summary>
	/// simple trigger listener that just logs enter/exit events
	/// </summary>
	public class TriggerListener : Component, ITriggerListener
	{
		void ITriggerListener.onTriggerEnter( Collider other, Collider self )
		{
			Debug.log( "onTriggerEnter: {0} entered {1}", other, self );
		}


		void ITriggerListener.onTriggerExit( Collider other, Collider self )
		{
			Debug.log( "onTriggerExit: {0} exited {1}", other, self );
		}

	}
}

