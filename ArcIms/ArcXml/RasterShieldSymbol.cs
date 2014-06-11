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
  public class RasterShieldSymbol : Symbol
  {
    public const string XmlName = "RASTERSHIELDSYMBOL";

    public static RasterShieldSymbol ReadFrom(ArcXmlReader reader)
    {
      try
      {
        RasterShieldSymbol rasterShieldSymbol = new RasterShieldSymbol();

        if (reader.HasAttributes)
        {
          while (reader.MoveToNextAttribute())
          {
            string value = reader.ReadContentAsString();

            if (value.Length > 0)
            {
              switch (reader.Name)
              {
                case "antialiasing": rasterShieldSymbol.Antialiasing = Convert.ToBoolean(value); break;
                case "boundary": rasterShieldSymbol.Boundary = Convert.ToBoolean(value); break;
                case "font": rasterShieldSymbol.Font = value; break;
                case "fontcolor": rasterShieldSymbol.FontColor = ColorConverter.ToColor(value); break;
                case "fontsize": rasterShieldSymbol.FontSize = Convert.ToInt32(value); break;
                case "fontstyle": rasterShieldSymbol.FontStyle = (FontStyle)ArcXmlEnumConverter.ToEnum(typeof(FontStyle), value); break;
                case "image": rasterShieldSymbol.Image = value; break;
                case "labelmode": rasterShieldSymbol.LabelMode = (ShieldLabelMode)ArcXmlEnumConverter.ToEnum(typeof(ShieldLabelMode), value); break;
                case "printmode": rasterShieldSymbol.PrintMode = (PrintMode)ArcXmlEnumConverter.ToEnum(typeof(PrintMode), value); break;
                case "shadow": rasterShieldSymbol.Shadow = ColorConverter.ToColor(value); break;
                case "transparency": rasterShieldSymbol.Transparency = Convert.ToDouble(value); break;
                case "url": rasterShieldSymbol.Url = value; break;

                case "textposition":
                  string[] p = value.Split(new char[] { ',' });
                  rasterShieldSymbol.TextPosition = new NetTopologySuite.Geometries.Point(Convert.ToDouble(p[0]), Convert.ToDouble(p[1]));
                  break;
              }
            }
          }

          reader.MoveToElement();
        }

        return rasterShieldSymbol;
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
    public bool Boundary = false;
    public string Font = "Arial";
    public Color FontColor = Color.Black;
    public int FontSize = 12;
    public FontStyle FontStyle = FontStyle.Regular;
    public string Image = null;
    public ShieldLabelMode LabelMode = ShieldLabelMode.NumericOnly;
    public PrintMode PrintMode = PrintMode.None;
    public Color Shadow = Color.Empty;
    public string Url = null;
    public IPoint TextPosition = null;
    public double Transparency = 1;

    public RasterShieldSymbol() { }

    public override object Clone()
    {
      RasterShieldSymbol clone = (RasterShieldSymbol)this.MemberwiseClone();
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

        if (Boundary)
        {
          writer.WriteAttributeString("antialiasing", "true");
        }

        if (!String.IsNullOrEmpty(Font) && String.Compare(Font, "Arial", true) != 0)
        {
          writer.WriteAttributeString("font", Font);
        }

        if (FontColor != Color.Black)
        {
          writer.WriteAttributeString("fontcolor", ColorConverter.ToArcXml(FontColor));
        }

        if (FontSize != 12)
        {
          writer.WriteAttributeString("fontsize", FontSize.ToString());
        }

        if (FontStyle != FontStyle.Regular)
        {
          writer.WriteAttributeString("fontstyle", ArcXmlEnumConverter.ToArcXml(typeof(FontStyle), FontStyle));
        }

        if (!String.IsNullOrEmpty(Image))
        {
          writer.WriteAttributeString("image", Image);
        }

        if (LabelMode != ShieldLabelMode.NumericOnly)
        {
          writer.WriteAttributeString("labelmode", ArcXmlEnumConverter.ToArcXml(typeof(ShieldLabelMode), LabelMode));
        }

        if (PrintMode != PrintMode.None)
        {
          writer.WriteAttributeString("printmode", ArcXmlEnumConverter.ToArcXml(typeof(PrintMode), PrintMode));
        }

        if (!Shadow.IsEmpty)
        {
          writer.WriteAttributeString("shadow", ColorConverter.ToArcXml(Shadow));
        }

        if (TextPosition != null)
        {
          writer.WriteAttributeString("textposition", String.Format("{0},{1}", TextPosition.Coordinate.X, TextPosition.Coordinate.Y));
        }

        if (0 <= Transparency && Transparency < 1)
        {
          writer.WriteAttributeString("transparency", Transparency.ToString("0.000"));
        }

        if (!String.IsNullOrEmpty(Url))
        {
          writer.WriteAttributeString("url", Url);
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
