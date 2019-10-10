using System;
using Microsoft.Xna.Framework;
using Nez.Textures;
using Microsoft.Xna.Framework.Graphics;
using Nez.Sprites;
using Nez.Tweens;
using Nez.Shadows;


namespace Nez.Samples
{
	[SampleScene("Mesh Shadows", 40, "2D shadow system\nArrow keys to move")]
	public class MeshShadowsScene : SampleScene
	{
		public override void Initialize()
		{
			base.Initialize();

			Screen.SetSize(1280, 720);

			// render layer for all lights and any emissive Sprites
			var lightRenderLayer = 5;
			ClearColor = Color.White;

			// create a Renderer that renders all but the light layer and screen space layer
			AddRenderer(new RenderLayerExcludeRenderer(0, lightRenderLayer, ScreenSpaceRenderLayer));

			// create a Renderer that renders only the light layer into a render target
			var lightRenderer = AddRenderer(new RenderLayerRenderer(-1, lightRenderLayer));
			lightRenderer.RenderTargetClearColor = new Color(10, 10, 10, 255);
			lightRenderer.RenderTexture = new RenderTexture();

			// add a PostProcessor that renders the light render target and blurs it
			AddPostProcessor(new PolyLightPostProcessor(0, lightRenderer.RenderTexture))
				.SetEnableBlur(true)
				.SetBlurAmount(0.5f);

			var lightTexture = Content.Load<Texture2D>(Nez.Content.Shadows.Spritelight);
			var moonTexture = Content.Load<Texture2D>(Nez.Content.Shared.Moon);
			var blockTexture = Content.Load<Texture2D>(Nez.Content.Shadows.Block);
			var blockGlowTexture = Content.Load<Texture2D>(Nez.Content.Shadows.BlockGlow);

			// create some boxes
			Action<Vector2, string, bool> boxMaker = (Vector2 pos, string name, bool isTrigger) =>
			{
				var ent = CreateEntity(name);
				ent.Position = pos;
				ent.AddComponent(new SpriteRenderer(blockTexture));
				var collider = ent.AddComponent<BoxCollider>();

				// add a glow sprite on the light render layer
				var glowSprite = new SpriteRenderer(blockGlowTexture);
				glowSprite.RenderLayer = lightRenderLayer;
				ent.AddComponent(glowSprite);

				if (isTrigger)
				{
					collider.IsTrigger = true;
					ent.AddComponent(new TriggerListener());
				}
			};

			boxMaker(new Vector2(0, 100), "box0", false);
			boxMaker(new Vector2(150, 100), "box1", false);
			boxMaker(new Vector2(300, 100), "box2", false);
			boxMaker(new Vector2(450, 100), "box3", false);
			boxMaker(new Vector2(600, 100), "box4", false);

			boxMaker(new Vector2(50, 500), "box5", true);
			boxMaker(new Vector2(500, 250), "box6", false);

			var moonEnt = CreateEntity("moon");
			moonEnt.AddComponent(new SpriteRenderer(moonTexture));
			moonEnt.Position = new Vector2(100, 0);

			moonEnt = CreateEntity("moon2");
			moonEnt.AddComponent(new SpriteRenderer(moonTexture));
			moonEnt.Position = new Vector2(-500, 0);


			var lightEnt = CreateEntity("sprite-light");
			lightEnt.AddComponent(new SpriteRenderer(lightTexture));
			lightEnt.Position = new Vector2(-700, 0);
			lightEnt.Scale = new Vector2(4);
			lightEnt.GetComponent<SpriteRenderer>().RenderLayer = lightRenderLayer;


			// add an animation to "box4"
			FindEntity("box4").AddComponent(new MovingPlatform(250, 400));

			// create a player block
			var entity = CreateEntity("player-block");
			entity.SetPosition(new Vector2(220, 220))
				.AddComponent(new SpriteRenderer(blockTexture).SetRenderLayer(lightRenderLayer))
				.AddComponent(new SimpleMover())
				.AddComponent<BoxCollider>();


			// add a follow camera
			Camera.Entity.AddComponent(new FollowCamera(entity));
			Camera.Entity.AddComponent(new CameraShake());


			// setup some lights and animate the colors
			var pointLight = new PolyLight(600, Color.Red);
			pointLight.RenderLayer = lightRenderLayer;
			pointLight.Power = 1f;

			var light = CreateEntity("light");
			light.SetPosition(new Vector2(700f, 300f))
				.AddComponent(pointLight);

			pointLight.TweenColorTo(new Color(0, 0, 255, 255), 1f)
				.SetEaseType(EaseType.Linear)
				.SetLoops(LoopType.PingPong, 100)
				.Start();

			PropertyTweens.FloatPropertyTo(pointLight, "Power", 0.1f, 1f)
				.SetEaseType(EaseType.Linear)
				.SetLoops(LoopType.PingPong, 100)
				.Start();


			pointLight = new PolyLight(500, Color.Yellow);
			pointLight.RenderLayer = lightRenderLayer;
			light = CreateEntity("light-two");
			light.Position = new Vector2(-50f);
			light.AddComponent(pointLight);

			pointLight.TweenColorTo(new Color(0, 255, 0, 255), 1f)
				.SetEaseType(EaseType.Linear)
				.SetLoops(LoopType.PingPong, 100)
				.Start();


			pointLight = new PolyLight(500, Color.AliceBlue);
			pointLight.RenderLayer = lightRenderLayer;
			light = CreateEntity("light-three");
			light.Position = new Vector2(100, 250);
			light.AddComponent(pointLight);

			pointLight.TweenColorTo(new Color(200, 100, 155, 255), 1f)
				.SetEaseType(EaseType.QuadIn)
				.SetLoops(LoopType.PingPong, 100)
				.Start();
		}
	}
}