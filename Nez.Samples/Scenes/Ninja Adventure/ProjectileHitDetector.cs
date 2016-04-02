using System;
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
		public int hitsUntilDead = 10;

		int _hitCounter;
		Sprite _sprite;


		public override void onAddedToEntity()
		{
			_sprite = entity.getComponent<Sprite>();
		}

		
		void ITriggerListener.onTriggerEnter( Collider other, Collider self )
		{
			if( _hitCounter > hitsUntilDead )
			{
				entity.destroy();
				return;
			}

			_hitCounter++;
			_sprite.color = Color.Red;
			Core.schedule( 0.1f, timer => _sprite.color = Color.White );
		}


		void ITriggerListener.onTriggerExit( Collider other, Collider self )
		{}
	}
}

