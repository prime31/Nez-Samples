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
		public override float Width
		{
			get { return 1280; }
		}

		public override float Height
		{
			get { return 720; }
		}

		public VerletWorld World;


		public VerletSystem()
		{
			World = new VerletWorld(new Rectangle(0, 0, (int) Width, (int) Height));
		}


		void toggleGravity()
		{
			if (World.Gravity.Y > 0)
				World.Gravity.Y = -980f;
			else
				World.Gravity.Y = 980f;

			Debug.DrawText(string.Format("Gravity {0}", World.Gravity.Y > 0 ? "Down" : "Up"), Color.Red, 2, 2);
		}


		void toggleZeroGravity()
		{
			if (World.Gravity.Y == 0)
			{
				World.Gravity.Y = 980f;
				Debug.DrawText("Gravity Restored", Color.Red, 2, 2);
			}
			else
			{
				World.Gravity.Y = 0;
				Debug.DrawText("Zero Gravity", Color.Red, 2, 2);
			}
		}


		public void Update()
		{
			if (Input.IsKeyPressed(Keys.G))
				toggleGravity();

			if (Input.IsKeyPressed(Keys.Z))
				toggleZeroGravity();

			World.Update();
		}


		public override void Render(Graphics graphics, Camera camera)
		{
			World.DebugRender(graphics.Batcher);
		}
	}
}