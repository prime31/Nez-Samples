using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Nez.Sprites;

namespace Nez.Samples
{
	public class LiveLightController : Component, IUpdatable
	{
		SpriteRenderer _spriteRenderer;

		public override void OnAddedToEntity()
		{
			_spriteRenderer = Entity.GetComponent<SpriteRenderer>();
		}

		public void Update()
		{
			Entity.SetPosition(Input.MousePosition);

			if (Input.IsKeyDown(Keys.W))
				Entity.Scale += new Vector2(0.2f);
			else if (Input.IsKeyDown(Keys.S))
				Entity.Scale -= new Vector2(0.2f);
			Entity.Scale = Vector2.Clamp(Entity.Scale, new Vector2(0.2f), new Vector2(30));

			if (Input.IsKeyDown(Keys.A))
			{
				var (h, s, l) = ColorExt.RgbToHsl(_spriteRenderer.Color);
				_spriteRenderer.Color = ColorExt.HslToRgb((h - 0.002f) % 360, s, l);
			}
			else if (Input.IsKeyDown(Keys.D))
			{
				var (h, s, l) = ColorExt.RgbToHsl(_spriteRenderer.Color);
				_spriteRenderer.Color = ColorExt.HslToRgb((h + 0.002f) % 360, s, l);
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
