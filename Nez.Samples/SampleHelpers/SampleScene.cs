using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Nez.UI;
using Microsoft.Xna.Framework.Graphics;
using Nez.Tweens;
using System.Linq;
using Nez.ImGuiTools;
using Nez.Console;

namespace Nez.Samples
{
	/// <summary>
	/// this entire class is one big sweet hack job to make adding samples easier. An exceptional hack is made so that we can render small
	/// pixel art scenes pixel perfect and still display our UI at a reasonable size.
	/// </summary>
	public abstract class SampleScene : Scene, IFinalRenderDelegate
	{
		public const int ScreenSpaceRenderLayer = 999;
		public UICanvas Canvas;

		Table _table;
		List<Button> _sceneButtons = new List<Button>();
		ScreenSpaceRenderer _screenSpaceRenderer;
		static bool _needsFullRenderSizeForUi;


		public SampleScene(bool addExcludeRenderer = true, bool needsFullRenderSizeForUi = false)
		{
			_needsFullRenderSizeForUi = needsFullRenderSizeForUi;

			// setup one renderer in screen space for the UI and then (optionally) another renderer to render everything else
			if (needsFullRenderSizeForUi)
			{
				// dont actually add the renderer since we will manually call it later
				_screenSpaceRenderer = new ScreenSpaceRenderer(100, ScreenSpaceRenderLayer);
				_screenSpaceRenderer.ShouldDebugRender = false;
				FinalRenderDelegate = this;
			}
			else
			{
				AddRenderer(new ScreenSpaceRenderer(100, ScreenSpaceRenderLayer));
			}

			if (addExcludeRenderer)
				AddRenderer(new RenderLayerExcludeRenderer(0, ScreenSpaceRenderLayer));

			// create our canvas and put it on the screen space render layer
			Canvas = CreateEntity("ui").AddComponent(new UICanvas());
			Canvas.IsFullScreen = true;
			Canvas.RenderLayer = ScreenSpaceRenderLayer;
			SetupSceneSelector();
		}


		IEnumerable<Type> GetTypesWithSampleSceneAttribute()
		{
			var assembly = typeof(SampleScene).Assembly;
			var scenes = assembly.GetTypes()
				.Where(t => t.GetCustomAttributes(typeof(SampleSceneAttribute), true).Length > 0)
				.OrderBy(t =>
					((SampleSceneAttribute) t.GetCustomAttributes(typeof(SampleSceneAttribute), true)[0]).Order);

			foreach (var s in scenes)
				yield return s;
		}

		void SetupSceneSelector()
		{
			_table = Canvas.Stage.AddElement(new Table());
			_table.SetFillParent(true).Right().Top();

			var topButtonStyle = new TextButtonStyle(new PrimitiveDrawable(Color.Black, 10f),
				new PrimitiveDrawable(Color.Yellow), new PrimitiveDrawable(Color.DarkSlateBlue))
			{
				DownFontColor = Color.Black
			};
			_table.Add(new TextButton("Toggle Scene List", topButtonStyle)).SetFillX().SetMinHeight(30)
				.GetElement<Button>().OnClicked += OnToggleSceneListClicked;

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
			foreach (var type in GetTypesWithSampleSceneAttribute())
			{
				foreach (var attr in type.GetCustomAttributes(true))
				{
					if (attr.GetType() == typeof(SampleSceneAttribute))
					{
						var sampleAttr = attr as SampleSceneAttribute;
						var button = _table.Add(new TextButton(sampleAttr.ButtonName, buttonStyle)).SetFillX()
							.SetMinHeight(30).GetElement<TextButton>();
						_sceneButtons.Add(button);
						button.OnClicked += butt =>
						{
							// stop all tweens in case any demo scene started some up
							TweenManager.StopAllTweens();
							Core.GetGlobalManager<ImGuiManager>()?.SetEnabled(false);
							Core.StartSceneTransition(new FadeTransition(() => Activator.CreateInstance(type) as Scene));
						};

						_table.Row().SetPadTop(10);

						// optionally add instruction text for the current scene
						if (sampleAttr.InstructionText != null && type == GetType())
							AddInstructionText(sampleAttr.InstructionText);
					}
				}
			}
		}

		void AddInstructionText(string text)
		{
			var instructionsEntity = CreateEntity("instructions");
			instructionsEntity
				.AddComponent(new TextComponent(Graphics.Instance.BitmapFont, text, new Vector2(10, 10), Color.White))
				.SetRenderLayer(ScreenSpaceRenderLayer);
		}

		void OnToggleSceneListClicked(Button butt)
		{
			foreach (var button in _sceneButtons)
				button.SetIsVisible(!button.IsVisible());
		}

		[Console.Command("toggle-imgui", "Toggles the Dear ImGui renderer")]
		static void ToggleImGui()
		{
			if (_needsFullRenderSizeForUi)
			{
				DebugConsole.Instance.Log("Error: due to the way the sample scenes are assembled, only full screen sample scenes can use ImGui");
				return;
			}

			// install the service if it isnt already there
			var service = Core.GetGlobalManager<ImGuiManager>();
			if (service == null)
			{
				service = new ImGuiManager();
				Core.RegisterGlobalManager(service);
			}
			else
			{
				service.SetEnabled(!service.Enabled);
			}
		}

		#region IFinalRenderDelegate

		private Scene _scene;

		public void OnAddedToScene(Scene scene) => _scene = scene;

		public void OnSceneBackBufferSizeChanged(int newWidth, int newHeight) => _screenSpaceRenderer.OnSceneBackBufferSizeChanged(newWidth, newHeight);

		public void HandleFinalRender(RenderTarget2D finalRenderTarget, Color letterboxColor, RenderTarget2D source,
		                              Rectangle finalRenderDestinationRect, SamplerState samplerState)
		{
			Core.GraphicsDevice.SetRenderTarget(null);
			Core.GraphicsDevice.Clear(letterboxColor);
			Graphics.Instance.Batcher.Begin(BlendState.Opaque, samplerState, DepthStencilState.None, RasterizerState.CullNone, null);
			Graphics.Instance.Batcher.Draw(source, finalRenderDestinationRect, Color.White);
			Graphics.Instance.Batcher.End();

			_screenSpaceRenderer.Render(_scene);
		}

		#endregion
	}


	[AttributeUsage(AttributeTargets.Class)]
	public class SampleSceneAttribute : Attribute
	{
		public string ButtonName;
		public int Order;
		public string InstructionText;


		public SampleSceneAttribute(string buttonName, int order, string instructionText = null)
		{
			ButtonName = buttonName;
			Order = order;
			InstructionText = instructionText;
		}
	}
}