using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;

namespace Nez.Samples
{
    public class PlatformerItem : Component, ITriggerListener
    {
        #region ITriggerListener implementation
        
        public void OnTriggerEnter(Collider other, Collider local)
        {
            // Just disappear
            Entity.Destroy();
        }

        public void OnTriggerExit(Collider other, Collider local)
        { }
        
        #endregion
    }
}