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


		public SampleScene( bool addExcludeRenderer = true, bool needsFullRenderSizeForUI = false )
		{
			// setup one renderer in screen space for the UI and then (optionally) another renderer to render everything else
			if( needsFullRenderSizeForUI )
			{
				// dont actually add the renderer since we will manually call it later
				_screenSpaceRenderer = new ScreenSpaceRenderer( 100, SCREEN_SPACE_RENDER_LAYER );
				_screenSpaceRenderer.shouldDebugRender = false;
				finalRenderDelegate = this;
			}
			else
			{
				addRenderer( new ScreenSpaceRenderer( 100, SCREEN_SPACE_RENDER_LAYER ) );
			}

			if( addExcludeRenderer )
				addRenderer( new RenderLayerExcludeRenderer( 0, SCREEN_SPACE_RENDER_LAYER ) );

			// create our canvas and put it on the screen space render layer
			canvas = createEntity( "ui" ).addComponent( new UICanvas() );
			canvas.isFullScreen = true;
			canvas.renderLayer = SCREEN_SPACE_RENDER_LAYER;
			setupSceneSelector();
		}


		IEnumerable<Type> getTypesWithSampleSceneAttribute()
		{
			var assembly = typeof( SampleScene ).Assembly;
			var scenes = assembly.GetTypes().Where( t => t.GetCustomAttributes( typeof( SampleSceneAttribute ), true ).Length > 0 )
					.OrderBy( t => ( (SampleSceneAttribute)t.GetCustomAttributes( typeof( SampleSceneAttribute ), true )[0] ).order );

			foreach( var s in scenes )
				yield return s;
		}


		void setupSceneSelector()
		{
			_table = canvas.stage.addElement( new Table() );
			_table.setFillParent( true ).right().top();

			var topButtonStyle = new TextButtonStyle( new PrimitiveDrawable( Color.Black, 10f ), new PrimitiveDrawable( Color.Yellow ), new PrimitiveDrawable( Color.DarkSlateBlue ) )
			{
				downFontColor = Color.Black
			};
			_table.add( new TextButton( "Toggle Scene List", topButtonStyle ) ).setFillX().setMinHeight( 30 ).getElement<Button>().onClicked += onToggleSceneListClicked;

			_table.row().setPadTop( 10 );
			var checkbox = _table.add( new CheckBox( "Debug Render", new CheckBoxStyle
			{
				checkboxOn = new PrimitiveDrawable( 30, Color.Green ),
				checkboxOff = new PrimitiveDrawable( 30, new Color( 0x00, 0x3c, 0xe7, 0xff ) )
			} ) ).getElement<CheckBox>();
			checkbox.onChanged += enabled => Core.debugRenderEnabled = enabled;
			checkbox.isChecked = Core.debugRenderEnabled;
			_table.row().setPadTop( 30 );

			var buttonStyle = new TextButtonStyle( new PrimitiveDrawable( new Color( 78, 91, 98 ), 10f ), new PrimitiveDrawable( new Color( 244, 23, 135 ) ), new PrimitiveDrawable( new Color( 168, 207, 115 ) ) )
			{
				downFontColor = Color.Black
			};

			// find every Scene with the SampleSceneAttribute and create a button for each one
			foreach( var type in getTypesWithSampleSceneAttribute() )
			{
				foreach( var attr in type.GetCustomAttributes( true ) )
				{
					if( attr.GetType() == typeof( SampleSceneAttribute ) )
					{
						var sampleAttr = attr as SampleSceneAttribute;
						var button = _table.add( new TextButton( sampleAttr.buttonName, buttonStyle ) ).setFillX().setMinHeight( 30 ).getElement<TextButton>();
						_sceneButtons.Add( button );
						button.onClicked += butt =>
						{
							// stop all tweens in case any demo scene started some up
							TweenManager.stopAllTweens();
							Core.startSceneTransition( new FadeTransition( () => Activator.CreateInstance( type ) as Scene ) );
						};

						_table.row().setPadTop( 10 );

						// optionally add instruction text for the current scene
						if( sampleAttr.instructionText != null && type == GetType() )
							addInstructionText( sampleAttr.instructionText );
					}
				}
			}
		}


		void addInstructionText( string text )
		{
			var instructionsEntity = createEntity( "instructions" );
			instructionsEntity.addComponent( new Text( Graphics.instance.bitmapFont, text, new Vector2( 10, 10 ), Color.White ) )
				.setRenderLayer( SCREEN_SPACE_RENDER_LAYER );
		}


		void onToggleSceneListClicked( Button butt )
		{
			foreach( var button in _sceneButtons )
				button.setIsVisible( !button.isVisible() );
		}


		#region IFinalRenderDelegate

		public Scene scene { get; set; }

		public void onAddedToScene()
		{ }


		public void onSceneBackBufferSizeChanged( int newWidth, int newHeight )
		{
			_screenSpaceRenderer.onSceneBackBufferSizeChanged( newWidth, newHeight );
		}


		public void handleFinalRender( Color letterboxColor, Microsoft.Xna.Framework.Graphics.RenderTarget2D source, Rectangle finalRenderDestinationRect, Microsoft.Xna.Framework.Graphics.SamplerState samplerState )
		{
			Core.graphicsDevice.SetRenderTarget( null );
			Core.graphicsDevice.Clear( letterboxColor );
			Graphics.instance.batcher.begin( BlendState.Opaque, samplerState, DepthStencilState.None, RasterizerState.CullNone, null );
			Graphics.instance.batcher.draw( source, finalRenderDestinationRect, Color.White );
			Graphics.instance.batcher.end();

			_screenSpaceRenderer.render( scene );
		}

		#endregion

	}


	[AttributeUsage( AttributeTargets.Class )]
	public class SampleSceneAttribute : Attribute
	{
		public string buttonName;
		public int order;
		public string instructionText;


		public SampleSceneAttribute( string buttonName, int order, string instructionText = null )
		{
			this.buttonName = buttonName;
			this.order = order;
			this.instructionText = instructionText;
		}
	}
}

