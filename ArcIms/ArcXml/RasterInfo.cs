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
  public class RasterInfo : ICloneable
  {
    public const string XmlName = "RASTER_INFO";

    public static RasterInfo ReadFrom(ArcXmlReader reader)
    {
      try
      {
        RasterInfo rasterInfo = new RasterInfo();

        if (!reader.IsEmptyElement)
        {
          reader.Read();

          while (!(reader.NodeType == XmlNodeType.EndElement && reader.Name == XmlName))
          {
            if (reader.NodeType == XmlNodeType.Element)
            {
              switch (reader.Name)
              {
                case Bands.XmlName: rasterInfo.Bands = Bands.ReadFrom(reader); break;
              }
            }

            reader.Read();
          }
        }

        return rasterInfo;
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

    public Bands Bands = null;

    public object Clone()
    {
      RasterInfo clone = (RasterInfo)this.MemberwiseClone();

      if (Bands != null)
      {
        clone.Bands = (Bands)Bands.Clone();
      }

      return clone;
    }
  }
}
