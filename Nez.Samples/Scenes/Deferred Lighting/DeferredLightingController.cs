using System;
using Nez.DeferredLighting;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;


namespace Nez.Samples
{
	public class DeferredLightingController : Component, IUpdatable
	{
		DeferredLight _currentLight;
		DirLight _dirLight;


		public override void onAddedToEntity()
		{
			_dirLight = entity.scene.findObjectOfType<DirLight>();
			_currentLight = _dirLight;
		}


		public void update()
		{
			// check for debug toggle
			if( Input.isKeyPressed( Keys.F ) )
			{
				var renderer = entity.scene.getRenderer<DeferredLightingRenderer>();
				renderer.enableDebugBufferRender = !renderer.enableDebugBufferRender;
			}
			
			// check for light changes
			var lightIndex = -1;
			if( Input.isKeyPressed( Keys.D1 ) )
				lightIndex = 0;
			if( Input.isKeyPressed( Keys.D2 ) )
				lightIndex = 1;
			if( Input.isKeyPressed( Keys.D3 ) )
				lightIndex = 2;
			if( Input.isKeyPressed( Keys.D4 ) )
				lightIndex = 3;
			if( Input.isKeyPressed( Keys.D5 ) )
				lightIndex = 4;
			if( Input.isKeyPressed( Keys.D6 ) )
				lightIndex = 5;

			if( lightIndex > -1 )
			{
				var lights = entity.scene.findObjectsOfType<DeferredLight>();
				if( lights.Count > lightIndex )
				{
					_currentLight = lights[lightIndex];
					updateInstructions();
				}
				else
				{
					Debug.log( "no light at index: {0}", lightIndex );
				}
			}
			
			checkInput();
		}


		void checkInput()
		{
			if( _currentLight is DirLight )
			{
				var light = _currentLight as DirLight;
				var zDir = light.direction.Z;
				var rotation = (float)Math.Atan2( light.direction.Y, light.direction.X );
				var magnitude = new Vector2( light.direction.X, light.direction.Y ).Length();

				if( Input.isKeyDown( Keys.Left ) )
					rotation -= 0.1f;
				else if( Input.isKeyDown( Keys.Right ) )
					rotation += 0.1f;

				if( Input.isKeyDown( Keys.Up ) )
					zDir = Mathf.clamp( zDir + 2, 0, 400 );
				else if( Input.isKeyDown( Keys.Down ) )
					zDir = Mathf.clamp( zDir - 2, 0, 400 );

				var newDir = new Vector3( Mathf.cos( rotation ) * magnitude, Mathf.sin( rotation ) * magnitude, zDir );
				light.setDirection( newDir );
			}

			if( _currentLight is PointLight || _currentLight is SpotLight )
			{
				var light = _currentLight as PointLight;

				if( Input.isKeyDown( Keys.Up ) )
					light.setRadius( Mathf.clamp( light.radius + 5, 10, 800 ) );
				else if( Input.isKeyDown( Keys.Down ) )
					light.setRadius(  Mathf.clamp( light.radius - 5, 10, 800 ) );

				if( Input.isKeyDown( Keys.Left ) )
					light.setIntensity( Mathf.clamp( light.intensity - 0.1f, 0, 20 ) );
				else if( Input.isKeyDown( Keys.Right ) )
					light.setIntensity( Mathf.clamp( light.intensity + 0.1f, 0, 20 ) );

				if( Input.isKeyDown( Keys.W ) )
					light.setZPosition( Mathf.clamp( light.zPosition + 1, 0, 300 ) );
				else if( Input.isKeyDown( Keys.S ) )
					light.setZPosition( Mathf.clamp( light.zPosition - 1, 0, 300 ) );
			}

			if( _currentLight is SpotLight )
			{
				var light = _currentLight as SpotLight;

				if( Input.isKeyDown( Keys.A ) )
					light.setConeAngle( Mathf.repeat( light.coneAngle - 3, 360 ) );
				else if( Input.isKeyDown( Keys.D ) )
					light.setConeAngle( Mathf.repeat( light.coneAngle + 3, 360 ) );

				if( Input.isKeyDown( Keys.Z ) )
					light.entity.setLocalRotationDegrees( Mathf.repeat( light.entity.rotationDegrees - 3, 360 ) );
				else if( Input.isKeyDown( Keys.X ) )
					light.entity.setLocalRotationDegrees( Mathf.repeat( light.entity.rotationDegrees + 3, 360 ) );
			}

			// color controls
			var color = _currentLight.color;
			if( Input.isKeyDown( Keys.R ) )
				color.R += (byte)2;
			if( Input.isKeyDown( Keys.G ) )
				color.G += (byte)2;
			if( Input.isKeyDown( Keys.B ) )
				color.B += (byte)2;

			if( color != _currentLight.color )
			{
				_currentLight.color = color;
				Debug.log( "new light color: {0}", _currentLight.color );
			}
		}


		void updateInstructions()
		{
			var textComp = entity.scene.findEntity( "instructions" ).getComponent<Text>();
			var colorText = "\nr/g/b keys change color";

			if( _currentLight is DirLight )
				textComp.text = "Controlling DirLight\nleft/right changes rotation\nup/down changes z-component of direction" + colorText;
			else if( _currentLight is SpotLight )
				textComp.text = "Controlling SpotLight\nup/down changes radius\nleft/right changes intensity\nw/s changes zPosition\na/d changes cone angle\nz/x changes rotation" + colorText;
			else if( _currentLight is PointLight )
				textComp.text = "Controlling PointLight\nup/down changes radius\nleft/right changes intensity\nw/s changes zPosition" + colorText;
		}
	}
}

