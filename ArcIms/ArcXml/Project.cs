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
using System.Collections.Generic;
using System.Xml;
using GeoAPI.Geometries;

namespace AppGeo.Clients.ArcIms.ArcXml
{
  [Serializable]
  public class Project : Response
  {
    public const string XmlName = "PROJECT";

		public static Project ReadFrom(ArcXmlReader reader)
		{
			try
			{
				Project project = new Project();

				if (!reader.IsEmptyElement)
				{
					reader.Read();

					while (!(reader.NodeType == XmlNodeType.EndElement && reader.Name == XmlName))
					{
						if (reader.NodeType == XmlNodeType.Element)
						{
							switch (reader.Name)
							{
                case "ENVELOPE": project.Envelopes.Add(EnvelopeSerializer.ReadFrom(reader)); break;
								
                case "MULTIPOINT":
                  IMultiPoint multiPoint = (IMultiPoint)GeometrySerializer.ReadFrom(reader);
                  IGeometry shape = multiPoint.Count == 1 ? (IGeometry)multiPoint[0] : (IGeometry)multiPoint;
                  project.Shapes.Add(shape); 
                  break;
								
                case "POLYLINE":
                case "POLYGON":
                  project.Shapes.Add(GeometrySerializer.ReadFrom(reader)); 
                  break;
							}
						}

						reader.Read();
					}
				}

				return project;
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

    private List<Envelope> _envelopes = new List<Envelope>();
    private List<IGeometry> _shapes = new List<IGeometry>();

    public Project() { }

    public List<Envelope> Envelopes
    {
      get
      {
        return _envelopes;
      }
    }

    public List<IGeometry> Shapes
    {
      get
      {
        return _shapes;
      }
    }

    public override object Clone()
    {
      Project clone = (Project)this.MemberwiseClone();

      clone._envelopes = new List<Envelope>();

      foreach (Envelope envelope in _envelopes)
      {
        clone._envelopes.Add(envelope);
      }

      clone._shapes = new List<IGeometry>();

      foreach (IGeometry shape in _shapes)
      {
        clone._shapes.Add((IGeometry)shape.Clone());
      }

      return clone;
    }
  }
}
