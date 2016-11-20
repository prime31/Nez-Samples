using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Nez.Verlet;


namespace Nez.Samples
{
	/// <summary>
	/// component that manages the Verlet World calling it's update and debugRender methods. Also handles toggling gravity via the G and Z keys.
	/// </summary>
	public class VerletSystem : RenderableComponent, IUpdatable
	{
		public override float width { get { return 1280; } }
		public override float height { get { return 720; } }

		public VerletWorld world;


		public VerletSystem()
		{
			world = new VerletWorld( new Rectangle( 0, 0, (int)width, (int)height ) );
		}


		void toggleGravity()
		{
			if( world.gravity.Y > 0 )
				world.gravity.Y = -980f;
			else
				world.gravity.Y = 980f;

			Debug.drawText( string.Format( "Gravity {0}", world.gravity.Y > 0 ? "Down" : "Up" ), Color.Red, 2, 2 );
		}


		void toggleZeroGravity()
		{
			if( world.gravity.Y == 0 )
			{
				world.gravity.Y = 980f;
				Debug.drawText( "Gravity Restored", Color.Red, 2, 2 );
			}
			else
			{
				world.gravity.Y = 0;
				Debug.drawText( "Zero Gravity", Color.Red, 2, 2 );
			}
		}


		public void update()
		{
			if( Input.isKeyPressed( Keys.G ) )
				toggleGravity();

			if( Input.isKeyPressed( Keys.Z ) )
				toggleZeroGravity();
			
			world.update();
		}


		public override void render( Graphics graphics, Camera camera )
		{
			world.debugRender( graphics.batcher );
		}
	
	}
}
