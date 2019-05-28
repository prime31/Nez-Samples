using System; // For introspection
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nez.Particles;
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

		// The currently load particle emitter configuration
		ParticleEmitterConfig _particleEmitterConfig;

		// the current emitter config index that we are playing
		int _currentParticleSystem = 0;

		// we keep around a reference to the CheckBoxes so that we can reset its state when we change particle systems
		bool _isCollisionEnabled;
		bool _simulateInWorldSpace = true;


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
				resetUI();
			}

			if( Input.isKeyPressed( Keys.W ) )
			{
				_currentParticleSystem = Mathf.incrementWithWrap( _currentParticleSystem, _particleConfigs.Length );
				loadParticleSystem();
				resetUI();
			}
		}


		/// <summary>
		/// loads a ParticleEmitterConfig and creates a ParticleEmitter to display it
		/// </summary>
		void loadParticleSystem()
		{
			// load up the config then add a ParticleEmitter
			_particleEmitterConfig = entity.scene.content.Load<ParticleEmitterConfig>( _particleConfigs[_currentParticleSystem] );
			resetEmitter();
		}


		void resetEmitter()
		{
			// kill the ParticleEmitter if we already have one
			if( _particleEmitter != null )
				entity.removeComponent( _particleEmitter );

			_particleEmitter = entity.addComponent( new ParticleEmitter( _particleEmitterConfig ) );

			// set state based on the values of our CheckBoxes
			_particleEmitter.collisionConfig.enabled = _isCollisionEnabled;
			_particleEmitter.simulateInWorldSpace = _simulateInWorldSpace;
		}

		/// <summary>
		/// Resets the user interface after the particle system is changed.
		/// (Tears down the old interface and builds a new one.  This is especially important
		/// with Gravity vs Radial emitters which have different parameters to control behavior.)
		/// </summary>
		void resetUI()
		{
			var previousUI = entity.scene.findEntity( "particles-ui" );
			if(previousUI != null)
				previousUI.destroy();

			createUI();
		}

		void createUI()
		{

			var uiCanvas = entity.scene.createEntity( "particles-ui" ).addComponent( new UICanvas() );
			uiCanvas.isFullScreen = true;
			var skin = Skin.createDefaultSkin();
			condenseSkin( skin );

			// Stolen from RuntimeInspector.cs
			var table = new Table();
			table.top().left();
			table.defaults().setPadTop( 4 ).setPadLeft( 4 ).setPadRight( 0 ).setAlign( Align.left );
			table.setBackground( new PrimitiveDrawable( new Color( 40, 40, 40, 220 ) ) );

			// wrap up the table in a ScrollPane
			var scrollPane = uiCanvas.stage.addElement( new ScrollPane( table, skin ) );

			// force a validate which will layout the ScrollPane and populate the proper scrollBarWidth
			scrollPane.validate();
			scrollPane.setSize( 340 + scrollPane.getScrollBarWidth(), Screen.height );

			table.row().setPadTop( 40 ); // Leave room for the directions

			var collisionCheckBox = table.add( new CheckBox( "Toggle Collision", skin ) ).getElement<CheckBox>();
			collisionCheckBox.isChecked = _isCollisionEnabled;
			collisionCheckBox.onChanged += isChecked =>
			{
				_particleEmitter.collisionConfig.enabled = isChecked;
				_isCollisionEnabled = isChecked;
			};

			table.row();

			var worldSpaceCheckbox = table.add( new CheckBox( "Simulate in World Space", skin ) ).getElement<CheckBox>();
			worldSpaceCheckbox.isChecked = _simulateInWorldSpace;
			worldSpaceCheckbox.onChanged += isChecked =>
			{
				_particleEmitter.simulateInWorldSpace = isChecked;
				_simulateInWorldSpace = isChecked;
			};

			table.row();

			var button = table.add( new TextButton( "Toggle Play/Pause", skin ) ).setFillX().setMinHeight( 30 ).getElement<TextButton>();
			button.onClicked += butt =>
			{
				if( _particleEmitter.isPlaying )
					_particleEmitter.pause();
				else
					_particleEmitter.play();
			};

			table.add( "" );
			var save = table.add( new TextButton( "Save as .pex", skin ) ).setFillX().setMinHeight( 30 ).getElement<TextButton>();
			save.onClicked += butt =>
			{
				exportPexClicked( skin );
			};

			table.row();
			makeSection( table, skin, "General" );
			table.row();
			makeEmitterDropdown( table, skin, "Emitter Type" );
			table.row();
			makeBlendDropdown( table, skin, "Source Blend Function", "blendFuncSource" );
			table.row();
			makeBlendDropdown( table, skin, "Destination Blend Function", "blendFuncDestination" );

			table.row();
			makeSection( table, skin, "Emitter Parameters" );
			table.row();
			makeVector2( table, skin, "Source Position Variance", "sourcePositionVariance" );
			table.row();
			makeSlider( table, skin, "Particle Lifespan", 0, 30, 0.5f, "particleLifespan" );
			table.row();
			makeSlider( table, skin, "Particle Lifespan Variance", 0, 30, 0.5f, "particleLifespanVariance" );
			table.row();
			makeSlider( table, skin, "Emission rate", 0, 4000, 1, "emissionRate" );
			table.row();
			makeSlider( table, skin, "Duration", -1, 2000, 1, "duration" );
			table.row();
			makeSlider( table, skin, "Angle", 0, 360, 1, "angle" );
			table.row();
			makeSlider( table, skin, "Angle Variance", 0, 360, 1, "angleVariance" );
			table.row();
			makeSlider( table, skin, "Maximum Particles", 0, 2000, 1, "maxParticles" );
			table.row();
			makeSlider( table, skin, "Start Particle Size", 0, 2000, 1, "startParticleSize" );
			table.row();
			makeSlider( table, skin, "Start Size Variance", 0, 2000, 1, "startParticleSizeVariance" );
			table.row();
			makeSlider( table, skin, "Finish Particle Size", 0, 2000, 1, "finishParticleSize" );
			table.row();
			makeSlider( table, skin, "Finish Size Variance", 0, 2000, 1, "finishParticleSizeVariance" );
			table.row();
			makeSlider( table, skin, "Rotation Start", 0, 2000, 1, "rotationStart" );
			table.row();
			makeSlider( table, skin, "Rotation Start Variance", 0, 2000, 1, "rotationStartVariance" );
			table.row();
			makeSlider( table, skin, "Rotation End", 0, 2000, 1, "rotationEnd" );
			table.row();
			makeSlider( table, skin, "Rotation End Variance", 0, 2000, 1, "rotationEndVariance" );
			table.row();
			makeColor( table, skin, "Start Color (R G B A)", "startColor" );
			table.row();
			makeColor( table, skin, "Start Color Variance", "startColorVariance" );
			table.row();
			makeColor( table, skin, "Finish Color (R G B A)", "finishColor" );
			table.row();
			makeColor( table, skin, "Finish Color Variance", "finishColorVariance" );


			if( _particleEmitterConfig.emitterType == ParticleEmitterType.Gravity )
			{
				table.row();
				makeSection( table, skin, "Gravity Emitter" );
				table.row();
				makeVector2( table, skin, "Gravity X/Y", "gravity" );
				table.row();
				makeSlider( table, skin, "Speed", 0, 2000, 1, "speed" );
				table.row();
				makeSlider( table, skin, "Speed Variance", 0, 2000, 1, "speedVariance" );
				table.row();
				makeSlider( table, skin, "Radial Acceleration", -2000, 2000, 1, "radialAcceleration" );
				table.row();
				makeSlider( table, skin, "Radial Accel Variance", 0, 2000, 1, "radialAccelVariance" );
				table.row();
				makeSlider( table, skin, "Tangential Acceleration", -2000, 2000, 1, "tangentialAcceleration" );
				table.row();
				makeSlider( table, skin, "Tangential Accel Variance", 0, 2000, 1, "tangentialAccelVariance" );
			} else
			{
				table.row();
				makeSection( table, skin, "Radial Emitter" );
				table.row();
				makeSlider( table, skin, "Maximum Radius", 0, 1000, 1, "maxRadius" );
				table.row();
				makeSlider( table, skin, "Maximum Radius Variance", 0, 1000, 1, "maxRadiusVariance" );
				table.row();
				makeSlider( table, skin, "Minimum Radius", 0, 1000, 1, "minRadius" );
				table.row();
				makeSlider( table, skin, "Minimum Radius Variance", 0, 1000, 1, "minRadiusVariance" );
				table.row();
				makeSlider( table, skin, "Rotation/Sec", 0, 1000, 1, "rotatePerSecond" );
				table.row();
				makeSlider( table, skin, "Rotation/Sec Variance", 0, 1000, 1, "rotatePerSecondVariance" );
			}
		}

		void makeSlider(Table table, Skin skin, string label, float min, float max, float step, string propertyName)
		{
			FieldInfo fieldInfo = typeof( ParticleEmitterConfig ).GetField( propertyName );
			float value = Convert.ToSingle( fieldInfo.GetValue( _particleEmitterConfig ) );

			var slider = new Slider( skin, null, min, max );
			var textBox = new TextField( value.ToString(), skin );

			slider.setStepSize( step );
			slider.setValue( value );
			slider.onChanged += newValue => {
				fieldInfo.SetValue( _particleEmitterConfig, typedConvert( fieldInfo, newValue ) );
				textBox.setText( newValue.ToString() );
				resetEmitter();
			};

			textBox.setMaxLength( 5 );
			textBox.onTextChanged += (field, str) => {
				if( float.TryParse( str, out float newValue ))
				{
					fieldInfo.SetValue( _particleEmitterConfig, typedConvert( fieldInfo, newValue ) );
					slider.setValue( newValue );
					resetEmitter();
				}

			};

			table.add( label ).left().width( 140 );
			table.add( textBox ).left().width( 30 );
			table.add( slider ).left();
		}

		void makeBlendDropdown(Table table, Skin skin, string label, string propertyName)
		{
			FieldInfo fieldInfo = typeof( ParticleEmitterConfig ).GetField( propertyName );
			Blend value = (Blend)fieldInfo.GetValue( _particleEmitterConfig );

			var dropdown = new SelectBox<string>( skin );
			var dropdownList = new List<string>() {
				"Zero",
				"One",
				"SourceColor",
				"InverseSourceColor",
				"SourceAlpha",
				"InverseSourceAlpha",
				"DestinationAlpha",
				"InverseDestinationAlpha",
				"DestinationColor",
				"InverseDestinationColor",
				"SourceAlphaSaturation"
			};
			dropdown.setItems( dropdownList );

			// Make a lookup table from string to blend function
			var nameLookup = new Dictionary<string, Blend>() {
				{ "Zero", Blend.Zero },
				{ "One", Blend.One },
				{ "SourceColor", Blend.SourceColor },
				{ "InverseSourceColor", Blend.InverseSourceColor },
				{ "SourceAlpha", Blend.SourceAlpha },
				{ "InverseSourceAlpha", Blend.InverseSourceAlpha },
				{ "DestinationAlpha", Blend.DestinationAlpha },
				{ "InverseDestinationAlpha", Blend.InverseDestinationAlpha },
				{ "DestinationColor", Blend.DestinationColor },
				{ "InverseDestinationColor", Blend.InverseDestinationColor },
				{ "SourceAlphaSaturation", Blend.SourceAlphaSaturation }
			};

			// Make a lookup table from blend function to string
			var functionLookup = new Dictionary<Blend, string>();
			foreach(var str in nameLookup.Keys)
			{
				functionLookup.Add( nameLookup[str], str );
			};

			dropdown.setSelectedIndex( dropdownList.IndexOf( functionLookup[value] ) );
			dropdown.onChanged += (str) => {
				Blend newValue = nameLookup[str];
				fieldInfo.SetValue( _particleEmitterConfig, newValue );
				resetEmitter();
			};

			table.add( label ).left().width( 140 );
			table.add( "" ).width( 1 ); // This table has 3 columns
			table.add( dropdown );
		}

		void makeEmitterDropdown(Table table, Skin skin, string label)
		{
			ParticleEmitterType value = _particleEmitterConfig.emitterType;

			var dropdown = new SelectBox<string>( skin );
			var dropdownList = new List<string>() {
				"Gravity",
				"Radial"
			};

			dropdown.setItems( dropdownList );

			if( _particleEmitterConfig.emitterType == ParticleEmitterType.Gravity )
				dropdown.setSelectedIndex( 0 );
			else
				dropdown.setSelectedIndex( 1 );

			dropdown.onChanged += (str) => {
				if( str == "Gravity" )
					_particleEmitterConfig.emitterType = ParticleEmitterType.Gravity;
				else
					_particleEmitterConfig.emitterType = ParticleEmitterType.Radial;
				resetEmitter();
				resetUI();
			};

			table.add( label ).left().width( 140 );
			table.add( "" ).width( 1 ); // This is a 3 column table
			table.add( dropdown );
		}

		void makeVector2(Table table, Skin skin, string label, string propertyName)
		{
			FieldInfo fieldInfo = typeof( ParticleEmitterConfig ).GetField( propertyName );
			Vector2 value = (Vector2)fieldInfo.GetValue( _particleEmitterConfig );
			var x = new TextField( value.X.ToString(), skin );
			var y = new TextField( value.Y.ToString(), skin );

			x.onTextChanged += (textbox, str) => {
				if( float.TryParse( str, out float newValue ))
				{
					value.X = newValue;
					fieldInfo.SetValue( _particleEmitterConfig, value );
					resetEmitter();
				}
			};

			y.onTextChanged += (textbox, str) => {
				if( float.TryParse( str, out float newValue ))
				{
					value.Y = newValue;
					fieldInfo.SetValue( _particleEmitterConfig, value );
					resetEmitter();
				}
			};

			table.add( label ).width( 140 );
			table.add( x ).width( 30 );
			table.add( y ).width( 30 );
		}

		void makeColor(Table table, Skin skin, string label, string propertyName)
		{
			FieldInfo fieldInfo = typeof( ParticleEmitterConfig ).GetField( propertyName );
			Color value = (Color)fieldInfo.GetValue( _particleEmitterConfig );

			var r = new TextField( value.R.ToString(), skin ).setMaxLength( 4 );
			var g = new TextField( value.G.ToString(), skin ).setMaxLength( 4 );
			var b = new TextField( value.B.ToString(), skin ).setMaxLength( 4 );
			var a = new TextField( value.A.ToString(), skin ).setMaxLength( 4 );

			void onChanged(TextField box, string str)
			{
				if( int.TryParse( r.getText(), out int newR )
					&& int.TryParse( g.getText(), out int newG )
					&& int.TryParse( b.getText(), out int newB )
					&& int.TryParse( a.getText(), out int newA )
					&& newR >= 0 && newR <= 255
					&& newB >= 0 && newB <= 255
					&& newG >= 0 && newG <= 255
					&& newR >= 0 && newA <= 255)
				{
					var newColor = new Color( newR, newG, newB, newA );
					fieldInfo.SetValue( _particleEmitterConfig, newColor );
				}
			}

			r.onTextChanged += onChanged;
			b.onTextChanged += onChanged;
			g.onTextChanged += onChanged;
			a.onTextChanged += onChanged;

			table.add( label ).width( 140 );
			table.add( r ).width( 30 );
			// Everything else is dealing with 3 columns.  Wrap the last ones up in
			// an hbox to avoid creating new columns in the table. (hack)
			g.setPreferredWidth( 30 );
			b.setPreferredWidth( 30 );
			a.setPreferredWidth( 30 );
			var hbox = new HorizontalGroup().setSpacing( 10 );
			hbox.addElement( g );
			hbox.addElement( b );
			hbox.addElement( a );
			table.add( hbox ).left().top().width( 30 );
		}

		void makeSection(Table table, Skin skin, string sectionName)
		{
			var label = new Label( sectionName, skin );
			label.setFontColor( new Color( 241, 156, 0 ) );
			table.add( label ).setPadTop( 20 );
		}


		/// <summary>
		/// Uses relfection to convert an object's property into the appropriate type
		/// </summary>
		/// <returns>A converted object in the propert type</returns>
		object typedConvert(FieldInfo fieldInfo, object value)
		{
			if( fieldInfo.FieldType == typeof( float ))
				return (float)value;

			if( fieldInfo.FieldType == typeof( int ))
				return (int)Convert.ToSingle( value );

			if( fieldInfo.FieldType == typeof( uint ))
				return (uint)Convert.ToSingle( value );

			return Convert.ToString( value );
		}

		void condenseSkin(Skin skin)
		{
			// This is ripped from the inspector

			// modify some of the default styles to better suit our needs
			var tfs = skin.get<TextFieldStyle>();
			tfs.background.leftWidth = tfs.background.rightWidth = 4;
			tfs.background.bottomHeight = 0;
			tfs.background.topHeight = 3;

			var checkbox = skin.get<CheckBoxStyle>();
			checkbox.checkboxOn.minWidth = checkbox.checkboxOn.minHeight = 15;
			checkbox.checkboxOff.minWidth = checkbox.checkboxOff.minHeight = 15;
			checkbox.checkboxOver.minWidth = checkbox.checkboxOver.minHeight = 15;
		}

		/// <summary>
		/// When the user clicks export, display an output filename dialog box and save
		/// when the appropriate button is pressed
		/// </summary>
		/// <param name="skin">UI Skin.</param>
		void exportPexClicked(Skin skin)
		{
			var canvas = entity.scene.createEntity( "save-dialog" ).addComponent( new UICanvas() );
			var dialog = canvas.stage.addElement( new Dialog( "Output Filename", skin ) );

			var contentTable = dialog.getContentTable();
			contentTable.add( "Filename: " ).left();
			contentTable.row();
			var outField = new TextField( "output.pex", skin );
			contentTable.add( outField ).center();

			var buttonTable = dialog.getButtonTable();
			var cancelButton = new TextButton( "Cancel", skin );
			var okButton = new TextButton( "OK", skin );

			cancelButton.onClicked += butt =>
			{
				entity.scene.findEntity( "save-dialog" ).destroy();
			};

			okButton.onClicked += butt =>
			{
				var outFilename = outField.getText();
				if ( outFilename.Length > 0 )
				{
					var exporter = new PexExporter();
					exporter.export( _particleEmitterConfig, outFilename );
				}
				entity.scene.findEntity( "save-dialog" ).destroy();
			};

			dialog.addButton( okButton );
			dialog.addButton( cancelButton );

			dialog.setMovable( true );
			dialog.setResizable( true );
			dialog.setPosition( ( Screen.width - dialog.getWidth() ) / 2f, ( Screen.height - dialog.getHeight() ) / 2f );

			var uiCanvas = entity.scene.createEntity( "particles-ui" ).addComponent( new UICanvas() );
		}
	}
}

