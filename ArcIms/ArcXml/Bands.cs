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

namespace AppGeo.Clients.ArcIms.ArcXml
{
  [Serializable]
  public class Bands : List<Band>, ICloneable
  {
    public const string XmlName = "BANDS";

    public static Bands ReadFrom(ArcXmlReader reader)
    {
      try
      {
        Bands bands = new Bands();

        if (reader.HasAttributes)
        {
          while (reader.MoveToNextAttribute())
          {
            string value = reader.ReadContentAsString();

            if (value.Length > 0)
            {
              switch (reader.Name)
              {
                case "rasterid": bands.RasterID = value; break;
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
                case Band.XmlName: bands.Add(Band.ReadFrom(reader)); break;
              }
            }

            reader.Read();
          }
        }

        return bands;
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

    public string RasterID = null;

    public Bands() { }

    public object Clone()
    {
      Bands clone = new Bands();
      clone.RasterID = RasterID;

      foreach (Band band in this)
      {
        clone.Add((Band)band.Clone());
      }

      return clone;
    }
  }
}
