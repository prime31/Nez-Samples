using Nez.Sprites;
using Microsoft.Xna.Framework.Graphics;
using Nez.Textures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;


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

		SpriteAnimator _animator;

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
			var characterPng = Random.Range(1, 7);
			var texture = Entity.Scene.Content.Load<Texture2D>("NinjaAdventure/characters/" + characterPng);
			var sprites = Sprite.SpritesFromAtlas(texture, 16, 16);

			_mover = Entity.AddComponent(new Mover());
			_animator = Entity.AddComponent<SpriteAnimator>();

			// add a shadow that will only be rendered when our player is behind the details layer of the tilemap (RenderLayer -1). The shadow
			// must be in a renderLayer ABOVE the details layer to be visible.
			var shadow = Entity.AddComponent(new SpriteMime(Entity.GetComponent<SpriteRenderer>()));
			shadow.Color = new Color(10, 10, 10, 80);
			shadow.Material = Material.StencilRead();
			shadow.RenderLayer = -2; // ABOVE our tiledmap layer so it is visible

			// extract the animations from the atlas
			_animator.AddAnimation("WalkLeft", new[]
			{
				sprites[2],
				sprites[6],
				sprites[10],
				sprites[14]
			});
			_animator.AddAnimation("WalkRight",new[]
			{
				sprites[3],
				sprites[7],
				sprites[11],
				sprites[15]
			});
			_animator.AddAnimation("WalkDown", new[]
			{
				sprites[0],
				sprites[4],
				sprites[8],
				sprites[12]
			});
			_animator.AddAnimation("WalkUp", new[]
			{
				sprites[1],
				sprites[5],
				sprites[9],
				sprites[13]
			});

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
			_fireInput.Nodes.Add(new VirtualButton.KeyboardKey(Keys.Z));
			_fireInput.Nodes.Add(new VirtualButton.GamePadButton(0, Buttons.A));

			// horizontal input from dpad, left stick or keyboard left/right
			_xAxisInput = new VirtualIntegerAxis();
			_xAxisInput.Nodes.Add(new VirtualAxis.GamePadDpadLeftRight());
			_xAxisInput.Nodes.Add(new VirtualAxis.GamePadLeftStickX());
			_xAxisInput.Nodes.Add(new VirtualAxis.KeyboardKeys(VirtualInput.OverlapBehavior.TakeNewer, Keys.Left,Keys.Right));

			// vertical input from dpad, left stick or keyboard up/down
			_yAxisInput = new VirtualIntegerAxis();
			_yAxisInput.Nodes.Add(new VirtualAxis.GamePadDpadUpDown());
			_yAxisInput.Nodes.Add(new VirtualAxis.GamePadLeftStickY());
			_yAxisInput.Nodes.Add(new VirtualAxis.KeyboardKeys(VirtualInput.OverlapBehavior.TakeNewer, Keys.Up,Keys.Down));
		}

		void IUpdatable.Update()
		{
			// handle movement and animations
			var moveDir = new Vector2(_xAxisInput.Value, _yAxisInput.Value);
			var animation = "WalkDown";

			if (moveDir.X < 0)
				animation = "WalkLeft";
			else if (moveDir.X > 0)
				animation = "WalkRight";

			if (moveDir.Y < 0)
				animation = "WalkUp";
			else if (moveDir.Y > 0)
				animation = "WalkDown";


			if (moveDir != Vector2.Zero)
			{
				if (!_animator.IsAnimationActive(animation))
					_animator.Play(animation);
				else
					_animator.UnPause();

				var movement = moveDir * _moveSpeed * Time.DeltaTime;

				_mover.CalculateMovement(ref movement, out var res);
				_subpixelV2.Update(ref movement);
				_mover.ApplyMovement(movement);
			}
			else
			{
				_animator.Pause();
			}

			// handle firing a projectile
			if (_fireInput.IsPressed)
			{
				// fire a projectile in the direction we are facing
				var dir = Vector2.Zero;
				switch (_animator.CurrentAnimationName)
				{
					case "WalkUp":
						dir.Y = -1;
						break;
					case "WalkDown":
						dir.Y = 1;
						break;
					case "WalkRight":
						dir.X = 1;
						break;
					case "WalkLeft":
						dir.X = -1;
						break;
					default:
						dir = new Vector2(1, 0);
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