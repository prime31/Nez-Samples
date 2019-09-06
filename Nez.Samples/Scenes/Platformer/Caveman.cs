using Microsoft.Xna.Framework;
using Nez.Sprites;
using Microsoft.Xna.Framework.Graphics;
using Nez.Textures;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using Nez.Tiled;


namespace Nez.Samples
{
	public class Caveman : Component, ITriggerListener, IUpdatable
	{
		public float MoveSpeed = 150;
		public float Gravity = 1000;
		public float JumpHeight = 16 * 5;

		enum Animations
		{
			Walk,
			Run,
			Idle,
			Attack,
			Death,
			Falling,
			Hurt,
			Jumping
		}

		Sprite<Animations> _animation;
		TiledMapMover _mover;
		BoxCollider _boxCollider;
		TiledMapMover.CollisionState _collisionState = new TiledMapMover.CollisionState();
		Vector2 _velocity;

		VirtualButton _jumpInput;
		VirtualIntegerAxis _xAxisInput;


		public override void OnAddedToEntity()
		{
			var texture = Entity.Scene.Content.Load<Texture2D>(Content.Platformer.Caveman);
			var subtextures = Subtexture.SubtexturesFromAtlas(texture, 32, 32);

			_boxCollider = Entity.GetComponent<BoxCollider>();
			_mover = Entity.GetComponent<TiledMapMover>();
			_animation = Entity.AddComponent(new Sprite<Animations>(subtextures[0]));

			// extract the animations from the atlas. they are setup in rows with 8 columns
			_animation.AddAnimation(Animations.Walk, new SpriteAnimation(new List<Subtexture>()
			{
				subtextures[0],
				subtextures[1],
				subtextures[2],
				subtextures[3],
				subtextures[4],
				subtextures[5]
			}));

			_animation.AddAnimation(Animations.Run, new SpriteAnimation(new List<Subtexture>()
			{
				subtextures[8 + 0],
				subtextures[8 + 1],
				subtextures[8 + 2],
				subtextures[8 + 3],
				subtextures[8 + 4],
				subtextures[8 + 5],
				subtextures[8 + 6]
			}));

			_animation.AddAnimation(Animations.Idle, new SpriteAnimation(new List<Subtexture>()
			{
				subtextures[16]
			}));

			_animation.AddAnimation(Animations.Attack, new SpriteAnimation(new List<Subtexture>()
			{
				subtextures[24 + 0],
				subtextures[24 + 1],
				subtextures[24 + 2],
				subtextures[24 + 3]
			}));

			_animation.AddAnimation(Animations.Death, new SpriteAnimation(new List<Subtexture>()
			{
				subtextures[40 + 0],
				subtextures[40 + 1],
				subtextures[40 + 2],
				subtextures[40 + 3]
			}));

			_animation.AddAnimation(Animations.Falling, new SpriteAnimation(new List<Subtexture>()
			{
				subtextures[48]
			}));

			_animation.AddAnimation(Animations.Hurt, new SpriteAnimation(new List<Subtexture>()
			{
				subtextures[64],
				subtextures[64 + 1]
			}));

			_animation.AddAnimation(Animations.Jumping, new SpriteAnimation(new List<Subtexture>()
			{
				subtextures[72 + 0],
				subtextures[72 + 1],
				subtextures[72 + 2],
				subtextures[72 + 3]
			}));

			SetupInput();
		}


		public override void OnRemovedFromEntity()
		{
			// deregister virtual input
			_jumpInput.Deregister();
			_xAxisInput.Deregister();
		}


		void SetupInput()
		{
			// setup input for jumping. we will allow z on the keyboard or a on the gamepad
			_jumpInput = new VirtualButton();
			_jumpInput.Nodes.Add(new Nez.VirtualButton.KeyboardKey(Keys.Z));
			_jumpInput.Nodes.Add(new Nez.VirtualButton.GamePadButton(0, Buttons.A));

			// horizontal input from dpad, left stick or keyboard left/right
			_xAxisInput = new VirtualIntegerAxis();
			_xAxisInput.Nodes.Add(new Nez.VirtualAxis.GamePadDpadLeftRight());
			_xAxisInput.Nodes.Add(new Nez.VirtualAxis.GamePadLeftStickX());
			_xAxisInput.Nodes.Add(new Nez.VirtualAxis.KeyboardKeys(VirtualInput.OverlapBehavior.TakeNewer, Keys.Left,
				Keys.Right));
		}


		void IUpdatable.Update()
		{
			// handle movement and animations
			var moveDir = new Vector2(_xAxisInput.Value, 0);
			var animation = Animations.Idle;

			if (moveDir.X < 0)
			{
				if (_collisionState.Below)
					animation = Animations.Run;
				_animation.FlipX = true;
				_velocity.X = -MoveSpeed;
			}
			else if (moveDir.X > 0)
			{
				if (_collisionState.Below)
					animation = Animations.Run;
				_animation.FlipX = false;
				_velocity.X = MoveSpeed;
			}
			else
			{
				_velocity.X = 0;
				if (_collisionState.Below)
					animation = Animations.Idle;
			}

			if (_collisionState.Below && _jumpInput.IsPressed)
			{
				animation = Animations.Jumping;
				_velocity.Y = -Mathf.Sqrt(2f * JumpHeight * Gravity);
			}

			if (!_collisionState.Below && _velocity.Y > 0)
				animation = Animations.Falling;

			// apply gravity
			_velocity.Y += Gravity * Time.DeltaTime;

			// move
			_mover.Move(_velocity * Time.DeltaTime, _boxCollider, _collisionState);

			if (_collisionState.Below)
				_velocity.Y = 0;

			if (!_animation.IsAnimationPlaying(animation))
				_animation.Play(animation);
		}


		#region ITriggerListener implementation

		void ITriggerListener.OnTriggerEnter(Collider other, Collider self)
		{
			Debug.Log("triggerEnter: {0}", other.Entity.Name);
		}


		void ITriggerListener.OnTriggerExit(Collider other, Collider self)
		{
			Debug.Log("triggerExit: {0}", other.Entity.Name);
		}

		#endregion
	}
}