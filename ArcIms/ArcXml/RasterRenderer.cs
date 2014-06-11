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
  public class RasterRenderer : Renderer
  {
    public const string XmlName = "RASTER_RENDERER";

    public static RasterRenderer ReadFrom(ArcXmlReader reader)
    {
      try
      {
        RasterRenderer rasterRenderer = new RasterRenderer();

        if (!reader.IsEmptyElement)
        {
          reader.Read();

          while (!(reader.NodeType == XmlNodeType.EndElement && reader.Name == XmlName))
          {
            if (reader.NodeType == XmlNodeType.Element)
            {
              switch (reader.Name)
              {
                case RasterExact.XmlName: rasterRenderer.Classifications.Add(RasterExact.ReadFrom(reader)); break;
                case RasterRange.XmlName: rasterRenderer.Classifications.Add(RasterRange.ReadFrom(reader)); break;
                case RasterOther.XmlName: rasterRenderer.Classifications.Add(RasterOther.ReadFrom(reader)); break;
              }
            }

            reader.Read();
          }
        }

        return rasterRenderer;
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

    private List<RasterClassification> _classifications = new List<RasterClassification>();

    public RasterRenderer() { }

    public List<RasterClassification> Classifications
    {
      get
      {
        return _classifications;
      }
    }

    public override object Clone()
    {
      RasterRenderer clone = (RasterRenderer)this.MemberwiseClone();

      foreach (RasterClassification classification in _classifications)
      {
        clone.Classifications.Add((RasterClassification)classification.Clone());
      }

      return clone;
    }

    public override void WriteTo(ArcXmlWriter writer)
    {
    }
  }
}
