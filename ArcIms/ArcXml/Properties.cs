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
using GeoAPI.Geometries;

namespace AppGeo.Clients.ArcIms.ArcXml
{
  [Serializable]
	public class Properties : ICloneable
	{
		public const string XmlName = "PROPERTIES";

		public static Properties ReadFrom(ArcXmlReader reader)
		{
			try
			{
				Properties properties = new Properties();

				if (!reader.IsEmptyElement)
				{
					reader.Read();

					while (!(reader.NodeType == XmlNodeType.EndElement && reader.Name == XmlName))
					{
						if (reader.NodeType == XmlNodeType.Element)
						{
							switch (reader.Name)
							{
								case Background.XmlName: properties.Background = Background.ReadFrom(reader); break;
                case "ENVELOPE": properties.Envelope = EnvelopeSerializer.ReadFrom(reader); break;
                case FeatureCoordSys.XmlName: properties.FeatureCoordSys = FeatureCoordSys.ReadFrom(reader); break;
								case FilterCoordSys.XmlName: properties.FilterCoordSys = FilterCoordSys.ReadFrom(reader); break;
                case ImageGeneralization.XmlName: properties.ImageGeneralization = ImageGeneralization.ReadFrom(reader); break;
                case ImageSize.XmlName: properties.ImageSize = ImageSize.ReadFrom(reader); break;
								case MapUnits.XmlName: properties.MapUnits = MapUnits.ReadFrom(reader); break;
								case Output.XmlName: properties.Output = Output.ReadFrom(reader); break;
							}
						}

						reader.Read();
					}
				}

				return properties;
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

		public Background Background = null;
    public Envelope Envelope = new Envelope();
		public FeatureCoordSys FeatureCoordSys = null;
		public FilterCoordSys FilterCoordSys = null;
    public ImageGeneralization ImageGeneralization = null;
		public ImageSize ImageSize = null;
		public LayerList LayerList = null;
		public MapUnits MapUnits = null;
		public Output Output = null;

		public Properties() { }

		public object Clone()
		{
			Properties clone = (Properties)this.MemberwiseClone();

			if (Background != null)
			{
				clone.Background = (Background)Background.Clone();
			}

			if (FeatureCoordSys != null)
			{
				clone.FeatureCoordSys = (FeatureCoordSys)FeatureCoordSys.Clone();
			}

			if (FilterCoordSys != null)
			{
				clone.FilterCoordSys = (FilterCoordSys)FilterCoordSys.Clone();
			}

			if (ImageSize != null)
			{
				clone.ImageSize = (ImageSize)ImageSize.Clone();
			}

			if (LayerList != null)
			{
				clone.LayerList = (LayerList)LayerList.Clone();
			}

			if (MapUnits != null)
			{
				clone.MapUnits = (MapUnits)MapUnits.Clone();
			}

			if (Output != null)
			{
				clone.Output = (Output)Output.Clone();
			}

			return clone;
		}

		public void WriteTo(ArcXmlWriter writer)
		{
			try
			{
				writer.WriteStartElement(XmlName);

				if (Background != null)
				{
					Background.WriteTo(writer);
				}

        if (!Envelope.IsNull)
				{
					EnvelopeSerializer.WriteTo(writer, Envelope);
				}

				if (FeatureCoordSys != null)
				{
					FeatureCoordSys.WriteTo(writer);
				}

				if (FilterCoordSys != null)
				{
					FilterCoordSys.WriteTo(writer);
				}

				if (ImageSize != null)
				{
					ImageSize.WriteTo(writer);
				}

				if (LayerList != null)
				{
					LayerList.WriteTo(writer);
				}

				if (Output != null)
				{
					Output.WriteTo(writer);
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
