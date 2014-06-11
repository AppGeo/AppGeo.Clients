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
using System.Drawing;
using GeoAPI.Geometries;

namespace AppGeo.Clients.ArcIms.ArcXml
{
  [Serializable]
  public class RasterMarkerSymbol : Symbol
  {
    public const string XmlName = "RASTERMARKERSYMBOL";

    public static RasterMarkerSymbol ReadFrom(ArcXmlReader reader)
    {
      try
      {
        RasterMarkerSymbol rasterMarkerSymbol = new RasterMarkerSymbol();

        if (reader.HasAttributes)
        {
          while (reader.MoveToNextAttribute())
          {
            string value = reader.ReadContentAsString();

            if (value.Length > 0)
            {
              switch (reader.Name)
              {
                case "antialiasing": rasterMarkerSymbol.Antialiasing = Convert.ToBoolean(value); break;
                case "image": rasterMarkerSymbol.Image = value; break;
                case "overlap": rasterMarkerSymbol.Overlap = Convert.ToBoolean(value); break;
                case "shadow": rasterMarkerSymbol.Shadow = ColorConverter.ToColor(value); break;
                case "transparency": rasterMarkerSymbol.Transparency = Convert.ToDouble(value); break;
                case "url": rasterMarkerSymbol.Url = value; break;
                case "usecentroid": rasterMarkerSymbol.UseCentroid = Convert.ToBoolean(value); break;

                case "hotspot":
                  string[] p = value.Split(new char[] { ',' });
                  rasterMarkerSymbol.Hotspot = new NetTopologySuite.Geometries.Point(Convert.ToDouble(p[0]), Convert.ToDouble(p[1]));
                  break;

                case "size":
                  string[] d = value.Split(new char[] { ',' });
                  rasterMarkerSymbol.Width = Convert.ToInt32(d[0]);
                  rasterMarkerSymbol.Height = Convert.ToInt32(d[1]);
                  break;
              }
            }
          }

          reader.MoveToElement();
        }

        return rasterMarkerSymbol;
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

    public bool Antialiasing = false;
    public int Height = 0;
    public IPoint Hotspot = null;
    public string Image = null;
    public bool Overlap = true;
    public Color Shadow = Color.Empty;
    public double Transparency = 1;
    public string Url = null;
    public bool UseCentroid = false;
    public int Width = 0;

    public RasterMarkerSymbol() { }

    public override object Clone()
    {
      RasterMarkerSymbol clone = (RasterMarkerSymbol)this.MemberwiseClone();
      return clone;
    }

    public override void WriteTo(ArcXmlWriter writer)
    {
      try
      {
        writer.WriteStartElement(XmlName);

        if (Antialiasing)
        {
          writer.WriteAttributeString("antialiasing", "true");
        }

        if (Hotspot != null)
        {
          writer.WriteAttributeString("hotspot", String.Format("{0},{1}", Hotspot.Coordinate.X, Hotspot.Coordinate.Y));
        }

        if (!String.IsNullOrEmpty(Image))
        {
          writer.WriteAttributeString("image", Image);
        }

        if (!Overlap)
        {
          writer.WriteAttributeString("overlap", "false");
        }

        if (!Shadow.IsEmpty)
        {
          writer.WriteAttributeString("shadow", ColorConverter.ToArcXml(Shadow));
        }

        if (0 <= Transparency && Transparency < 1)
        {
          writer.WriteAttributeString("transparency", Transparency.ToString("0.000"));
        }

        if (!String.IsNullOrEmpty(Url))
        {
          writer.WriteAttributeString("url", Url);
        }

        if (UseCentroid)
        {
          writer.WriteAttributeString("usecentroid", "true");
        }

        if (Width > 0 && Height > 0)
        {
          writer.WriteAttributeString("size", String.Format("{0},{1}", Width, Height));
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
