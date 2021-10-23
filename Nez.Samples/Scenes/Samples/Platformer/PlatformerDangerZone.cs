using Microsoft.Xna.Framework;

namespace Nez.Samples
{
    public class PlatformerDangerZone : Component, ITriggerListener
    {
        Vector2 _respawnPoint;

        public PlatformerDangerZone(Vector2 respawnPoint)
        {
            _respawnPoint = respawnPoint;
        }

        #region ITriggerListener implementation
        
        public void OnTriggerEnter(Collider other, Collider local)
        {
            // everything that touches this zone gets relocated to the spawn point
            other.Entity.Position = _respawnPoint;
        }

        public void OnTriggerExit(Collider other, Collider local)
        { }
        
        #endregion
    }
}