using System;
using Microsoft.Xna.Framework;


namespace Nez.Samples
{
	public class CameraBounds : Component, IUpdatable
	{
		public Vector2 min, max;


		public CameraBounds()
		{
			// make sure we run last so the camera is already moved before we evaluate its position
			updateOrder = int.MaxValue;
		}


		public CameraBounds( Vector2 min, Vector2 max ) : this()
		{
			this.min = min;
			this.max = max;
		}


		public override void onAddedToEntity()
		{
			entity.updateOrder = int.MaxValue;
		}


		void IUpdatable.update()
		{
			var cameraBounds = entity.scene.camera.bounds;

			if( cameraBounds.top < min.Y )
				entity.scene.camera.position += new Vector2( 0, min.Y - cameraBounds.top );

			if( cameraBounds.left < min.X )
				entity.scene.camera.position += new Vector2( min.X - cameraBounds.left, 0 );

			if( cameraBounds.bottom > max.Y )
				entity.scene.camera.position += new Vector2( 0, max.Y - cameraBounds.bottom );

			if( cameraBounds.right > max.X )
				entity.scene.camera.position += new Vector2( max.X - cameraBounds.right, 0 );
		}
	}
}

