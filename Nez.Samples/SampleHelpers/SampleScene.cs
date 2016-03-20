using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Nez.UI;
using Microsoft.Xna.Framework.Input;


namespace Nez.Samples
{
	public abstract class SampleScene : Scene
	{
		public const int SCREEN_SPACE_RENDER_LAYER = 999;
		public UICanvas canvas;

		Table _table;
		List<Button> _sceneButtons = new List<Button>();


		public SampleScene( bool addExcludeRenderer = true ) : base()
		{
			// setup one renderer in screen space for the UI and then (optionally) another renderer to render everything else
			addRenderer( new ScreenSpaceRenderer( 100, SCREEN_SPACE_RENDER_LAYER ) );

			if( addExcludeRenderer )
				addRenderer( new RenderLayerExcludeRenderer( 0, SCREEN_SPACE_RENDER_LAYER ) );

			// create our canvas and put it on the screen space render layer
			canvas = createEntity( "ui" ).addComponent( new UICanvas() );
			canvas.renderLayer = SCREEN_SPACE_RENDER_LAYER;
			setupSceneSelector();
		}


		IEnumerable<Type> getTypesWithSampleSceneAttribute()
		{
			var assembly = typeof( SampleScene ).Assembly;
			foreach( var type in assembly.GetTypes() )
			{
				if( type.GetCustomAttributes( typeof( SampleSceneAttribute ), true ).Length > 0 )
					yield return type;
			}
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
			_table.add( new CheckBox( "Debug Render", new CheckBoxStyle {
				checkboxOn = new PrimitiveDrawable( 30, Color.Green ),
				checkboxOff = new PrimitiveDrawable( 30, Color.MonoGameOrange )
			} ) ).getElement<CheckBox>().onChanged += enabled => Core.debugRenderEnabled = enabled;
			_table.row().setPadTop( 30 );

			var buttonStyle = new TextButtonStyle( new PrimitiveDrawable( new Color( 78, 91, 98 ), 10f ), new PrimitiveDrawable( new Color( 244, 23, 135 ) ), new PrimitiveDrawable( new Color( 168, 207, 115 ) ) ) {
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
							Core.scene = Activator.CreateInstance( type ) as Scene;
						};

						// optionally add instruction text
						if( sampleAttr.instructionText != null )
							addInstructionText( sampleAttr.instructionText );
					}
				}
			}
		}


		void addInstructionText( string text )
		{
			var instructionsEntity = createEntity( "instructions" );
			instructionsEntity.addComponent( new Text( Graphics.instance.bitmapFont, text, new Vector2( 10, 10 ), Color.White ) );
		}


		void onToggleSceneListClicked( Button butt )
		{
			foreach( var button in _sceneButtons )
				button.setIsVisible( !button.isVisible() );
		}

	}


	[AttributeUsage( System.AttributeTargets.Class )]
	public class SampleSceneAttribute : Attribute
	{
		public string buttonName;
		public string instructionText;


		public SampleSceneAttribute( string buttonName, string instructionText = null )
		{
			this.buttonName = buttonName;
			this.instructionText = instructionText;
		}
	}
}

