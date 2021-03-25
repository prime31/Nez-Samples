using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Nez.Sprites;


namespace Nez.Samples
{
	public class PlayerDashMover : Component, IUpdatable
	{
		float _speed = 800f;
		bool _isGrounded = true;
		bool _destroyedTile;
		Vector2 _moveDir;

		Mover _mover;
		TiledMapRenderer _tiledMapRenderer;
		SpriteRenderer _sprite;


		public override void OnAddedToEntity()
		{
			_sprite = this.GetComponent<SpriteRenderer>();
			_tiledMapRenderer = Entity.Scene.FindEntity("tiled-map").GetComponent<TiledMapRenderer>();
			_mover = new Mover();
			Entity.AddComponent(_mover);
		}


		void IUpdatable.Update()
		{
			Entity.Scene.Camera.Position = new Vector2(Entity.Scene.SceneRenderTargetSize.X / 2,
				Entity.Scene.SceneRenderTargetSize.Y / 2);
			if (_isGrounded)
			{
				if (Input.IsKeyPressed(Keys.Left))
				{
					_moveDir.X = -1f;
					if (!CanMove())
					{
						_moveDir.X = 0;
						return;
					}

					_sprite.FlipY = false;
					Entity.RotationDegrees = 90f;
				}
				else if (Input.IsKeyPressed(Keys.Right))
				{
					_moveDir.X = 1f;
					if (!CanMove())
					{
						_moveDir.X = 0;
						return;
					}

					_sprite.FlipY = false;
					Entity.RotationDegrees = -90f;
				}
				else if (Input.IsKeyPressed(Keys.Up))
				{
					_moveDir.Y = -1f;
					if (!CanMove())
					{
						_moveDir.Y = 0;
						return;
					}

					_sprite.FlipY = true;
					Entity.RotationDegrees = 0f;
				}
				else if (Input.IsKeyPressed(Keys.Down))
				{
					_moveDir.Y = 1f;
					if (!CanMove())
					{
						_moveDir.Y = 0;
						return;
					}

					_sprite.FlipY = false;
					Entity.RotationDegrees = 0f;
				}
			}


			if (_moveDir != Vector2.Zero)
			{
				var movement = _moveDir * _speed * Time.DeltaTime;
				CollisionResult res;
				if (_mover.Move(movement, out res))
				{
					var pos = Entity.Position + new Vector2(-16) * res.Normal;
					var tile = _tiledMapRenderer.GetTileAtWorldPosition(pos);

					// the presence of a tilesetTile means we have a tile with custom properties. The only tiles with custom properties are our
					// wall tiles which cannot be destroyed
					if (!_destroyedTile && tile != null && tile.TilesetTile == null)
					{
						_destroyedTile = true;
						_tiledMapRenderer.CollisionLayer.RemoveTile(tile.X, tile.Y);
						_tiledMapRenderer.RemoveColliders();
						_tiledMapRenderer.AddColliders();
					}
					else
					{
						_moveDir = Vector2.Zero;
						_isGrounded = true;
						_destroyedTile = false;
						Entity.GetComponent<CameraShake>().Shake(8, 0.8f);
					}
				}
				else
				{
					_isGrounded = false;
				}
			}
		}


		bool CanMove()
		{
			var pos = Entity.Position + new Vector2(16) * _moveDir;
			var tile = _tiledMapRenderer.GetTileAtWorldPosition(pos);

			return tile == null;
		}
	}
}