using Nez.Sprites;
using Microsoft.Xna.Framework;


namespace Nez.Samples
{
	/// <summary>
	/// simple component that detects if it has been hit by a projectile. When hit, it flashes red and destroys itself after being hit
	/// a certain number of times.
	/// </summary>
	public class ProjectileHitDetector : Component, ITriggerListener
	{
		public int HitsUntilDead = 10;

		int _hitCounter;
		SpriteRenderer _sprite;


		public override void OnAddedToEntity()
		{
			_sprite = Entity.GetComponent<SpriteRenderer>();
		}


		void ITriggerListener.OnTriggerEnter(Collider other, Collider self)
		{
			_hitCounter++;
			if (_hitCounter >= HitsUntilDead)
			{
				Entity.Destroy();
				return;
			}

			_sprite.Color = Color.Red;
			Core.Schedule(0.1f, timer => _sprite.Color = Color.White);
		}


		void ITriggerListener.OnTriggerExit(Collider other, Collider self)
		{
		}
	}
}