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
			Content.ParticleDesigner_Comet,
			Content.ParticleDesigner_AtomicBubble,
			Content.ParticleDesigner_BlueFlame,
			Content.ParticleDesigner_BlueGalaxy,
			Content.ParticleDesigner_CrazyBlue,
			Content.ParticleDesigner_Electrons,
			Content.ParticleDesigner_Fire,
			Content.ParticleDesigner_Foam,
			Content.ParticleDesigner_GirosGratis,
			Content.ParticleDesigner_IntoTheBlue,
			Content.ParticleDesigner_JasonChoi_Flash,
			Content.ParticleDesigner_JasonChoi_Swirl01,
			Content.ParticleDesigner_JasonChoi_Risingup,
			Content.ParticleDesigner_Leaves,
			Content.ParticleDesigner_PlasmaGlow,
			Content.ParticleDesigner_MeksBloodSpill,
			Content.ParticleDesigner_RealPopcorn,
			Content.ParticleDesigner_ShootingFireball,
			Content.ParticleDesigner_Snow,
			Content.ParticleDesigner_TheSun,
			Content.ParticleDesigner_TouchUp,
			Content.ParticleDesigner_Trippy,
			Content.ParticleDesigner_WinnerStars,
			Content.ParticleDesigner_Huo1,
			Content.ParticleDesigner_Wu1
		};

		// the ParticleEmitter component
		ParticleEmitter _particleEmitter;

		// the current emitter config index that we are playing
		int _currentParticleSystem = 0;



		public override void onAddedToEntity()
		{
			loadParticleSystem();
			createUI();
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
		}


		void createUI()
		{
			// stick a UI in so we can play with a few emitter options
			var uiCanvas = entity.scene.createEntity( "sprite-light-ui" ).addComponent( new UICanvas() );
			uiCanvas.isFullScreen = true;
			//uiCanvas.renderLayer = SCREEN_SPACE_RENDER_LAYER;
			var skin = Skin.createDefaultSkin();

			var table = uiCanvas.stage.addElement( new Table() );
			table.setFillParent( true ).left().top().padLeft( 10 ).padTop( 30 );


			table.row().setPadTop( 20 ).setAlign( Align.left );

			var collisionCheckbox = table.add( new CheckBox( "Toggle Collision", skin ) ).getElement<CheckBox>();
			collisionCheckbox.onChanged += isChecked =>
			{
				_particleEmitter.collisionConfig.enabled = !_particleEmitter.collisionConfig.enabled;
			};

			table.row().setPadTop( 20 ).setAlign( Align.left );

			var worldSpaceCheckbox = table.add( new CheckBox( "Simulate in World Space", skin ) ).getElement<CheckBox>();
			worldSpaceCheckbox.isChecked = true;
			worldSpaceCheckbox.onChanged += isChecked =>
			{
				_particleEmitter.simulateInWorldSpace = isChecked;
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

