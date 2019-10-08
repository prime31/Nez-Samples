using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Nez.Sprites;


namespace Nez.Samples
{
	/// <summary>
	/// adds basic movement with the arrow keys and includes collision detection and resolution. A debug line is displayed when a collision
	/// occurs in the direction of the collision normal.
	/// </summary>
	public class SimpleMover : Component, IUpdatable
	{
		float _speed = 600f;
		Mover _mover;
		SpriteRenderer _sprite;


		public override void OnAddedToEntity()
		{
			_sprite = this.GetComponent<SpriteRenderer>();
			_mover = new Mover();
			Entity.AddComponent(_mover);
		}


		void IUpdatable.Update()
		{
			var moveDir = Vector2.Zero;

			if (Input.IsKeyDown(Keys.Left))
			{
				moveDir.X = -1f;
				if (_sprite != null)
					_sprite.FlipX = true;
			}
			else if (Input.IsKeyDown(Keys.Right))
			{
				moveDir.X = 1f;
				if (_sprite != null)
					_sprite.FlipX = false;
			}

			if (Input.IsKeyDown(Keys.Up))
				moveDir.Y = -1f;
			else if (Input.IsKeyDown(Keys.Down))
				moveDir.Y = 1f;


			if (moveDir != Vector2.Zero)
			{
				var movement = moveDir * _speed * Time.DeltaTime;

				if (_mover.Move(movement, out CollisionResult res))
					Debug.DrawLine(Entity.Position, Entity.Position + res.Normal * 100, Color.Black, 0.3f);
			}
		}
	}
}