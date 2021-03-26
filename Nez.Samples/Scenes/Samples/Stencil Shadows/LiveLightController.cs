using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Nez.Samples
{
	/// <summary>
	/// this Component lets you control a StencilLight with WASD keys and click to clone it
	/// </summary>
	public class LiveLightController : Component, IUpdatable
	{
		StencilLight _light;

		public override void OnAddedToEntity()
		{
			_light = Entity.GetComponent<StencilLight>();
		}

		public void Update()
		{
			Entity.SetPosition(Input.MousePosition);

			if (Input.IsKeyDown(Keys.W))
				_light.Radius += 10;
			else if (Input.IsKeyDown(Keys.S))
				_light.Radius -= 10;
			Entity.Scale = Vector2.Clamp(Entity.Scale, new Vector2(0.2f), new Vector2(30));

			if (Input.IsKeyDown(Keys.A))
			{
				var (h, s, l) = ColorExt.RgbToHsl(_light.Color);
				_light.Color = ColorExt.HslToRgb((h - 0.002f) % 360, s, l);
			}
			else if (Input.IsKeyDown(Keys.D))
			{
				var (h, s, l) = ColorExt.RgbToHsl(_light.Color);
				_light.Color = ColorExt.HslToRgb((h + 0.002f) % 360, s, l);
			}

			if (Input.LeftMouseButtonPressed)
			{
				var clone = Entity.Clone(Entity.Position);
				clone.RemoveComponent<LiveLightController>();
				Entity.Scene.AddEntity(clone);
			}
		}
	}
}
