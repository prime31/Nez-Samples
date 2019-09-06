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


		public override void OnAddedToEntity()
		{
			_mover = Entity.GetComponent<ProjectileMover>();
		}


		void IUpdatable.Update()
		{
			if( _mover.Move( velocity * Time.DeltaTime ) )
				Entity.Destroy();
		}
	}
}

