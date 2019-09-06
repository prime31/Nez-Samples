using System;
using Microsoft.Xna.Framework;


namespace Nez.Samples
{
	public class LineCaster : RenderableComponent, IUpdatable
	{
		private Vector2 lastPosition = new Vector2(101, 101);
		private Vector2 collisionPosition = new Vector2(-1, -1);

		// make sure we arent culled
		public override float Width
		{
			get { return 1000; }
		}

		public override float Height
		{
			get { return 1000; }
		}

		public LineCaster()
		{
		}

		public override void Render(Graphics graphics, Camera camera)
		{
			graphics.Batcher.DrawPixel(lastPosition.X, lastPosition.Y, Color.Yellow, 4);
			graphics.Batcher.DrawPixel(Transform.Position.X, Transform.Position.Y, Color.White, 4);
			graphics.Batcher.DrawLine(lastPosition, Transform.Position, Color.White);
			if (collisionPosition.X > 0 && collisionPosition.Y > 0)
			{
				graphics.Batcher.DrawPixel(collisionPosition.X, collisionPosition.Y, Color.Red, 4);
			}
		}

		void IUpdatable.Update()
		{
			if (Input.LeftMouseButtonPressed)
			{
				lastPosition = Transform.Position;
				Transform.Position = Input.MousePosition;
			}

			if (Input.RightMouseButtonPressed || Input.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Space))
			{
				var hit = Physics.Linecast(lastPosition, Transform.Position);
				if (hit.Collider != null)
				{
					collisionPosition = hit.Point;
				}
			}
		}
	}
}