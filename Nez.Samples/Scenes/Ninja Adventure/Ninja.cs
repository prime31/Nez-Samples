using System;
using Nez;
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

		Sprite<Animations> _animation;
		Mover _mover;
		float _moveSpeed = 100f;
		Vector2 _projectileVelocity = new Vector2( 175 );

		VirtualButton _fireInput;
		VirtualIntegerAxis _xAxisInput;
		VirtualIntegerAxis _yAxisInput;


		public override void onAddedToEntity()
		{
			// load up our character texture atlas. we have different characters in 1 - 6.png for variety
			var characterPng = Nez.Random.range( 1, 7 );
			var texture = entity.scene.contentManager.Load<Texture2D>( "NinjaAdventure/characters/" + characterPng );
			var subtextures = Subtexture.subtexturesFromAtlas( texture, 16, 16 );

			_mover = entity.addComponent( new Mover() );
			_animation = entity.addComponent( new Sprite<Animations>( subtextures[0] ) );

			// add a shadow that will only be rendered when our player is behind the detailss layer of the tilemap (renderLayer -1). The shadow
			// must be in a renderLayer ABOVE the details layer to be visible.
			var shadow = entity.addComponent( new SpriteMime( _animation ) );
			shadow.color = new Color( 10, 10, 10, 80 );
			shadow.material = Material.stencilRead();
			shadow.renderLayer = -2; // ABOVE our tiledmap layer so it is visible

			// extract the animations from the atlas
			_animation.addAnimation( Animations.WalkDown, new SpriteAnimation( new List<Subtexture>()
			{
				subtextures[0],
				subtextures[4],
				subtextures[8],
				subtextures[12]
			}) );

			_animation.addAnimation( Animations.WalkUp, new SpriteAnimation( new List<Subtexture>()
			{
				subtextures[1],
				subtextures[5],
				subtextures[9],
				subtextures[13]
			}) );

			_animation.addAnimation( Animations.WalkLeft, new SpriteAnimation( new List<Subtexture>()
			{
				subtextures[2],
				subtextures[6],
				subtextures[10],
				subtextures[14]
			}) );

			_animation.addAnimation( Animations.WalkRight, new SpriteAnimation( new List<Subtexture>()
			{
				subtextures[3],
				subtextures[7],
				subtextures[11],
				subtextures[15]
			}) );

			setupInput();
		}


		public override void onRemovedFromEntity()
		{
			// deregister virtual input
			_fireInput.deregister();
		}


		void setupInput()
		{
			// setup input for shooting a fireball. we will allow z on the keyboard or a on the gamepad
			_fireInput = new VirtualButton();
			_fireInput.nodes.Add( new Nez.VirtualButton.KeyboardKey( Keys.Z ) );
			_fireInput.nodes.Add( new Nez.VirtualButton.GamePadButton( 0, Buttons.A ) );

			// horizontal input from dpad, left stick or keyboard left/right
			_xAxisInput = new VirtualIntegerAxis();
			_xAxisInput.nodes.Add( new Nez.VirtualAxis.GamePadDpadLeftRight() );
			_xAxisInput.nodes.Add( new Nez.VirtualAxis.GamePadLeftStickX() );
			_xAxisInput.nodes.Add( new Nez.VirtualAxis.KeyboardKeys( VirtualInput.OverlapBehavior.TakeNewer, Keys.Left, Keys.Right ) );

			// vertical input from dpad, left stick or keyboard up/down
			_yAxisInput = new VirtualIntegerAxis();
			_yAxisInput.nodes.Add( new Nez.VirtualAxis.GamePadDpadUpDown() );
			_yAxisInput.nodes.Add( new Nez.VirtualAxis.GamePadLeftStickY() );
			_yAxisInput.nodes.Add( new Nez.VirtualAxis.KeyboardKeys( VirtualInput.OverlapBehavior.TakeNewer, Keys.Up, Keys.Down ) );
		}


		void IUpdatable.update()
		{
			// handle movement and animations
			var moveDir = new Vector2( _xAxisInput.value, _yAxisInput.value );
			var animation = Animations.WalkDown;

			if( moveDir.X < 0 )
				animation = Animations.WalkLeft;
			else if( moveDir.X > 0 )
				animation = Animations.WalkRight;

			if( moveDir.Y < 0 )
				animation = Animations.WalkUp;
			else if( moveDir.Y > 0 )
				animation = Animations.WalkDown;


			if( moveDir != Vector2.Zero )
			{
				if( !_animation.isAnimationPlaying( animation ) )
					_animation.play( animation );
				
				var movement = moveDir * _moveSpeed * Time.deltaTime;

				CollisionResult res;
				_mover.move( movement, out res );
			}
			else
			{
				_animation.stop();
			}

			// handle firing a projectile
			if( _fireInput.isPressed )
			{
				// fire a projectile in the direction we are facing
				var dir = Vector2.Zero;
				switch( _animation.currentAnimation )
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

				var ninjaScene = entity.scene as NinjaAdventureScene;
				ninjaScene.createProjectiles( entity.transform.position, _projectileVelocity * dir );
			}
		}


		#region ITriggerListener implementation

		void ITriggerListener.onTriggerEnter( Collider other, Collider self )
		{
			Debug.log( "triggerEnter: {0}", other.entity.name );
		}


		void ITriggerListener.onTriggerExit( Collider other, Collider self )
		{
			Debug.log( "triggerExit: {0}", other.entity.name );
		}

		#endregion

	}
}

