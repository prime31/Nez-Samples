using System;
using Nez.Particles;
using Microsoft.Xna.Framework.Input;
using Nez.UI;


namespace Nez.Samples
{
	public class ParticleSystemSelector : Component, IUpdatable
	{
		// array of all the particle systems we have available
		string[] _particleConfigs = new string[] {
				Content.ParticleDesigner.comet,
				Content.ParticleDesigner.atomicBubble,
				Content.ParticleDesigner.blueFlame,
				Content.ParticleDesigner.blueGalaxy,
				Content.ParticleDesigner.crazyBlue,
				Content.ParticleDesigner.electrons,
				Content.ParticleDesigner.fire,
				Content.ParticleDesigner.foam,
				Content.ParticleDesigner.girosGratis,
				Content.ParticleDesigner.intoTheBlue,
				Content.ParticleDesigner.jasonChoi_Flash,
				Content.ParticleDesigner.jasonChoi_Swirl01,
				Content.ParticleDesigner.jasonChoi_risingup,
				Content.ParticleDesigner.leaves,
				Content.ParticleDesigner.plasmaGlow,
				Content.ParticleDesigner.meksBloodSpill,
				Content.ParticleDesigner.realPopcorn,
				Content.ParticleDesigner.shootingFireball,
				Content.ParticleDesigner.snow,
				Content.ParticleDesigner.theSun,
				Content.ParticleDesigner.touchUp,
				Content.ParticleDesigner.trippy,
				Content.ParticleDesigner.winnerStars,
				Content.ParticleDesigner.huo1,
				Content.ParticleDesigner.wu1
		};

		// the ParticleEmitter component
		ParticleEmitter _particleEmitter;

		// the current emitter config index that we are playing
		int _currentParticleSystem = 0;

		// we keep around a reference to the CheckBoxes so that we can reset its state when we change particle systems
		bool _isCollisionEnabled;
		bool _simulateInWorldSpace = true;


		public override void onAddedToEntity()
		{
			createUI();
			loadParticleSystem();
		}


		void IUpdatable.update()
		{
			// toggle the emitter config used when Q/W are pressed
			if( Input.isKeyPressed( Keys.Q ) )
			{
				_currentParticleSystem = Mathf.decrementWithWrap( _currentParticleSystem, _particleConfigs.Length );
				loadParticleSystem();
			}

			if( Input.isKeyPressed( Keys.W ) )
			{
				_currentParticleSystem = Mathf.incrementWithWrap( _currentParticleSystem, _particleConfigs.Length );
				loadParticleSystem();
			}
		}


		/// <summary>
		/// loads a ParticleEmitterConfig and creates a ParticleEmitter to display it
		/// </summary>
		void loadParticleSystem()
		{
			// kill the ParticleEmitter if we already have one
			if( _particleEmitter != null )
				entity.removeComponent( _particleEmitter );

			// load up the config then add a ParticleEmitter
			var particleSystemConfig = entity.scene.contentManager.Load<ParticleEmitterConfig>( _particleConfigs[_currentParticleSystem] );
			_particleEmitter = entity.addComponent( new ParticleEmitter( particleSystemConfig ) );

			// set state based on the values of our CheckBoxes
			_particleEmitter.collisionConfig.enabled = _isCollisionEnabled;
			_particleEmitter.simulateInWorldSpace = _simulateInWorldSpace;
		}


		void createUI()
		{
			// stick a UI in so we can play with a few emitter options
			var uiCanvas = entity.scene.createEntity( "sprite-light-ui" ).addComponent( new UICanvas() );
			uiCanvas.isFullScreen = true;
			var skin = Skin.createDefaultSkin();

			var table = uiCanvas.stage.addElement( new Table() );
			table.setFillParent( true ).left().top().padLeft( 10 ).padTop( 30 );


			table.row().setPadTop( 20 ).setAlign( Align.left );

			var collisionCheckBox = table.add( new CheckBox( "Toggle Collision", skin ) ).getElement<CheckBox>();
			collisionCheckBox.isChecked = _isCollisionEnabled;
			collisionCheckBox.onChanged += isChecked =>
			{
				_particleEmitter.collisionConfig.enabled = isChecked;
				_isCollisionEnabled = isChecked;
			};

			table.row().setPadTop( 20 ).setAlign( Align.left );

			var worldSpaceCheckbox = table.add( new CheckBox( "Simulate in World Space", skin ) ).getElement<CheckBox>();
			worldSpaceCheckbox.isChecked = _simulateInWorldSpace;
			worldSpaceCheckbox.onChanged += isChecked =>
			{
				_particleEmitter.simulateInWorldSpace = isChecked;
				_simulateInWorldSpace = isChecked;
			};

			table.row().setPadTop( 20 ).setAlign( Align.left );

			var button = table.add( new TextButton( "Toggle Play/Pause", skin ) ).setFillX().setMinHeight( 30 ).getElement<TextButton>();
			button.onClicked += butt =>
			{
				if( _particleEmitter.isPlaying )
					_particleEmitter.pause();
				else
					_particleEmitter.play();
			};
		}

	}
}

