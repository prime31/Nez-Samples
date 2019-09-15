namespace Nez.Samples
{
	/// <summary>
	/// simple trigger listener that just logs enter/exit events
	/// </summary>
	public class TriggerListener : Component, ITriggerListener
	{
		void ITriggerListener.OnTriggerEnter(Collider other, Collider self)
		{
			Debug.Log("onTriggerEnter: {0} entered {1}", other, self);
		}


		void ITriggerListener.OnTriggerExit(Collider other, Collider self)
		{
			Debug.Log("onTriggerExit: {0} exited {1}", other, self);
		}
	}
}