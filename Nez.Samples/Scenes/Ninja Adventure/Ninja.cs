using Nez.Sprites;
using Microsoft.Xna.Framework.Graphics;
using Nez.Textures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;


namespace Nez.Samples
{
	public class Ninja : Component, ITriggerListener, IUpdatable
	{
		enum Animations
		{
			WalkUp,
			WalkDown,
			WalkRight,
			WalkLeft
		}

		SpriteAnimationRenderer<Animations> _animation;
		SubpixelVector2 _subpixelV2 = new SubpixelVector2();
		Mover _mover;
		float _moveSpeed = 100f;
		Vector2 _projectileVelocity = new Vector2(175);

		VirtualButton _fireInput;
		VirtualIntegerAxis _xAxisInput;
		VirtualIntegerAxis _yAxisInput;


		public override void OnAddedToEntity()
		{
			// load up our character texture atlas. we have different characters in 1 - 6.png for variety
			var characterPng = Nez.Random.Range(1, 7);
			var texture = Entity.Scene.Content.Load<Texture2D>("NinjaAdventure/characters/" + characterPng);
			var subtextures = Sprite.SpritesFromAtlas(texture, 16, 16);

			_mover = Entity.AddComponent(new Mover());
			_animation = Entity.AddComponent(new SpriteAnimationRenderer<Animations>(subtextures[0]));

			// add a shadow that will only be rendered when our player is behind the detailss layer of the tilemap (renderLayer -1). The shadow
			// must be in a renderLayer ABOVE the details layer to be visible.
			var shadow = Entity.AddComponent(new SpriteMime(_animation));
			shadow.Color = new Color(10, 10, 10, 80);
			shadow.Material = Material.StencilRead();
			shadow.RenderLayer = -2; // ABOVE our tiledmap layer so it is visible

			// extract the animations from the atlas
			_animation.AddAnimation(Animations.WalkDown, new SpriteAnimation(new List<Sprite>()
			{
				subtextures[0],
				subtextures[4],
				subtextures[8],
				subtextures[12]
			}));

			_animation.AddAnimation(Animations.WalkUp, new SpriteAnimation(new List<Sprite>()
			{
				subtextures[1],
				subtextures[5],
				subtextures[9],
				subtextures[13]
			}));

			_animation.AddAnimation(Animations.WalkLeft, new SpriteAnimation(new List<Sprite>()
			{
				subtextures[2],
				subtextures[6],
				subtextures[10],
				subtextures[14]
			}));

			_animation.AddAnimation(Animations.WalkRight, new SpriteAnimation(new List<Sprite>()
			{
				subtextures[3],
				subtextures[7],
				subtextures[11],
				subtextures[15]
			}));

			SetupInput();
		}


		public override void OnRemovedFromEntity()
		{
			// deregister virtual input
			_fireInput.Deregister();
		}


		void SetupInput()
		{
			// setup input for shooting a fireball. we will allow z on the keyboard or a on the gamepad
			_fireInput = new VirtualButton();
			_fireInput.Nodes.Add(new Nez.VirtualButton.KeyboardKey(Keys.Z));
			_fireInput.Nodes.Add(new Nez.VirtualButton.GamePadButton(0, Buttons.A));

			// horizontal input from dpad, left stick or keyboard left/right
			_xAxisInput = new VirtualIntegerAxis();
			_xAxisInput.Nodes.Add(new Nez.VirtualAxis.GamePadDpadLeftRight());
			_xAxisInput.Nodes.Add(new Nez.VirtualAxis.GamePadLeftStickX());
			_xAxisInput.Nodes.Add(new Nez.VirtualAxis.KeyboardKeys(VirtualInput.OverlapBehavior.TakeNewer, Keys.Left,
				Keys.Right));

			// vertical input from dpad, left stick or keyboard up/down
			_yAxisInput = new VirtualIntegerAxis();
			_yAxisInput.Nodes.Add(new Nez.VirtualAxis.GamePadDpadUpDown());
			_yAxisInput.Nodes.Add(new Nez.VirtualAxis.GamePadLeftStickY());
			_yAxisInput.Nodes.Add(new Nez.VirtualAxis.KeyboardKeys(VirtualInput.OverlapBehavior.TakeNewer, Keys.Up,
				Keys.Down));
		}


		void IUpdatable.Update()
		{
			// handle movement and animations
			var moveDir = new Vector2(_xAxisInput.Value, _yAxisInput.Value);
			var animation = Animations.WalkDown;

			if (moveDir.X < 0)
				animation = Animations.WalkLeft;
			else if (moveDir.X > 0)
				animation = Animations.WalkRight;

			if (moveDir.Y < 0)
				animation = Animations.WalkUp;
			else if (moveDir.Y > 0)
				animation = Animations.WalkDown;


			if (moveDir != Vector2.Zero)
			{
				if (!_animation.IsAnimationPlaying(animation))
					_animation.Play(animation);

				var movement = moveDir * _moveSpeed * Time.DeltaTime;

				_mover.CalculateMovement(ref movement, out var res);
				_subpixelV2.Update(ref movement);
				_mover.ApplyMovement(movement);
			}
			else
			{
				_animation.Stop();
			}

			// handle firing a projectile
			if (_fireInput.IsPressed)
			{
				// fire a projectile in the direction we are facing
				var dir = Vector2.Zero;
				switch (_animation.CurrentAnimation)
				{
					case Animations.WalkUp:
						dir.Y = -1;
						break;
					case Animations.WalkDown:
						dir.Y = 1;
						break;
					case Animations.WalkRight:
						dir.X = 1;
						break;
					case Animations.WalkLeft:
						dir.X = -1;
						break;
				}

				var ninjaScene = Entity.Scene as NinjaAdventureScene;
				ninjaScene.CreateProjectiles(Entity.Transform.Position, _projectileVelocity * dir);
			}
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