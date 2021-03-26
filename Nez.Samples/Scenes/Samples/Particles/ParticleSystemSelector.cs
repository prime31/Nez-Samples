using System;
using System.Collections.Generic;
using System.IO;
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
		string[] _particleConfigs;

		// the ParticleEmitter component
		ParticleEmitter _particleEmitter;

		// The currently load particle emitter configuration
		ParticleEmitterConfig _particleEmitterConfig;

		// the current emitter config index that we are playing
		int _currentParticleSystem;

		// we keep around a reference to the CheckBoxes so that we can reset its state when we change particle systems
		bool _isCollisionEnabled;
		bool _simulateInWorldSpace = true;


		public override void OnAddedToEntity()
		{
			_particleConfigs = Directory.GetFiles("Content/ParticleDesigner", "*.pex");
			if (_particleConfigs.Length > 20)
				_currentParticleSystem = 20;
			LoadParticleSystem();
			CreateUi();
		}

		void IUpdatable.Update()
		{
			// toggle the emitter config used when Q/W are pressed
			if (Input.IsKeyPressed(Keys.Q))
			{
				_currentParticleSystem = Mathf.DecrementWithWrap(_currentParticleSystem, _particleConfigs.Length);
				LoadParticleSystem();
				ResetUi();
			}

			if (Input.IsKeyPressed(Keys.W))
			{
				_currentParticleSystem = Mathf.IncrementWithWrap(_currentParticleSystem, _particleConfigs.Length);
				LoadParticleSystem();
				ResetUi();
			}
		}

		/// <summary>
		/// loads a ParticleEmitterConfig and creates a ParticleEmitter to display it
		/// </summary>
		void LoadParticleSystem()
		{
			// load up the config then add a ParticleEmitter
			_particleEmitterConfig = Entity.Scene.Content.LoadParticleEmitterConfig(_particleConfigs[_currentParticleSystem]);
			ResetEmitter();
		}

		void ResetEmitter()
		{
			// kill the ParticleEmitter if we already have one
			if (_particleEmitter != null)
				Entity.RemoveComponent(_particleEmitter);

			_particleEmitter = Entity.AddComponent(new ParticleEmitter(_particleEmitterConfig));

			// set state based on the values of our CheckBoxes
			_particleEmitter.CollisionConfig.Enabled = _isCollisionEnabled;
			_particleEmitter.SimulateInWorldSpace = _simulateInWorldSpace;
		}

		/// <summary>
		/// Resets the user interface after the particle system is changed.
		/// (Tears down the old interface and builds a new one.  This is especially important
		/// with Gravity vs Radial emitters which have different parameters to control behavior.)
		/// </summary>
		void ResetUi()
		{
			var previousUi = Entity.Scene.FindEntity("particles-ui");
			if (previousUi != null)
				previousUi.Destroy();

			CreateUi();
		}

		void CreateUi()
		{
			var uiCanvas = Entity.Scene.CreateEntity("particles-ui").AddComponent(new UICanvas());
			uiCanvas.IsFullScreen = true;
			var skin = Skin.CreateDefaultSkin();
			CondenseSkin(skin);

			// Stolen from RuntimeInspector.cs
			var table = new Table();
			table.Top().Left();
			table.Defaults().SetPadTop(4).SetPadLeft(4).SetPadRight(0).SetAlign(Align.Left);
			table.SetBackground(new PrimitiveDrawable(new Color(40, 40, 40, 220)));

			// wrap up the table in a ScrollPane
			var scrollPane = uiCanvas.Stage.AddElement(new ScrollPane(table, skin));

			// force a validate which will layout the ScrollPane and populate the proper scrollBarWidth
			scrollPane.Validate();
			scrollPane.SetSize(340 + scrollPane.GetScrollBarWidth(), Screen.Height);

			table.Row().SetPadTop(40); // Leave room for the directions

			var collisionCheckBox = table.Add(new CheckBox("Toggle Collision", skin)).GetElement<CheckBox>();
			collisionCheckBox.IsChecked = _isCollisionEnabled;
			collisionCheckBox.OnChanged += isChecked =>
			{
				_particleEmitter.CollisionConfig.Enabled = isChecked;
				_isCollisionEnabled = isChecked;
			};

			table.Row();

			var worldSpaceCheckbox = table.Add(new CheckBox("Simulate in World Space", skin)).GetElement<CheckBox>();
			worldSpaceCheckbox.IsChecked = _simulateInWorldSpace;
			worldSpaceCheckbox.OnChanged += isChecked =>
			{
				_particleEmitter.SimulateInWorldSpace = isChecked;
				_simulateInWorldSpace = isChecked;
			};

			table.Row();

			var button = table.Add(new TextButton("Toggle Play/Pause", skin)).SetFillX().SetMinHeight(30)
				.GetElement<TextButton>();
			button.OnClicked += butt =>
			{
				if (_particleEmitter.IsPlaying)
					_particleEmitter.Pause();
				else
					_particleEmitter.Play();
			};

			table.Add("");
			var save = table.Add(new TextButton("Save as .pex", skin)).SetFillX().SetMinHeight(30)
				.GetElement<TextButton>();
			save.OnClicked += butt => { ExportPexClicked(skin); };

			table.Row();
			MakeSection(table, skin, "General");
			table.Row();
			MakeEmitterDropdown(table, skin, "Emitter Type");
			table.Row();
			MakeBlendDropdown(table, skin, "Source Blend Function", "BlendFuncSource");
			table.Row();
			MakeBlendDropdown(table, skin, "Destination Blend Function", "BlendFuncDestination");

			table.Row();
			MakeSection(table, skin, "Emitter Parameters");
			table.Row();
			MakeVector2(table, skin, "Source Position Variance", "SourcePositionVariance");
			table.Row();
			MakeSlider(table, skin, "Particle Lifespan", 0, 30, 0.5f, "ParticleLifespan");
			table.Row();
			MakeSlider(table, skin, "Particle Lifespan Variance", 0, 30, 0.5f, "ParticleLifespanVariance");
			table.Row();
			MakeSlider(table, skin, "Emission rate", 0, 4000, 1, "EmissionRate");
			table.Row();
			MakeSlider(table, skin, "Duration", -1, 2000, 1, "Duration");
			table.Row();
			MakeSlider(table, skin, "Angle", 0, 360, 1, "Angle");
			table.Row();
			MakeSlider(table, skin, "Angle Variance", 0, 360, 1, "AngleVariance");
			table.Row();
			MakeSlider(table, skin, "Maximum Particles", 0, 2000, 1, "MaxParticles");
			table.Row();
			MakeSlider(table, skin, "Start Particle Size", 0, 2000, 1, "StartParticleSize");
			table.Row();
			MakeSlider(table, skin, "Start Size Variance", 0, 2000, 1, "StartParticleSizeVariance");
			table.Row();
			MakeSlider(table, skin, "Finish Particle Size", 0, 2000, 1, "FinishParticleSize");
			table.Row();
			MakeSlider(table, skin, "Finish Size Variance", 0, 2000, 1, "FinishParticleSizeVariance");
			table.Row();
			MakeSlider(table, skin, "Rotation Start", 0, 2000, 1, "RotationStart");
			table.Row();
			MakeSlider(table, skin, "Rotation Start Variance", 0, 2000, 1, "RotationStartVariance");
			table.Row();
			MakeSlider(table, skin, "Rotation End", 0, 2000, 1, "RotationEnd");
			table.Row();
			MakeSlider(table, skin, "Rotation End Variance", 0, 2000, 1, "RotationEndVariance");
			table.Row();
			MakeColor(table, skin, "Start Color (R G B A)", "StartColor");
			table.Row();
			MakeColor(table, skin, "Start Color Variance", "StartColorVariance");
			table.Row();
			MakeColor(table, skin, "Finish Color (R G B A)", "FinishColor");
			table.Row();
			MakeColor(table, skin, "Finish Color Variance", "FinishColorVariance");


			if (_particleEmitterConfig.EmitterType == ParticleEmitterType.Gravity)
			{
				table.Row();
				MakeSection(table, skin, "Gravity Emitter");
				table.Row();
				MakeVector2(table, skin, "Gravity X/Y", "Gravity");
				table.Row();
				MakeSlider(table, skin, "Speed", 0, 2000, 1, "Speed");
				table.Row();
				MakeSlider(table, skin, "Speed Variance", 0, 2000, 1, "SpeedVariance");
				table.Row();
				MakeSlider(table, skin, "Radial Acceleration", -2000, 2000, 1, "RadialAcceleration");
				table.Row();
				MakeSlider(table, skin, "Radial Accel Variance", 0, 2000, 1, "RadialAccelVariance");
				table.Row();
				MakeSlider(table, skin, "Tangential Acceleration", -2000, 2000, 1, "TangentialAcceleration");
				table.Row();
				MakeSlider(table, skin, "Tangential Accel Variance", 0, 2000, 1, "TangentialAccelVariance");
			}
			else
			{
				table.Row();
				MakeSection(table, skin, "Radial Emitter");
				table.Row();
				MakeSlider(table, skin, "Maximum Radius", 0, 1000, 1, "MaxRadius");
				table.Row();
				MakeSlider(table, skin, "Maximum Radius Variance", 0, 1000, 1, "MaxRadiusVariance");
				table.Row();
				MakeSlider(table, skin, "Minimum Radius", 0, 1000, 1, "MinRadius");
				table.Row();
				MakeSlider(table, skin, "Minimum Radius Variance", 0, 1000, 1, "MinRadiusVariance");
				table.Row();
				MakeSlider(table, skin, "Rotation/Sec", 0, 1000, 1, "RotatePerSecond");
				table.Row();
				MakeSlider(table, skin, "Rotation/Sec Variance", 0, 1000, 1, "RotatePerSecondVariance");
			}
		}

		void MakeSlider(Table table, Skin skin, string label, float min, float max, float step, string propertyName)
		{
			FieldInfo fieldInfo = typeof(ParticleEmitterConfig).GetField(propertyName);
			float value = Convert.ToSingle(fieldInfo.GetValue(_particleEmitterConfig));

			var slider = new Slider(skin, null, min, max);
			var textBox = new TextField(value.ToString(), skin);

			slider.SetStepSize(step);
			slider.SetValue(value);
			slider.OnChanged += newValue =>
			{
				fieldInfo.SetValue(_particleEmitterConfig, TypedConvert(fieldInfo, newValue));
				textBox.SetText(newValue.ToString());
				ResetEmitter();
			};

			textBox.SetMaxLength(5);
			textBox.OnTextChanged += (field, str) =>
			{
				if (float.TryParse(str, out float newValue))
				{
					fieldInfo.SetValue(_particleEmitterConfig, TypedConvert(fieldInfo, newValue));
					slider.SetValue(newValue);
					ResetEmitter();
				}
			};

			table.Add(label).Left().Width(140);
			table.Add(textBox).Left().Width(30);
			table.Add(slider).Left();
		}

		void MakeBlendDropdown(Table table, Skin skin, string label, string propertyName)
		{
			FieldInfo fieldInfo = typeof(ParticleEmitterConfig).GetField(propertyName);
			Blend value = (Blend) fieldInfo.GetValue(_particleEmitterConfig);

			var dropdown = new SelectBox<string>(skin);
			var dropdownList = new List<string>()
			{
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
			dropdown.SetItems(dropdownList);

			// Make a lookup table from string to blend function
			var nameLookup = new Dictionary<string, Blend>()
			{
				{"Zero", Blend.Zero},
				{"One", Blend.One},
				{"SourceColor", Blend.SourceColor},
				{"InverseSourceColor", Blend.InverseSourceColor},
				{"SourceAlpha", Blend.SourceAlpha},
				{"InverseSourceAlpha", Blend.InverseSourceAlpha},
				{"DestinationAlpha", Blend.DestinationAlpha},
				{"InverseDestinationAlpha", Blend.InverseDestinationAlpha},
				{"DestinationColor", Blend.DestinationColor},
				{"InverseDestinationColor", Blend.InverseDestinationColor},
				{"SourceAlphaSaturation", Blend.SourceAlphaSaturation}
			};

			// Make a lookup table from blend function to string
			var functionLookup = new Dictionary<Blend, string>();
			foreach (var str in nameLookup.Keys)
			{
				functionLookup.Add(nameLookup[str], str);
			}

			;

			dropdown.SetSelectedIndex(dropdownList.IndexOf(functionLookup[value]));
			dropdown.OnChanged += (str) =>
			{
				Blend newValue = nameLookup[str];
				fieldInfo.SetValue(_particleEmitterConfig, newValue);
				ResetEmitter();
			};

			table.Add(dropdown);
		}

		void MakeEmitterDropdown(Table table, Skin skin, string label)
		{
			ParticleEmitterType value = _particleEmitterConfig.EmitterType;

			var dropdown = new SelectBox<string>(skin);
			var dropdownList = new List<string>()
			{
				"Gravity",
				"Radial"
			};

			dropdown.SetItems(dropdownList);

			if (_particleEmitterConfig.EmitterType == ParticleEmitterType.Gravity)
				dropdown.SetSelectedIndex(0);
			else
				dropdown.SetSelectedIndex(1);

			dropdown.OnChanged += (str) =>
			{
				if (str == "Gravity")
					_particleEmitterConfig.EmitterType = ParticleEmitterType.Gravity;
				else
					_particleEmitterConfig.EmitterType = ParticleEmitterType.Radial;
				ResetEmitter();
				ResetUi();
			};

			table.Add(label).Left().Width(140);
			table.Add("").Width(1); // This is a 3 column table
			table.Add(dropdown);
		}

		void MakeVector2(Table table, Skin skin, string label, string propertyName)
		{
			var fieldInfo = typeof(ParticleEmitterConfig).GetField(propertyName);
			var value = (Vector2) fieldInfo.GetValue(_particleEmitterConfig);
			var x = new TextField(value.X.ToString(), skin);
			var y = new TextField(value.Y.ToString(), skin);

			x.OnTextChanged += (textbox, str) =>
			{
				if (float.TryParse(str, out float newValue))
				{
					value.X = newValue;
					fieldInfo.SetValue(_particleEmitterConfig, value);
					ResetEmitter();
				}
			};

			y.OnTextChanged += (textbox, str) =>
			{
				if (float.TryParse(str, out float newValue))
				{
					value.Y = newValue;
					fieldInfo.SetValue(_particleEmitterConfig, value);
					ResetEmitter();
				}
			};

			table.Add(label).Width(140);
			table.Add(x).Width(30);
			table.Add(y).Width(30);
		}

		void MakeColor(Table table, Skin skin, string label, string propertyName)
		{
			FieldInfo fieldInfo = typeof(ParticleEmitterConfig).GetField(propertyName);
			Color value = (Color) fieldInfo.GetValue(_particleEmitterConfig);

			var r = new TextField(value.R.ToString(), skin).SetMaxLength(4);
			var g = new TextField(value.G.ToString(), skin).SetMaxLength(4);
			var b = new TextField(value.B.ToString(), skin).SetMaxLength(4);
			var a = new TextField(value.A.ToString(), skin).SetMaxLength(4);

			void OnChanged(TextField box, string str)
			{
				if (int.TryParse(r.GetText(), out int newR)
				    && int.TryParse(g.GetText(), out int newG)
				    && int.TryParse(b.GetText(), out int newB)
				    && int.TryParse(a.GetText(), out int newA)
				    && newR >= 0 && newR <= 255
				    && newB >= 0 && newB <= 255
				    && newG >= 0 && newG <= 255
				    && newR >= 0 && newA <= 255)
				{
					var newColor = new Color(newR, newG, newB, newA);
					fieldInfo.SetValue(_particleEmitterConfig, newColor);
				}
			}

			r.OnTextChanged += OnChanged;
			b.OnTextChanged += OnChanged;
			g.OnTextChanged += OnChanged;
			a.OnTextChanged += OnChanged;

			table.Add(label).Width(140);
			table.Add(r).Width(30);

			// Everything else is dealing with 3 columns.  Wrap the last ones up in
			// an hbox to avoid creating new columns in the table. (hack)
			g.SetPreferredWidth(30);
			b.SetPreferredWidth(30);
			a.SetPreferredWidth(30);
			var hbox = new HorizontalGroup().SetSpacing(10);
			hbox.AddElement(g);
			hbox.AddElement(b);
			hbox.AddElement(a);
			table.Add(hbox).Left().Top().Width(30);
		}

		void MakeSection(Table table, Skin skin, string sectionName)
		{
			var label = new Label(sectionName, skin);
			label.SetFontColor(new Color(241, 156, 0));
			table.Add(label).SetPadTop(20);
		}


		/// <summary>
		/// Uses relfection to convert an object's property into the appropriate type
		/// </summary>
		/// <returns>A converted object in the propert type</returns>
		object TypedConvert(FieldInfo fieldInfo, object value)
		{
			if (fieldInfo.FieldType == typeof(float))
				return (float) value;

			if (fieldInfo.FieldType == typeof(int))
				return (int) Convert.ToSingle(value);

			if (fieldInfo.FieldType == typeof(uint))
				return (uint) Convert.ToSingle(value);

			return Convert.ToString(value);
		}

		void CondenseSkin(Skin skin)
		{
			// This is ripped from the inspector

			// modify some of the default styles to better suit our needs
			var tfs = skin.Get<TextFieldStyle>();
			tfs.Background.LeftWidth = tfs.Background.RightWidth = 4;
			tfs.Background.BottomHeight = 0;
			tfs.Background.TopHeight = 3;

			var checkbox = skin.Get<CheckBoxStyle>();
			checkbox.CheckboxOn.MinWidth = checkbox.CheckboxOn.MinHeight = 15;
			checkbox.CheckboxOff.MinWidth = checkbox.CheckboxOff.MinHeight = 15;
			checkbox.CheckboxOver.MinWidth = checkbox.CheckboxOver.MinHeight = 15;
		}

		/// <summary>
		/// When the user clicks export, display an output filename dialog box and save
		/// when the appropriate button is pressed
		/// </summary>
		/// <param name="skin">UI Skin.</param>
		void ExportPexClicked(Skin skin)
		{
			var canvas = Entity.Scene.CreateEntity("save-dialog").AddComponent(new UICanvas());
			var dialog = canvas.Stage.AddElement(new Dialog("Output Filename", skin));

			var contentTable = dialog.GetContentTable();
			contentTable.Add("Filename: ").Left();
			contentTable.Row();
			var outField = new TextField("output.pex", skin);
			contentTable.Add(outField).Center();

			var buttonTable = dialog.GetButtonTable();
			var cancelButton = new TextButton("Cancel", skin);
			var okButton = new TextButton("OK", skin);

			cancelButton.OnClicked += butt => { Entity.Scene.FindEntity("save-dialog").Destroy(); };

			okButton.OnClicked += butt =>
			{
				var outFilename = outField.GetText();
				if (outFilename.Length > 0)
				{
					var exporter = new PexExporter();
					exporter.Export(_particleEmitterConfig, outFilename);
				}

				Entity.Scene.FindEntity("save-dialog").Destroy();
			};

			dialog.AddButton(okButton);
			dialog.AddButton(cancelButton);

			dialog.SetMovable(true);
			dialog.SetResizable(true);
			dialog.SetPosition((Screen.Width - dialog.GetWidth()) / 2f, (Screen.Height - dialog.GetHeight()) / 2f);

			var uiCanvas = Entity.Scene.CreateEntity("particles-ui").AddComponent(new UICanvas());
		}
	}
}