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

namespace AppGeo.Clients.ArcIms.ArcXml
{
  [Serializable]
	public class CallOutMarkerSymbol : Symbol
	{
    public const string XmlName = "CALLOUTMARKERSYMBOL";

    public static CallOutMarkerSymbol ReadFrom(ArcXmlReader reader)
    {
      try
      {
        CallOutMarkerSymbol callOutMarkerSymbol = new CallOutMarkerSymbol();

        if (reader.HasAttributes)
        {
          while (reader.MoveToNextAttribute())
          {
            string value = reader.ReadContentAsString();

            if (value.Length > 0)
            {
              switch (reader.Name)
              {
                case "antialiasing": callOutMarkerSymbol.Antialiasing = Convert.ToBoolean(value); break;
                case "backcolor": callOutMarkerSymbol.BackColor = ColorConverter.ToColor(value); break;
                case "boundarycolor": callOutMarkerSymbol.BoundaryColor = ColorConverter.ToColor(value); break;
                case "font": callOutMarkerSymbol.Font = value; break;
                case "fontcolor": callOutMarkerSymbol.FontColor = ColorConverter.ToColor(value); break;
                case "fontsize": callOutMarkerSymbol.FontSize = Convert.ToInt32(value); break;
                case "fontstyle": callOutMarkerSymbol.FontStyle = (FontStyle)ArcXmlEnumConverter.ToEnum(typeof(FontStyle), value); break;
                case "glowing": callOutMarkerSymbol.Glowing = ColorConverter.ToColor(value); break;
                case "interval": callOutMarkerSymbol.Interval = Convert.ToInt32(value); break;
                case "outline": callOutMarkerSymbol.Outline = ColorConverter.ToColor(value); break;
                case "shadow": callOutMarkerSymbol.Shadow = ColorConverter.ToColor(value); break;
                case "transparency": callOutMarkerSymbol.Transparency = Convert.ToDouble(value); break;
              }
            }
          }

          reader.MoveToElement();
        }

        return callOutMarkerSymbol;
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
    public Color BackColor = Color.White;
    public Color BoundaryColor = Color.Black;
    public string Font = "Arial";
    public Color FontColor = Color.Black;
    public int FontSize = 12;
    public FontStyle FontStyle = FontStyle.Regular;
    public Color Glowing = Color.Empty;
    public int Interval = 0;
    public Color Outline = Color.Empty;
    public Color Shadow = Color.Empty;
    public double Transparency = 1;

    public CallOutMarkerSymbol() { }

    public override object Clone()
    {
      CallOutMarkerSymbol clone = (CallOutMarkerSymbol)this.MemberwiseClone();
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

        if (!BackColor.IsEmpty)
        {
          writer.WriteAttributeString("backcolor", ColorConverter.ToArcXml(BackColor));
        }

        if (!BoundaryColor.IsEmpty)
        {
          writer.WriteAttributeString("boundarycolor", ColorConverter.ToArcXml(BoundaryColor));
        }

        if (!String.IsNullOrEmpty(Font) && String.Compare(Font, "Arial", true) != 0)
        {
          writer.WriteAttributeString("font", Font);
        }

        if (!FontColor.IsEmpty && FontColor != Color.Black)
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

        if (!Glowing.IsEmpty)
        {
          writer.WriteAttributeString("glowing", ColorConverter.ToArcXml(Glowing));
        }

        if (Interval > 0)
        {
          writer.WriteAttributeString("interval", Interval.ToString());
        }

        if (!Outline.IsEmpty)
        {
          writer.WriteAttributeString("outline", ColorConverter.ToArcXml(Outline));
        }

        if (!Shadow.IsEmpty)
        {
          writer.WriteAttributeString("shadow", ColorConverter.ToArcXml(Shadow));
        }

        if (0 <= Transparency && Transparency < 1)
        {
          writer.WriteAttributeString("transparency", Transparency.ToString("0.000"));
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
