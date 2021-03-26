using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez.Particles;
using Nez.Textures;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Xml;


namespace Nez.Samples
{
	public class PexExporter
	{
		/// <summary>
		/// Export the specified ParticleEmitterConfig to the specified filename in PEX format.
		/// </summary>
		/// <param name="emitterConfig">Emitter config.</param>
		/// <param name="filename">Output filename.</param>
		public void Export(ParticleEmitterConfig emitterConfig, string filename)
		{
			// We don't use XmlSerializer here because the output format needed by PEX is bizzare.
			// I first tried to implement it using Serializer overrides, but that became much larger than
			// constructing by hand.
			var doc = new XmlDocument();
			var parent = doc.CreateElement("particleEmitterConfig");
			AddXmlChild(doc, parent, "yCoordFlipped", "1");
			AddXmlChild(doc, parent, "sourcePosition", emitterConfig.SourcePosition);
			AddXmlChild(doc, parent, "sourcePositionVariance", emitterConfig.SourcePositionVariance);
			AddXmlChild(doc, parent, "speed", emitterConfig.Speed);
			AddXmlChild(doc, parent, "speedVariance", emitterConfig.SpeedVariance);
			AddXmlChild(doc, parent, "particleLifeSpan", emitterConfig.ParticleLifespan, "F4");
			AddXmlChild(doc, parent, "particleLifespanVariance", emitterConfig.ParticleLifespanVariance, "F4");
			AddXmlChild(doc, parent, "angle", emitterConfig.Angle, "F0");
			AddXmlChild(doc, parent, "angleVariance", emitterConfig.AngleVariance, "F0");
			AddXmlChild(doc, parent, "gravity", emitterConfig.Gravity);
			AddXmlChild(doc, parent, "radialAcceleration", emitterConfig.RadialAcceleration);
			AddXmlChild(doc, parent, "radialAccelVariance", emitterConfig.RadialAccelVariance);
			AddXmlChild(doc, parent, "tangentialAcceleration", emitterConfig.TangentialAcceleration);
			AddXmlChild(doc, parent, "tangentialAccelVariance", emitterConfig.TangentialAccelVariance);
			AddXmlChild(doc, parent, "startColor", emitterConfig.StartColor);
			AddXmlChild(doc, parent, "startColorVariance", emitterConfig.StartColorVariance);
			AddXmlChild(doc, parent, "finishColor", emitterConfig.FinishColor);
			AddXmlChild(doc, parent, "finishColorVariance", emitterConfig.FinishColorVariance);
			AddXmlChild(doc, parent, "maxParticles", emitterConfig.MaxParticles, "F0");
			AddXmlChild(doc, parent, "startParticleSize", emitterConfig.StartParticleSize);
			AddXmlChild(doc, parent, "startParticleSizeVariance", emitterConfig.StartParticleSizeVariance);
			AddXmlChild(doc, parent, "finishParticleSize", emitterConfig.FinishParticleSize);
			AddXmlChild(doc, parent, "finishParticleSizeVariance", emitterConfig.FinishParticleSizeVariance);
			AddXmlChild(doc, parent, "duration", emitterConfig.Duration);
			AddXmlChild(doc, parent, "emitterType", emitterConfig.EmitterType);
			AddXmlChild(doc, parent, "maxRadius", emitterConfig.MaxRadius);
			AddXmlChild(doc, parent, "maxRadiusVariance", emitterConfig.MaxRadiusVariance);
			AddXmlChild(doc, parent, "minRadius", emitterConfig.MinRadius);
			AddXmlChild(doc, parent, "minRadiusVariance", emitterConfig.MinRadiusVariance);
			AddXmlChild(doc, parent, "rotatePerSecond", emitterConfig.RotatePerSecond);
			AddXmlChild(doc, parent, "rotatePerSecondVariance", emitterConfig.RotatePerSecondVariance);
			AddXmlChild(doc, parent, "blendFuncSource", emitterConfig.BlendFuncSource);
			AddXmlChild(doc, parent, "blendFuncDestination", emitterConfig.BlendFuncDestination);
			AddXmlChild(doc, parent, "rotationStart", emitterConfig.RotationStart);
			AddXmlChild(doc, parent, "rotationStartVariance", emitterConfig.RotationStartVariance);
			AddXmlChild(doc, parent, "rotationEnd", emitterConfig.RotationEnd);
			AddXmlChild(doc, parent, "rotationEndVariance", emitterConfig.RotationEndVariance);
			AddXmlChild(doc, parent, "texture", emitterConfig.Sprite);


			doc.AppendChild(parent);
			try
			{
				doc.Save(filename);
			}
			catch (Exception e)
			{
				// It would be nice to display another dialog when something happens,
				// but this is a sample program and the application console will do.
				System.Console.WriteLine("Error saving to {0}: {1}", filename, e.Message);
			}
		}

		void AddXmlChild(XmlDocument doc, XmlElement parent, string elementName, float value, string formatString = "F")
		{
			AddXmlChild(doc, parent, elementName, value.ToString(formatString));
		}

		void AddXmlChild(XmlDocument doc, XmlElement parent, string elementName, string value)
		{
			var attrs = new Dictionary<string, string>();
			attrs["value"] = value;
			AddXmlChild(doc, parent, elementName, attrs);
		}

		void AddXmlChild(XmlDocument doc, XmlElement parent, string elementName, Vector2 coordinate)
		{
			var attrs = new Dictionary<string, string>();
			attrs["x"] = coordinate.X.ToString();
			attrs["y"] = coordinate.Y.ToString();
			AddXmlChild(doc, parent, elementName, attrs);
		}

		void AddXmlChild(XmlDocument doc, XmlElement parent, string elementName, Color color)
		{
			var attrs = new Dictionary<string, string>();
			var r = color.R / 255f;
			var g = color.G / 255f;
			var b = color.B / 255f;
			var a = color.A / 255f;
			attrs["red"] = r.ToString("F2");
			attrs["green"] = g.ToString("F2");
			attrs["blue"] = b.ToString("F2");
			attrs["alpha"] = a.ToString("F2");
			AddXmlChild(doc, parent, elementName, attrs);
		}

		void AddXmlChild(XmlDocument doc, XmlElement parent, string elementName, Blend blendFunction)
		{
			int value = 0;
			switch (blendFunction)
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

			AddXmlChild(doc, parent, elementName, value, "F0");
		}

		void AddXmlChild(XmlDocument doc, XmlElement parent, string elementName, ParticleEmitterType emitterType)
		{
			int value = 0;
			if (emitterType == ParticleEmitterType.Radial)
				value = 1;
			AddXmlChild(doc, parent, elementName, value, "F0");
		}

		void AddXmlChild(XmlDocument doc, XmlElement parent, string elementName, Sprite texture)
		{
			using (var rawStream = new MemoryStream())
			{
				texture.Texture2D.SaveAsPng(rawStream, texture.Texture2D.Width, texture.Texture2D.Height);
				rawStream.Position = 0;

				using (var outStream = new MemoryStream())
				{
					using (var compressedStream = new GZipStream(outStream, CompressionLevel.Optimal))
						rawStream.CopyTo(compressedStream);
					var bytes = outStream.ToArray();

					var attrs = new Dictionary<string, string>();
					attrs["name"] = "texture.png";
					attrs["data"] = Convert.ToBase64String(bytes);
					AddXmlChild(doc, parent, elementName, attrs);
				}
			}
		}

		void AddXmlChild(XmlDocument doc, XmlElement parent, string elementName, Dictionary<string, string> attributes)
		{
			var element = doc.CreateElement(elementName);
			foreach (var key in attributes.Keys)
			{
				var attribute = doc.CreateAttribute(key);
				attribute.Value = attributes[key];
				element.Attributes.Append(attribute);
			}

			parent.AppendChild(element);
		}
	}
}