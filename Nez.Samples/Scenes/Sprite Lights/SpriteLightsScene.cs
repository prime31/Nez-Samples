using System;
using Nez.Textures;
using Microsoft.Xna.Framework.Graphics;
using Nez.Sprites;
using Microsoft.Xna.Framework;
using Nez.Tweens;
using Nez.UI;


namespace Nez.Samples
{
	[SampleScene( "Sprite Lights", "Old-school 2D blended lighting\nPlay with the controls to change the effect and add lights" )]
	public class SpriteLightsScene : SampleScene
	{
		public const int SPRITE_LIGHT_RENDER_LAYER = 50;
		SpriteLightPostProcessor _spriteLightPostProcessor;
		RenderLayerRenderer _lightRenderer;

		
		public SpriteLightsScene() : base( false, true )
		{}


		public override void initialize()
		{
			base.initialize();

			// setup screen that fits our map
			setDesignResolution( 1280, 720, Scene.SceneResolutionPolicy.ShowAll );
			Screen.setSize( 1280, 720 );

			addRenderer( new RenderLayerExcludeRenderer( 0, SCREEN_SPACE_RENDER_LAYER, SPRITE_LIGHT_RENDER_LAYER ) );
			_lightRenderer = addRenderer( new RenderLayerRenderer( -1, SPRITE_LIGHT_RENDER_LAYER ) );
			_lightRenderer.renderTexture = new RenderTexture();
			_lightRenderer.renderTargetClearColor = new Color( 10, 10, 10, 255 );

			_spriteLightPostProcessor = addPostProcessor( new SpriteLightPostProcessor( 0, _lightRenderer.renderTexture ) );
			addPostProcessor( new ScanlinesPostProcessor( 0 ) );


			var bg = content.Load<Texture2D>( Content.SpriteLights.bg );
			var bgEntity = createEntity( "bg" );
			bgEntity.transform.position = Screen.center;
			bgEntity.addComponent( new Sprite( bg ) );
			bgEntity.transform.scale = new Vector2( 9.4f );

			var moonTex = content.Load<Texture2D>( Content.Shared.moon );
			var entity = createEntity( "moon" );
			entity.addComponent( new Sprite( moonTex ) );
			entity.transform.position = new Vector2( Screen.width / 4, Screen.height / 8 );


			var lightTex = content.Load<Texture2D>( Content.SpriteLights.spritelight );
			var pixelLightTex = content.Load<Texture2D>( Content.SpriteLights.pixelspritelight );

			addSpriteLight( lightTex, new Vector2( 50, 50 ), 2 );
			addSpriteLight( lightTex, Screen.center, 3 );
			addSpriteLight( lightTex, new Vector2( Screen.width - 100, 150 ), 2 );
			addSpriteLight( pixelLightTex, Screen.center + new Vector2( 200, 10 ), 10 );
			addSpriteLight( pixelLightTex, Screen.center - new Vector2( 200, 10 ), 13 );
			addSpriteLight( pixelLightTex, Screen.center + new Vector2( 10, 200 ), 8 );

			createUI();
		}


		void createUI()
		{
			// stick a UI in so we can play with the sprite light effect
			var uiCanvas = createEntity( "sprite-light-ui" ).addComponent( new UICanvas() );
			uiCanvas.isFullScreen = true;
			uiCanvas.renderLayer = SCREEN_SPACE_RENDER_LAYER;
			var skin = Skin.createDefaultSkin();

			var table = uiCanvas.stage.addElement( new Table() );
			table.setFillParent( true ).left().top().padLeft( 10 ).padTop( 50 );


			var checkbox = table.add( new CheckBox( "Toggle PostProcessor", skin ) ).getElement<CheckBox>();
			checkbox.isChecked = true;
			checkbox.onChanged += isChecked =>
			{
				_spriteLightPostProcessor.enabled = isChecked;
			};

			table.row().setPadTop( 20 ).setAlign( Align.left );

			table.add( "Blend Multiplicative Factor" );
			table.row().setPadTop( 0 ).setAlign( Align.left );

			var slider = table.add( new Slider( 0.5f, 3f, 0.1f, false, skin.get<SliderStyle>() ) ).setFillX().getElement<Slider>();
			slider.setValue( 1f );
			slider.onChanged += value =>
			{
				_spriteLightPostProcessor.multiplicativeFactor = value;
			};

			table.row().setPadTop( 20 ).setAlign( Align.left );

			table.add( "Ambient Light Intensity" );
			table.row().setPadTop( 0 ).setAlign( Align.left );

			var ambientColorStyle = table.add( new Slider( 10, 75, 1f, false, skin.get<SliderStyle>() ) ).setFillX().getElement<Slider>();
			ambientColorStyle.setValue( 10f );
			ambientColorStyle.onChanged += value =>
			{
				var valueInt = Mathf.roundToInt( value );
				_lightRenderer.renderTargetClearColor = new Color( valueInt, valueInt, valueInt * 2, 255 );
			};

			table.row().setPadTop( 20 ).setAlign( Align.left ).setFillX();

			var button = table.add( new TextButton( "Add Light", skin ) ).setFillX().setMinHeight( 30 ).getElement<TextButton>();
			button.onClicked += butt =>
			{
				var lightTex = content.Load<Texture2D>( Content.SpriteLights.spritelight );
				addSpriteLight( lightTex, Screen.center, 2f );
			};
		}


		void addSpriteLight( Texture2D texture, Vector2 position, float scale )
		{
			// random target to tween towards that is on screen
			var target = new Vector2( Nez.Random.range( 50, sceneRenderTargetSize.X - 100 ), Nez.Random.range( 50, sceneRenderTargetSize.Y - 100 ) );

			var entity = createEntity( "light" );
			var sprite = entity.addComponent( new Sprite( texture ) );
			entity.transform.position = position;
			entity.transform.scale = new Vector2( scale );
			sprite.renderLayer = SPRITE_LIGHT_RENDER_LAYER;

			entity.transform.tweenPositionTo( target, 2 )
				.setCompletionHandler( lightTweenCompleted )
				.setRecycleTween( false )
				.start();
		}


		void lightTweenCompleted( ITween<Vector2> tween )
		{
			// get a random point on screen and a random delay for the tweens
			var target = new Vector2( Nez.Random.range( 50, sceneRenderTargetSize.X - 100 ), Nez.Random.range( 50, sceneRenderTargetSize.Y - 100 ) );
			var delay = Nez.Random.range( 0f, 1f );

			var transform = tween.getTargetObject() as Transform;
			tween.prepareForReuse( transform.position, target, 2f )
				.setCompletionHandler( lightTweenCompleted )
				.setDelay( delay )
				.start();

			// every so often add a scale tween
			if( Nez.Random.chance( 0.6f ) )
			{
				transform.tweenLocalScaleTo( transform.localScale.X * 2f, 1f )
					.setLoops( LoopType.PingPong )
					.setEaseType( EaseType.CubicIn )
					.setDelay( delay )
					.start();
			}

			// every so often change our color
			if( Nez.Random.chance( 0.8f ) )
			{
				var sprite = transform.entity.getComponent<Sprite>();
				PropertyTweens.colorPropertyTo( sprite, "color", Nez.Random.nextColor(), 2f )
					.setDelay( delay )
					.start();
			}
		}
	}
}

