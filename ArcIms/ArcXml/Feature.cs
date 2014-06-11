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
	public class Feature : ICloneable
	{
		public const string XmlName = "FEATURE";

		public static Feature ReadFrom(ArcXmlReader reader)
		{
			try
			{
				Feature feature = new Feature();

				if (reader.HasAttributes)
				{
					while (reader.MoveToNextAttribute())
					{
						string value = reader.ReadContentAsString();

						if (value.Length > 0)
						{
							switch (reader.Name)
							{
								case "featureid": feature.FeatureID = value; break;
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
								case "ENVELOPE": feature.Envelope = EnvelopeSerializer.ReadFrom(reader); break;
								case Fields.XmlName: feature.Fields = Fields.ReadFrom(reader); break;
								case Field.XmlName: feature.Fields.Add(Field.ReadFrom(reader)); break;
                
                case "MULTIPOINT":
                  IMultiPoint multiPoint = GeometrySerializer.ReadMultiPointFrom(reader);
                  feature.Shape = multiPoint.Count == 1 ? (IGeometry)multiPoint[0] : (IGeometry)multiPoint;
                  break;

                case "POLYLINE": feature.Shape = GeometrySerializer.ReadPolylineFrom(reader); break;
                case "POLYGON": feature.Shape = GeometrySerializer.ReadPolygonFrom(reader); break;
              }
						}

						reader.Read();
					}
				}

				return feature;
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

		public string FeatureID = null;
		public Envelope Envelope = new Envelope();
    public Fields Fields = new Fields();
    public IGeometry Shape = null;

		public Feature() { }

		public object Clone()
		{
			Feature clone = (Feature)this.MemberwiseClone();

      if (Fields != null)
      {
        clone.Fields = (Fields)Fields.Clone();
      }

			if (Shape != null)
			{
        clone.Shape = (IGeometry)Shape.Clone();
			}

			return clone;
		}
	}
}
