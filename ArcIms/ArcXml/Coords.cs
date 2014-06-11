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
using NetTopologySuite.Geometries;

namespace AppGeo.Clients.ArcIms.ArcXml
{
  [Serializable]
  public class Coords : List<IPoint>, ICloneable
  {
    public const string XmlName = "COORDS";

    public static Coords ReadFrom(ArcXmlReader reader)
    {
      try
      {
        Coords coords = new Coords();

        if (!reader.IsEmptyElement)
        {
          reader.Read();

          while (!(reader.NodeType == XmlNodeType.EndElement && reader.Name == XmlName))
          {
            if (reader.NodeType == XmlNodeType.Text)
            {
              string[] tuples = reader.ReadContentAsString().Split(reader.TupleSeparator);

              if (tuples.Length > 0)
              {
                for (int i = 0; i < tuples.Length; ++i)
                {
                  string[] c = tuples[i].Split(reader.CoordinateSeparator);
                  coords.Add(new Point(Convert.ToDouble(c[0]), Convert.ToDouble(c[1])));
                }
              }
            }

            reader.Read();
          }
        }

        return coords;
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

		public Coords() { }

    public Coords(IEnumerable<IPoint> points) : base(points) { }

    public object Clone()
    {
      Coords clone = new Coords();

      foreach (Point point in this)
      {
        clone.Add((Point)point.Clone());
      }

      return clone;
    }

    public void WriteTo(ArcXmlWriter writer)
    {
      try
      {
        writer.WriteStartElement(XmlName);

        string[] coords = new string[Count];

        for (int i = 0; i < Count; ++i)
        {
          coords[i] = String.Format("{0}{1}{2}", this[i].X, writer.CoordinateSeparator[0], this[i].X);
        }

        writer.WriteString(String.Join(writer.TupleSeparator[0].ToString(), coords));

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
