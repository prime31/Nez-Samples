using System;


namespace Nez.Samples
{
	/// <summary>
	/// simple trigger listener that just logs enter/exit events
	/// </summary>
	public class TriggerListener : Component, Mover.ITriggerListener
	{
		public void onTriggerEnter( Collider other )
		{
			Debug.log( "onTriggerEnter: {0}", other );
		}


		public void onTriggerExit( Collider other )
		{
			Debug.log( "onTriggerExit: {0}", other );
		}

	}
}

