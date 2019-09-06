using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Nez.UI;
using Microsoft.Xna.Framework.Graphics;
using Nez.Tweens;
using System.Linq;


namespace Nez.Samples
{
	/// <summary>
	/// this entire class is one big sweet hack job to make adding samples easier. An exceptional hack is made so that we can render small
	/// pixel art scenes pixel perfect and still display our UI at a reasonable size.
	/// </summary>
	public abstract class SampleScene : Scene, IFinalRenderDelegate
	{
		public const int SCREEN_SPACE_RENDER_LAYER = 999;
		public UICanvas canvas;

		Table _table;
		List<Button> _sceneButtons = new List<Button>();
		ScreenSpaceRenderer _screenSpaceRenderer;


		public SampleScene(bool addExcludeRenderer = true, bool needsFullRenderSizeForUI = false)
		{
			// setup one renderer in screen space for the UI and then (optionally) another renderer to render everything else
			if (needsFullRenderSizeForUI)
			{
				// dont actually add the renderer since we will manually call it later
				_screenSpaceRenderer = new ScreenSpaceRenderer(100, SCREEN_SPACE_RENDER_LAYER);
				_screenSpaceRenderer.ShouldDebugRender = false;
				FinalRenderDelegate = this;
			}
			else
			{
				AddRenderer(new ScreenSpaceRenderer(100, SCREEN_SPACE_RENDER_LAYER));
			}

			if (addExcludeRenderer)
				AddRenderer(new RenderLayerExcludeRenderer(0, SCREEN_SPACE_RENDER_LAYER));

			// create our canvas and put it on the screen space render layer
			canvas = CreateEntity("ui").AddComponent(new UICanvas());
			canvas.IsFullScreen = true;
			canvas.RenderLayer = SCREEN_SPACE_RENDER_LAYER;
			setupSceneSelector();
		}


		IEnumerable<Type> getTypesWithSampleSceneAttribute()
		{
			var assembly = typeof(SampleScene).Assembly;
			var scenes = assembly.GetTypes()
				.Where(t => t.GetCustomAttributes(typeof(SampleSceneAttribute), true).Length > 0)
				.OrderBy(t =>
					((SampleSceneAttribute) t.GetCustomAttributes(typeof(SampleSceneAttribute), true)[0]).order);

			foreach (var s in scenes)
				yield return s;
		}


		void setupSceneSelector()
		{
			_table = canvas.Stage.AddElement(new Table());
			_table.SetFillParent(true).Right().Top();

			var topButtonStyle = new TextButtonStyle(new PrimitiveDrawable(Color.Black, 10f),
				new PrimitiveDrawable(Color.Yellow), new PrimitiveDrawable(Color.DarkSlateBlue))
			{
				DownFontColor = Color.Black
			};
			_table.Add(new TextButton("Toggle Scene List", topButtonStyle)).SetFillX().SetMinHeight(30)
				.GetElement<Button>().OnClicked += onToggleSceneListClicked;

			_table.Row().SetPadTop(10);
			var checkbox = _table.Add(new CheckBox("Debug Render", new CheckBoxStyle
			{
				CheckboxOn = new PrimitiveDrawable(30, Color.Green),
				CheckboxOff = new PrimitiveDrawable(30, new Color(0x00, 0x3c, 0xe7, 0xff))
			})).GetElement<CheckBox>();
			checkbox.OnChanged += enabled => Core.DebugRenderEnabled = enabled;
			checkbox.IsChecked = Core.DebugRenderEnabled;
			_table.Row().SetPadTop(30);

			var buttonStyle = new TextButtonStyle(new PrimitiveDrawable(new Color(78, 91, 98), 10f),
				new PrimitiveDrawable(new Color(244, 23, 135)), new PrimitiveDrawable(new Color(168, 207, 115)))
			{
				DownFontColor = Color.Black
			};

			// find every Scene with the SampleSceneAttribute and create a button for each one
			foreach (var type in getTypesWithSampleSceneAttribute())
			{
				foreach (var attr in type.GetCustomAttributes(true))
				{
					if (attr.GetType() == typeof(SampleSceneAttribute))
					{
						var sampleAttr = attr as SampleSceneAttribute;
						var button = _table.Add(new TextButton(sampleAttr.buttonName, buttonStyle)).SetFillX()
							.SetMinHeight(30).GetElement<TextButton>();
						_sceneButtons.Add(button);
						button.OnClicked += butt =>
						{
							// stop all tweens in case any demo scene started some up
							TweenManager.StopAllTweens();
							Core.StartSceneTransition(new FadeTransition(() =>
								Activator.CreateInstance(type) as Scene));
						};

						_table.Row().SetPadTop(10);

						// optionally add instruction text for the current scene
						if (sampleAttr.instructionText != null && type == GetType())
							addInstructionText(sampleAttr.instructionText);
					}
				}
			}
		}


		void addInstructionText(string text)
		{
			var instructionsEntity = CreateEntity("instructions");
			instructionsEntity
				.AddComponent(new TextField(Graphics.Instance.BitmapFont, text, new Vector2(10, 10), Color.White))
				.SetRenderLayer(SCREEN_SPACE_RENDER_LAYER);
		}


		void onToggleSceneListClicked(Button butt)
		{
			foreach (var button in _sceneButtons)
				button.SetIsVisible(!button.IsVisible());
		}


		#region IFinalRenderDelegate

		public Scene Scene { get; set; }

		public void OnAddedToScene(Scene scene)
		{
		}


		public void OnSceneBackBufferSizeChanged(int newWidth, int newHeight)
		{
			_screenSpaceRenderer.OnSceneBackBufferSizeChanged(newWidth, newHeight);
		}


		public void HandleFinalRender(RenderTarget2D finalRenderTarget, Color letterboxColor, RenderTarget2D source,
		                              Rectangle finalRenderDestinationRect, SamplerState samplerState)
		{
			Core.GraphicsDevice.SetRenderTarget(null);
			Core.GraphicsDevice.Clear(letterboxColor);
			Graphics.Instance.Batcher.Begin(BlendState.Opaque, samplerState, DepthStencilState.None,
				RasterizerState.CullNone, null);
			Graphics.Instance.Batcher.Draw(source, finalRenderDestinationRect, Color.White);
			Graphics.Instance.Batcher.End();

			_screenSpaceRenderer.Render(Scene);
		}

		#endregion
	}


	[AttributeUsage(AttributeTargets.Class)]
	public class SampleSceneAttribute : Attribute
	{
		public string buttonName;
		public int order;
		public string instructionText;


		public SampleSceneAttribute(string buttonName, int order, string instructionText = null)
		{
			this.buttonName = buttonName;
			this.order = order;
			this.instructionText = instructionText;
		}
	}
}