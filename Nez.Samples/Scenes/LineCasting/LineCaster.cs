using System;
using Microsoft.Xna.Framework;

namespace Nez.Samples
{
    public class LineCaster : RenderableComponent, IUpdatable
    {
        private Vector2 lastPosition = new Vector2(101, 101);
        private Vector2 collisionPosition = new Vector2(-1, -1);
		
        // make sure we arent culled
		public override float width { get { return 1000; } }
		public override float height { get { return 1000; } }

		public LineCaster()
        {
        }

        public override void render(Graphics graphics, Camera camera)
        {
            graphics.batcher.drawPixel(lastPosition.X, lastPosition.Y, Color.Yellow, 4);
			graphics.batcher.drawPixel(transform.position.X, transform.position.Y, Color.White, 4);
            graphics.batcher.drawLine(lastPosition, transform.position, Color.White);
            if (collisionPosition.X > 0 && collisionPosition.Y > 0) {
				graphics.batcher.drawPixel(collisionPosition.X, collisionPosition.Y, Color.Red, 4);
			}
		}

        void IUpdatable.update()
        {
            if (Input.leftMouseButtonPressed) {
                lastPosition = this.transform.position;
                transform.position = Input.mousePosition;
            }

            if (Input.rightMouseButtonPressed || Input.isKeyPressed(Microsoft.Xna.Framework.Input.Keys.Space))
            {
                var hit = Physics.linecast(lastPosition, transform.position);
                if (hit.collider != null) {
					collisionPosition = hit.point;
				}
            }
		}
    }
}
