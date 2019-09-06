using Nez.Textures;
using Microsoft.Xna.Framework.Graphics;
using Nez.Sprites;
using Microsoft.Xna.Framework;
using Nez.Tweens;
using Nez.UI;


namespace Nez.Samples
{
	[SampleScene( "Sprite Lights", 50, "Old-school 2D blended lighting\nPlay with the controls to change the effect and add lights" )]
	public class SpriteLightsScene : SampleScene
	{
		public const int SPRITE_LIGHT_RENDER_LAYER = 50;
		SpriteLightPostProcessor _spriteLightPostProcessor;
		RenderLayerRenderer _lightRenderer;


		public SpriteLightsScene() : base( false, true )
		{ }


		public override void Initialize()
		{
			base.Initialize();

			// setup screen that fits our map
			SetDesignResolution( 1280, 720, Scene.SceneResolutionPolicy.ShowAll );
			Screen.SetSize( 1280, 720 );

			AddRenderer( new RenderLayerExcludeRenderer( 0, SCREEN_SPACE_RENDER_LAYER, SPRITE_LIGHT_RENDER_LAYER ) );
			_lightRenderer = AddRenderer( new RenderLayerRenderer( -1, SPRITE_LIGHT_RENDER_LAYER ) );
			_lightRenderer.RenderTexture = new RenderTexture();
			_lightRenderer.RenderTargetClearColor = new Color( 10, 10, 10, 255 );

			_spriteLightPostProcessor = AddPostProcessor( new SpriteLightPostProcessor( 0, _lightRenderer.RenderTexture ) );
			AddPostProcessor( new ScanlinesPostProcessor( 0 ) );


			var bg = Content.Load<Texture2D>( Nez.Content.SpriteLights.bg );
			var bgEntity = CreateEntity( "bg" );
			bgEntity.Position = Screen.Center;
			bgEntity.AddComponent( new Sprite( bg ) );
			bgEntity.Scale = new Vector2( 9.4f );

			var moonTex = Content.Load<Texture2D>( Nez.Content.Shared.moon );
			var entity = CreateEntity( "moon" );
			entity.AddComponent( new Sprite( moonTex ) );
			entity.Position = new Vector2( Screen.Width / 4, Screen.Height / 8 );


			var lightTex = Content.Load<Texture2D>( Nez.Content.SpriteLights.spritelight );
			var pixelLightTex = Content.Load<Texture2D>( Nez.Content.SpriteLights.pixelspritelight );

			addSpriteLight( lightTex, new Vector2( 50, 50 ), 2 );
			addSpriteLight( lightTex, Screen.Center, 3 );
			addSpriteLight( lightTex, new Vector2( Screen.Width - 100, 150 ), 2 );
			addSpriteLight( pixelLightTex, Screen.Center + new Vector2( 200, 10 ), 10 );
			addSpriteLight( pixelLightTex, Screen.Center - new Vector2( 200, 10 ), 13 );
			addSpriteLight( pixelLightTex, Screen.Center + new Vector2( 10, 200 ), 8 );

			createUI();
		}


		void createUI()
		{
			// stick a UI in so we can play with the sprite light effect
			var uiCanvas = CreateEntity( "sprite-light-ui" ).AddComponent( new UICanvas() );
			uiCanvas.IsFullScreen = true;
			uiCanvas.RenderLayer = SCREEN_SPACE_RENDER_LAYER;
			var skin = Skin.CreateDefaultSkin();

			var table = uiCanvas.Stage.AddElement( new Table() );
			table.SetFillParent( true ).Left().Top().PadLeft( 10 ).PadTop( 50 );


			var checkbox = table.Add( new CheckBox( "Toggle PostProcessor", skin ) ).GetElement<CheckBox>();
			checkbox.IsChecked = true;
			checkbox.OnChanged += isChecked =>
			{
				_spriteLightPostProcessor.Enabled = isChecked;
			};

			table.Row().SetPadTop( 20 ).SetAlign( Align.Left );

			table.Add( "Blend Multiplicative Factor" );
			table.Row().SetPadTop( 0 ).SetAlign( Align.Left );

			var slider = table.Add( new Slider( 0.5f, 3f, 0.1f, false, skin.Get<SliderStyle>() ) ).SetFillX().GetElement<Slider>();
			slider.SetValue( 1f );
			slider.OnChanged += value =>
			{
				_spriteLightPostProcessor.MultiplicativeFactor = value;
			};

			table.Row().SetPadTop( 20 ).SetAlign( Align.Left );

			table.Add( "Ambient Light Intensity" );
			table.Row().SetPadTop( 0 ).SetAlign( Align.Left );

			var ambientColorStyle = table.Add( new Slider( 10, 75, 1f, false, skin.Get<SliderStyle>() ) ).SetFillX().GetElement<Slider>();
			ambientColorStyle.SetValue( 10f );
			ambientColorStyle.OnChanged += value =>
			{
				var valueInt = Mathf.RoundToInt( value );
				_lightRenderer.RenderTargetClearColor = new Color( valueInt, valueInt, valueInt * 2, 255 );
			};

			table.Row().SetPadTop( 20 ).SetAlign( Align.Left ).SetFillX();

			var button = table.Add( new TextButton( "Add Light", skin ) ).SetFillX().SetMinHeight( 30 ).GetElement<TextButton>();
			button.OnClicked += butt =>
			{
				var lightTex = Content.Load<Texture2D>( Nez.Content.SpriteLights.spritelight );
				var position = new Vector2( Random.Range( 0, Screen.Width ), Random.Range( 0, Screen.Height ) );
				addSpriteLight( lightTex, position, Random.Range( 2f, 3f ) );
			};
		}


		void addSpriteLight( Texture2D texture, Vector2 position, float scale )
		{
			// random target to tween towards that is on screen
			var target = new Vector2( Random.Range( 50, SceneRenderTargetSize.X - 100 ), Random.Range( 50, SceneRenderTargetSize.Y - 100 ) );

			var entity = CreateEntity( "light" );
			var sprite = entity.AddComponent( new Sprite( texture ) );
			entity.Position = position;
			entity.Scale = new Vector2( scale );
			sprite.RenderLayer = SPRITE_LIGHT_RENDER_LAYER;

			if( Random.Chance( 50 ) )
			{
				sprite.SetColor( Random.NextColor() );
				var cyler = entity.AddComponent( new ColorCycler() );
				cyler.WaveFunction = (WaveFunctions)Random.Range( 0, 5 );
				cyler.Offset = Random.NextFloat();
				cyler.Frequency = Random.Range( 0.6f, 1.5f );
				cyler.Phase = Random.NextFloat();
			}
			else
			{
				entity.TweenPositionTo( target, 2 )
					  .SetCompletionHandler( lightTweenCompleted )
					  .SetRecycleTween( false )
					  .Start();
			}
		}


		void lightTweenCompleted( ITween<Vector2> tween )
		{
			// get a random point on screen and a random delay for the tweens
			var target = new Vector2( Random.Range( 50, SceneRenderTargetSize.X - 100 ), Random.Range( 50, SceneRenderTargetSize.Y - 100 ) );
			var delay = Nez.Random.Range( 0f, 1f );

			var Transform = tween.GetTargetObject() as Transform;
			tween.PrepareForReuse( Transform.Position, target, 2f )
				.SetCompletionHandler( lightTweenCompleted )
				.SetDelay( delay )
				.Start();

			// every so often add a scale tween
			if( Random.Chance( 60 ) )
			{
				Transform.TweenLocalScaleTo( Transform.LocalScale.X * 2f, 1f )
					.SetLoops( LoopType.PingPong )
					.SetEaseType( EaseType.CubicIn )
					.SetDelay( delay )
					.Start();
			}

			// every so often change our color
			if( Random.Chance( 80 ) )
			{
				var sprite = Transform.Entity.GetComponent<Sprite>();
				PropertyTweens.ColorPropertyTo( sprite, "color", Random.NextColor(), 2f )
					.SetDelay( delay )
					.Start();
			}
		}
	
	}
}

