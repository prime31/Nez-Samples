using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez.Particles;
using Nez.Textures;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Xml;

namespace Nez.Samples
{
	public class PexExporter
	{

		public PexExporter()
		{
		}

		/// <summary>
		/// Export the specified ParticleEmitterConfig to the specified filename in PEX format.
		/// </summary>
		/// <param name="emitterConfig">Emitter config.</param>
		/// <param name="filename">Output filename.</param>
		public void export( ParticleEmitterConfig emitterConfig, string filename )
		{
			// We don't use XmlSerializer here because the output format needed by PEX is bizzare.
			// I first tried to implement it using Serializer overrides, but that became much larger than
			// constructing by hand.
			XmlDocument doc = new XmlDocument();
			XmlElement parent = doc.CreateElement( "particleEmitterConfig" );
			addXmlChild( doc, parent, "yCoordFlipped", "1" );
			addXmlChild( doc, parent, "sourcePosition", emitterConfig.sourcePosition );
			addXmlChild( doc, parent, "sourcePositionVariance", emitterConfig.sourcePositionVariance );
			addXmlChild( doc, parent, "speed", emitterConfig.speed );
			addXmlChild( doc, parent, "speedVariance", emitterConfig.speedVariance );
			addXmlChild( doc, parent, "particleLifeSpan", emitterConfig.particleLifespan, "F4" );
			addXmlChild( doc, parent, "particleLifespanVariance", emitterConfig.particleLifespanVariance, "F4" );
			addXmlChild( doc, parent, "angle", emitterConfig.angle, "F0" );
			addXmlChild( doc, parent, "angleVariance", emitterConfig.angleVariance, "F0" );
			addXmlChild( doc, parent, "gravity", emitterConfig.gravity );
			addXmlChild( doc, parent, "radialAcceleration", emitterConfig.radialAcceleration );
			addXmlChild( doc, parent, "radialAccelVariance", emitterConfig.radialAccelVariance );
			addXmlChild( doc, parent, "tangentialAcceleration", emitterConfig.tangentialAcceleration );
			addXmlChild( doc, parent, "tangentialAccelVariance", emitterConfig.tangentialAccelVariance );
			addXmlChild( doc, parent, "startColor", emitterConfig.startColor );
			addXmlChild( doc, parent, "startColorVariance", emitterConfig.startColorVariance );
			addXmlChild( doc, parent, "finishColor", emitterConfig.finishColor );
			addXmlChild( doc, parent, "finishColorVariance", emitterConfig.finishColorVariance );
			addXmlChild( doc, parent, "maxParticles", emitterConfig.maxParticles, "F0" );
			addXmlChild( doc, parent, "startParticleSize", emitterConfig.startParticleSize );
			addXmlChild( doc, parent, "startParticleSizeVariance", emitterConfig.startParticleSizeVariance );
			addXmlChild( doc, parent, "finishParticleSize", emitterConfig.finishParticleSize );
			addXmlChild( doc, parent, "finishParticleSizeVariance", emitterConfig.finishParticleSizeVariance );
			addXmlChild( doc, parent, "duration", emitterConfig.duration );
			addXmlChild( doc, parent, "emitterType", emitterConfig.emitterType );
			addXmlChild( doc, parent, "maxRadius", emitterConfig.maxRadius );
			addXmlChild( doc, parent, "maxRadiusVariance", emitterConfig.maxRadiusVariance );
			addXmlChild( doc, parent, "minRadius", emitterConfig.minRadius );
			addXmlChild( doc, parent, "minRadiusVariance", emitterConfig.minRadiusVariance );
			addXmlChild( doc, parent, "rotatePerSecond", emitterConfig.rotatePerSecond );
			addXmlChild( doc, parent, "rotatePerSecondVariance", emitterConfig.rotatePerSecondVariance );
			addXmlChild( doc, parent, "blendFuncSource", emitterConfig.blendFuncSource );
			addXmlChild( doc, parent, "blendFuncDestination", emitterConfig.blendFuncDestination );
			addXmlChild( doc, parent, "rotationStart", emitterConfig.rotationStart );
			addXmlChild( doc, parent, "rotationStartVariance", emitterConfig.rotationStartVariance );
			addXmlChild( doc, parent, "rotationEnd", emitterConfig.rotationEnd );
			addXmlChild( doc, parent, "rotationEndVariance", emitterConfig.rotationEndVariance );
			addXmlChild( doc, parent, "texture", emitterConfig.subtexture );


			doc.AppendChild( parent );
			try
			{
				doc.Save(filename);
			}
			catch( Exception e )
			{
				// It would be nice to display another dialog when something happens,
				// but this is a sample program and the application console will do.
				System.Console.WriteLine( "Error saving to {0}: {1}", filename, e.Message );
			}
		}

		void addXmlChild( XmlDocument doc, XmlElement parent, string elementName, float value, string formatString = "F" )
		{
			addXmlChild( doc, parent, elementName, value.ToString( formatString ) );
		}

		void addXmlChild( XmlDocument doc, XmlElement parent, string elementName, string value )
		{
			var attrs = new Dictionary<string, string>();
			attrs["value"] = value;
			addXmlChild( doc, parent, elementName, attrs );
		}

		void addXmlChild( XmlDocument doc, XmlElement parent, string elementName, Vector2 coordinate )
		{
			var attrs = new Dictionary<string, string>();
			attrs["x"] = coordinate.X.ToString();
			attrs["y"] = coordinate.Y.ToString();
			addXmlChild( doc, parent, elementName, attrs );
		}

		void addXmlChild( XmlDocument doc, XmlElement parent, string elementName, Color color )
		{
			var attrs = new Dictionary<string, string>();
			float r = (float)color.R / 255f;
			float g = (float)color.G / 255f;
			float b = (float)color.B / 255f;
			float a = (float)color.A / 255f;
			attrs["red"] = r.ToString( "F2" );
			attrs["green"] = g.ToString( "F2" );
			attrs["blue"] = b.ToString( "F2" );
			attrs["alpha"] = a.ToString( "F2" );
			addXmlChild( doc, parent, elementName, attrs );
		}

		void addXmlChild( XmlDocument doc, XmlElement parent, string elementName, Blend blendFunction )
		{
			int value = 0;
			switch( blendFunction )
			{
				case Blend.Zero:
					value = 0;
					break;
				case Blend.One:
					value = 1;
					break;
				case Blend.SourceColor:
					value = 0x300;
					break;
				case Blend.InverseSourceColor:
					value = 0x0301;
					break;
				case Blend.SourceAlpha:
					value = 0x0302;
					break;
				case Blend.InverseSourceAlpha:
					value = 0x0303;
					break;
				case Blend.DestinationAlpha:
					value = 0x0304;
					break;
				case Blend.InverseDestinationAlpha:
					value = 0x0305;
					break;
				case Blend.DestinationColor:
					value = 0x0306;
					break;
				case Blend.InverseDestinationColor:
					value = 0x0307;
					break;
				case Blend.SourceAlphaSaturation:
					value = 0x0308;
					break;
			}
			addXmlChild( doc, parent, elementName, value, "F0" );
		}

		void addXmlChild( XmlDocument doc, XmlElement parent, string elementName, ParticleEmitterType emitterType )
		{
			int value = 0;
			if( emitterType == ParticleEmitterType.Radial )
				value = 1;
			addXmlChild( doc, parent, elementName, value, "F0" );
		}

		void addXmlChild( XmlDocument doc, XmlElement parent, string elementName, Subtexture texture )
		{
			var rawStream = new MemoryStream();
			texture.texture2D.SaveAsPng( rawStream, texture.texture2D.Width, texture.texture2D.Height );
			rawStream.Position = 0;

			using( var outStream = new MemoryStream() )
			{
				using( var compressedStream = new GZipStream( outStream, CompressionLevel.Optimal ) )
					rawStream.CopyTo( compressedStream );
				var bytes = outStream.ToArray();

				var attrs = new Dictionary<string, string>();
				attrs["name"] = "texture.png";
				attrs["data"] = Convert.ToBase64String( bytes );
				addXmlChild( doc, parent, elementName, attrs );
			}
		}

		void addXmlChild( XmlDocument doc, XmlElement parent, string elementName, Dictionary<string, string> attributes )
		{
			XmlElement element = doc.CreateElement( elementName );
			foreach( var key in attributes.Keys )
			{
				var attribute = doc.CreateAttribute( key );
				attribute.Value = attributes[key];
				element.Attributes.Append( attribute );
			}
			parent.AppendChild( element );
		}
	}
}
