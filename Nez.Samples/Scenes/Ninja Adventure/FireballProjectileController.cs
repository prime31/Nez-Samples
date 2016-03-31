using System;
using Microsoft.Xna.Framework;


namespace Nez.Samples
{
	/// <summary>
	/// moves a ProjectileMover and destroys the Entity if it hits anything
	/// </summary>
	public class FireballProjectileController : Component, IUpdatable
	{
		public Vector2 velocity;

		ProjectileMover _mover;


		public FireballProjectileController( Vector2 velocity )
		{
			this.velocity = velocity;
		}


		public override void onAddedToEntity()
		{
			_mover = entity.getComponent<ProjectileMover>();
		}


		void IUpdatable.update()
		{
			if( _mover.move( velocity * Time.deltaTime ) )
				entity.destroy();
		}
	}
}

