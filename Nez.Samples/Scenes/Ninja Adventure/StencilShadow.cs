using System;
using Nez.Sprites;
using Microsoft.Xna.Framework;


namespace Nez.Samples
{
	/// <summary>
	/// this component will draw a black version of the spriteToShadow every frame. It is intended to be used in conjunction with a RenderState
	/// that includes a stencil read.
	/// </summary>
	public class StencilShadow : RenderableComponent
	{
		public override float width { get { return _spriteToShadow.width; } }

		public override float height { get { return _spriteToShadow.height; } }


		Sprite _spriteToShadow;


		public StencilShadow( Sprite spriteToShadow )
		{
			_spriteToShadow = spriteToShadow;
			color = Color.Black;
		}


		public override void render( Graphics graphics, Camera camera )
		{
			if( isVisibleFromCamera( camera ) )
				graphics.spriteBatch.Draw( _spriteToShadow.subtexture, entity.transform.position + _spriteToShadow.localPosition, _spriteToShadow.subtexture.sourceRect, color, entity.transform.rotation, _spriteToShadow.origin, entity.transform.scale, _spriteToShadow.spriteEffects, _spriteToShadow.layerDepth );
		}
	}
}

