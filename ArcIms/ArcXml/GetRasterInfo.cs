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
using GeoAPI.Geometries;

namespace AppGeo.Clients.ArcIms.ArcXml
{
  [Serializable]
  public class GetRasterInfo : Request, ICloneable
  {
    public const string XmlName = "GET_RASTER_INFO";

    public string DataFrame = null;
    public string LayerID = null;
    public double X = 0;
    public double Y = 0;

    public CoordSys CoordSys = null;

    public GetRasterInfo(string layerID, IPoint p) : this(layerID, p.Coordinate.X, p.Coordinate.Y) { }

    public GetRasterInfo(string layerID, double x, double y)
    {
      LayerID = layerID;
      X = x;
      Y = y;
    }

    public GetRasterInfo(string layerID, IPoint p, CoordSys coordSys) : this(layerID, p.Coordinate.X, p.Coordinate.Y, coordSys) { }

    public GetRasterInfo(string layerID, double x, double y, CoordSys coordSys) : this(layerID, x, y)
    {
      CoordSys = coordSys;
    }


    public override object Clone()
    {
      GetRasterInfo clone = (GetRasterInfo)this.MemberwiseClone();

      if (CoordSys != null)
      {
        clone.CoordSys = (CoordSys)CoordSys.Clone();
      }

      return clone;
    }

    public override void WriteTo(ArcXmlWriter writer)
    {
      try
      {
        writer.WriteStartElement(XmlName);

        writer.WriteAttributeString("layerid", LayerID);
        writer.WriteAttributeString("x", X.ToString());
        writer.WriteAttributeString("y", Y.ToString());

        if (!String.IsNullOrEmpty(DataFrame))
        {
          writer.WriteAttributeString("dataframe", DataFrame);
        }

        if (CoordSys != null)
        {
          CoordSys.WriteTo(writer);
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
