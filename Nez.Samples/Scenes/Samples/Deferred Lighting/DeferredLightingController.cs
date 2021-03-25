using System;
using Nez.DeferredLighting;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;


namespace Nez.Samples
{
	public class DeferredLightingController : Component, IUpdatable
	{
		DeferredLight _currentLight;
		DirLight _dirLight;


		public override void OnAddedToEntity()
		{
			_dirLight = Entity.Scene.FindComponentOfType<DirLight>();
			_currentLight = _dirLight;
		}


		public void Update()
		{
			// check for debug toggle
			if (Input.IsKeyPressed(Keys.F))
			{
				var renderer = Entity.Scene.GetRenderer<DeferredLightingRenderer>();
				renderer.EnableDebugBufferRender = !renderer.EnableDebugBufferRender;
			}

			// check for light changes
			var lightIndex = -1;
			if (Input.IsKeyPressed(Keys.D1))
				lightIndex = 0;
			if (Input.IsKeyPressed(Keys.D2))
				lightIndex = 1;
			if (Input.IsKeyPressed(Keys.D3))
				lightIndex = 2;
			if (Input.IsKeyPressed(Keys.D4))
				lightIndex = 3;
			if (Input.IsKeyPressed(Keys.D5))
				lightIndex = 4;
			if (Input.IsKeyPressed(Keys.D6))
				lightIndex = 5;

			if (lightIndex > -1)
			{
				var lights = Entity.Scene.FindComponentsOfType<DeferredLight>();
				if (lights.Count > lightIndex)
				{
					_currentLight = lights[lightIndex];
					UpdateInstructions();
				}
				else
				{
					Debug.Log("no light at index: {0}", lightIndex);
				}
			}

			CheckInput();
		}


		void CheckInput()
		{
			if (_currentLight is DirLight)
			{
				var light = _currentLight as DirLight;
				var zDir = light.Direction.Z;
				var rotation = (float) Math.Atan2(light.Direction.Y, light.Direction.X);
				var magnitude = new Vector2(light.Direction.X, light.Direction.Y).Length();

				if (Input.IsKeyDown(Keys.Left))
					rotation -= 0.1f;
				else if (Input.IsKeyDown(Keys.Right))
					rotation += 0.1f;

				if (Input.IsKeyDown(Keys.Up))
					zDir = Mathf.Clamp(zDir + 2, 0, 400);
				else if (Input.IsKeyDown(Keys.Down))
					zDir = Mathf.Clamp(zDir - 2, 0, 400);

				var newDir = new Vector3(Mathf.Cos(rotation) * magnitude, Mathf.Sin(rotation) * magnitude, zDir);
				light.SetDirection(newDir);
			}

			if (_currentLight is PointLight || _currentLight is SpotLight)
			{
				var light = _currentLight as PointLight;

				if (Input.IsKeyDown(Keys.Up))
					light.SetRadius(Mathf.Clamp(light.Radius + 5, 10, 800));
				else if (Input.IsKeyDown(Keys.Down))
					light.SetRadius(Mathf.Clamp(light.Radius - 5, 10, 800));

				if (Input.IsKeyDown(Keys.Left))
					light.SetIntensity(Mathf.Clamp(light.Intensity - 0.1f, 0, 20));
				else if (Input.IsKeyDown(Keys.Right))
					light.SetIntensity(Mathf.Clamp(light.Intensity + 0.1f, 0, 20));

				if (Input.IsKeyDown(Keys.W))
					light.SetZPosition(Mathf.Clamp(light.ZPosition + 1, 0, 300));
				else if (Input.IsKeyDown(Keys.S))
					light.SetZPosition(Mathf.Clamp(light.ZPosition - 1, 0, 300));
			}

			if (_currentLight is SpotLight)
			{
				var light = _currentLight as SpotLight;

				if (Input.IsKeyDown(Keys.A))
					light.SetConeAngle(Mathf.Repeat(light.ConeAngle - 3, 360));
				else if (Input.IsKeyDown(Keys.D))
					light.SetConeAngle(Mathf.Repeat(light.ConeAngle + 3, 360));

				if (Input.IsKeyDown(Keys.Z))
					light.Entity.SetLocalRotationDegrees(Mathf.Repeat(light.Entity.RotationDegrees - 3, 360));
				else if (Input.IsKeyDown(Keys.X))
					light.Entity.SetLocalRotationDegrees(Mathf.Repeat(light.Entity.RotationDegrees + 3, 360));
			}

			// color controls
			var color = _currentLight.Color;
			if (Input.IsKeyDown(Keys.R))
				color.R += (byte) 2;
			if (Input.IsKeyDown(Keys.G))
				color.G += (byte) 2;
			if (Input.IsKeyDown(Keys.B))
				color.B += (byte) 2;

			if (color != _currentLight.Color)
			{
				_currentLight.Color = color;
				Debug.Log("new light color: {0}", _currentLight.Color);
			}
		}


		void UpdateInstructions()
		{
			var textComp = Entity.Scene.FindEntity("instructions").GetComponent<TextComponent>();
			var colorText = "\nr/g/b keys change color";

			if (_currentLight is DirLight)
				textComp.Text =
					"Controlling DirLight\nleft/right changes rotation\nup/down changes z-component of direction" +
					colorText;
			else if (_currentLight is SpotLight)
				textComp.Text =
					"Controlling SpotLight\nup/down changes radius\nleft/right changes intensity\nw/s changes zPosition\na/d changes cone angle\nz/x changes rotation" +
					colorText;
			else if (_currentLight is PointLight)
				textComp.Text =
					"Controlling PointLight\nup/down changes radius\nleft/right changes intensity\nw/s changes zPosition" +
					colorText;
		}
	}
}