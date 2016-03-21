using System;
using Microsoft.Xna.Framework;
using Nez.Textures;
using Microsoft.Xna.Framework.Graphics;
using Nez.Sprites;
using Nez.Tweens;


namespace Nez.Samples
{
	[SampleScene( "Shadows", "2D shadow system\nArrow keys to move")]
	public class ShadowsScene : SampleScene
	{
		public ShadowsScene() : base( false, true )
		{}


		public override void initialize()
		{
			base.initialize();

			Screen.setSize( 1280, 720 );

			// render layer for all lights and any emissive Sprites
			var LIGHT_RENDER_LAYER = 5;
			clearColor = Color.MonoGameOrange;

			// create a Renderer that renders all but the light layer and screen space layer
			addRenderer( new RenderLayerExcludeRenderer( 0, LIGHT_RENDER_LAYER, SCREEN_SPACE_RENDER_LAYER ) );

			// create a Renderer that renders only the light layer into a render target
			var lightRenderer = addRenderer( new RenderLayerRenderer( -1, LIGHT_RENDER_LAYER ) );
			lightRenderer.renderTargetClearColor = new Color( 10, 10, 10, 255 );
			lightRenderer.renderTarget = RenderTarget.create();

			// add a PostProcessor that renders the light render target
			addPostProcessor( new SpriteLightPostProcessor( 0, lightRenderer ) );

			var lightTexture = contentManager.Load<Texture2D>( "Shadows/sprite-light" );
			var moonTexture = contentManager.Load<Texture2D>( "Shared/moon" );
			var blockTexture = contentManager.Load<Texture2D>( "Shadows/Block" );
			var blockGlowTexture = contentManager.Load<Texture2D>( "Shadows/BlockGlow" );

			// create some moons
			Action<Vector2,string,bool> boxMaker = ( Vector2 pos, string name, bool isTrigger ) =>
			{
				var ent = createEntity( name );
				ent.transform.position = pos;
				ent.addComponent( new Sprite( blockTexture ) );
				ent.colliders.add( new BoxCollider() );

				// add a glow sprite on the light render layer
				var glowSprite = new Sprite( blockGlowTexture );
				glowSprite.renderLayer = LIGHT_RENDER_LAYER;
				ent.addComponent( glowSprite );

				if( isTrigger )
				{
					ent.colliders[0].isTrigger = true;
					ent.addComponent( new TriggerListener() );
				}
			};

			boxMaker( new Vector2( 0, 100 ), "moon1", false );
			boxMaker( new Vector2( 150, 100 ), "moon11", false );
			boxMaker( new Vector2( 300, 100 ), "moon12", false );
			boxMaker( new Vector2( 450, 100 ), "moon13", false );
			boxMaker( new Vector2( 600, 100 ), "moon14", false );

			boxMaker( new Vector2( 50, 500 ), "moon3", true );
			boxMaker( new Vector2( 500, 250 ), "moon4", false );

			var moonEnt = createEntity( "moon" );
			moonEnt.addComponent( new Sprite( moonTexture ) );
			moonEnt.transform.position = new Vector2( 100, 0 );

			moonEnt = createEntity( "moon2" );
			moonEnt.addComponent( new Sprite( moonTexture ) );
			moonEnt.transform.position = new Vector2( -500, 0 );


			var lightEnt = createEntity( "sprite-light" );
			lightEnt.addComponent( new Sprite( lightTexture ) );
			lightEnt.transform.position = new Vector2( -700, 0 );
			lightEnt.transform.scale = new Vector2( 4 );
			lightEnt.getComponent<Sprite>().renderLayer = LIGHT_RENDER_LAYER;


			// add an animation to "moon4"
			findEntity( "moon4" ).addComponent( new MovingPlatform( 250, 400 ) );

			// create a player moon
			var entity = createEntity( "player-block" );
			entity.transform.position = new Vector2( 220, 220 );
			var sprite = new Sprite( blockTexture );
			sprite.renderLayer = LIGHT_RENDER_LAYER;
			entity.addComponent( sprite );
			entity.addComponent( new SimpleMover() );
			entity.colliders.add( new BoxCollider() );


			// add a follow camera
			var camFollow = createEntity( "camera-follow" );
			camFollow.addComponent( new FollowCamera( entity ) );
			camFollow.addComponent( new CameraShake() );


			// setup some lights and animate the colors
			var pointLight = new Nez.Shadows.PointLight( 600, Color.Red );
			pointLight.renderLayer = LIGHT_RENDER_LAYER;
			pointLight.power = 1f;
			var light = createEntity( "light" );
			light.transform.position = new Vector2( 650f, 300f );
			light.addComponent( pointLight );

			PropertyTweens.colorPropertyTo( pointLight, "color", new Color( 0, 0, 255, 255 ), 1f )
				.setEaseType( EaseType.Linear )
				.setLoops( LoopType.PingPong, 100 )
				.start();

			PropertyTweens.floatPropertyTo( pointLight, "power", 0.1f, 1f )
				.setEaseType( EaseType.Linear )
				.setLoops( LoopType.PingPong, 100 )
				.start();


			pointLight = new Nez.Shadows.PointLight( 500, Color.Yellow );
			pointLight.renderLayer = LIGHT_RENDER_LAYER;
			light = createEntity( "light-two" );
			light.transform.position = new Vector2( -50f );
			light.addComponent( pointLight );

			PropertyTweens.colorPropertyTo( pointLight, "color", new Color( 0, 255, 0, 255 ), 1f )
				.setEaseType( EaseType.Linear )
				.setLoops( LoopType.PingPong, 100 )
				.start();


			pointLight = new Nez.Shadows.PointLight( 500, Color.AliceBlue );
			pointLight.renderLayer = LIGHT_RENDER_LAYER;
			light = createEntity( "light-three" );
			light.transform.position = new Vector2( 100f );
			light.addComponent( pointLight );
		}

	}
}

