//  Copyright 2012 Applied Geographics, Inc.
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using System;
using System.Xml;

namespace AppGeo.Clients.ArcIms.ArcXml
{
  [Serializable]
	public class ScaleDependentRenderer : Renderer
	{
		public const string XmlName = "SCALEDEPENDENTRENDERER";

		public static ScaleDependentRenderer ReadFrom(ArcXmlReader reader)
		{
			try
			{
				ScaleDependentRenderer scaleDependentRenderer = new ScaleDependentRenderer();

				if (reader.HasAttributes)
				{
					while (reader.MoveToNextAttribute())
					{
						string value = reader.ReadContentAsString();

						if (value.Length > 0)
						{
							switch (reader.Name)
							{
								case "lower": scaleDependentRenderer.Lower = value; break;
								case "upper": scaleDependentRenderer.Upper = value; break;
							}
						}
					}

					reader.MoveToElement();
				}

				if (!reader.IsEmptyElement)
				{
					reader.Read();

					while (!(reader.NodeType == XmlNodeType.EndElement && reader.Name == XmlName))
					{
						if (reader.NodeType == XmlNodeType.Element)
						{
							switch (reader.Name)
							{
								case GroupRenderer.XmlName: scaleDependentRenderer.Renderer = GroupRenderer.ReadFrom(reader); break;
								case ScaleDependentRenderer.XmlName: scaleDependentRenderer.Renderer = ScaleDependentRenderer.ReadFrom(reader); break;
								case SimpleLabelRenderer.XmlName: scaleDependentRenderer.Renderer = SimpleLabelRenderer.ReadFrom(reader); break;
								case SimpleRenderer.XmlName: scaleDependentRenderer.Renderer = SimpleRenderer.ReadFrom(reader); break;
								case ValueMapLabelRenderer.XmlName: scaleDependentRenderer.Renderer = ValueMapLabelRenderer.ReadFrom(reader); break;
								case ValueMapRenderer.XmlName: scaleDependentRenderer.Renderer = ValueMapRenderer.ReadFrom(reader); break;
							}
						}

						reader.Read();
					}
				}

				return scaleDependentRenderer;
			}
			catch (Exception ex)
			{
				if (ex is ArcXmlException)
				{
					throw ex;
				}
				else
				{
					throw new ArcXmlException(String.Format("Could not read {0} element.", XmlName), ex);
				}
			}
		}

		public string Lower = "";
		public string Upper = "";

		public Renderer Renderer = null;

		public ScaleDependentRenderer() { }

		public override object Clone()
		{
			ScaleDependentRenderer clone = (ScaleDependentRenderer)this.MemberwiseClone();

			if (Renderer != null)
			{
				clone.Renderer = (Renderer)Renderer.Clone();
			}

			return clone;
		}

		public override void WriteTo(ArcXmlWriter writer)
		{
			try
			{
				writer.WriteStartElement(XmlName);

				if (!String.IsNullOrEmpty(Lower))
				{
					writer.WriteAttributeString("lower", Lower);
				}

				if (!String.IsNullOrEmpty(Upper))
				{
					writer.WriteAttributeString("upper", Upper);
				}

				if (Renderer != null)
				{
					Renderer.WriteTo(writer);
				}

				writer.WriteEndElement();
			}
			catch (Exception ex)
			{
				if (ex is ArcXmlException)
				{
					throw ex;
				}
				else
				{
					throw new ArcXmlException(String.Format("Could not write {0} object.", GetType().Name), ex);
				}
			}
		}
	}
}
