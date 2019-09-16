using Microsoft.Xna.Framework;


namespace Nez.Samples
{
	public class LineCaster : RenderableComponent, IUpdatable
	{
		private Vector2 _lastPosition = new Vector2(101, 101);
		private Vector2 _collisionPosition = new Vector2(-1, -1);

		// make sure we arent culled
		public override float Width => 1000;

		public override float Height => 1000;

		public LineCaster()
		{
		}

		public override void Render(Batcher batcher, Camera camera)
		{
			batcher.DrawPixel(_lastPosition.X, _lastPosition.Y, Color.Yellow, 4);
			batcher.DrawPixel(Transform.Position.X, Transform.Position.Y, Color.White, 4);
			batcher.DrawLine(_lastPosition, Transform.Position, Color.White);
			if (_collisionPosition.X > 0 && _collisionPosition.Y > 0)
			{
				batcher.DrawPixel(_collisionPosition.X, _collisionPosition.Y, Color.Red, 10);
			}
		}

		void IUpdatable.Update()
		{
			if (Input.LeftMouseButtonPressed)
			{
				_lastPosition = Transform.Position;
				Transform.Position = Input.MousePosition;
                _collisionPosition = new Vector2(-1, -1);
            }

			if (Input.RightMouseButtonPressed || Input.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Space))
			{
				var hit = Physics.Linecast(_lastPosition, Transform.Position);
				if (hit.Collider != null)
				{
					_collisionPosition = hit.Point;
				}
			}
		}
	}
}