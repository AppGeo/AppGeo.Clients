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
	public class TextMarkerSymbol : TextSymbol
	{
		public new const string XmlName = "TEXTMARKERSYMBOL";

    public new static TextMarkerSymbol ReadFrom(ArcXmlReader reader)
    {
      try
      {
        TextMarkerSymbol textMarkerSymbol = new TextMarkerSymbol();

        if (reader.HasAttributes)
        {
          while (reader.MoveToNextAttribute())
          {
            string value = reader.ReadContentAsString();

            if (value.Length > 0)
            {
              switch (reader.Name)
              {
                case "angle": textMarkerSymbol.Angle = Convert.ToDouble(value); break;
                case "antialiasing": textMarkerSymbol.Antialiasing = Convert.ToBoolean(value); break;
                case "blockout": textMarkerSymbol.BlockOut = ColorConverter.ToColor(value); break;
                case "font": textMarkerSymbol.Font = value; break;
                case "fontcolor": textMarkerSymbol.FontColor = ColorConverter.ToColor(value); break;
                case "fontsize": textMarkerSymbol.FontSize = reader.ReadContentAsInt(); break;
                case "fontstyle": textMarkerSymbol.FontStyle = (FontStyle)ArcXmlEnumConverter.ToEnum(typeof(FontStyle), value); break;
                case "glowing": textMarkerSymbol.Glowing = ColorConverter.ToColor(value); break;
                case "halignment": textMarkerSymbol.HAlignment = (HorizontalAlignment)ArcXmlEnumConverter.ToEnum(typeof(HorizontalAlignment), value); break;
                case "interval": textMarkerSymbol.Interval = Convert.ToInt32(value); break;
                case "outline": textMarkerSymbol.Outline = ColorConverter.ToColor(value); break;
                case "overlap": textMarkerSymbol.Overlap = Convert.ToBoolean(value); break;
                case "printmode": textMarkerSymbol.PrintMode = (PrintMode)ArcXmlEnumConverter.ToEnum(typeof(PrintMode), value); break;
                case "shadow": textMarkerSymbol.Shadow = ColorConverter.ToColor(value); break;
                case "transparency": textMarkerSymbol.Transparency = Convert.ToDouble(value); break;
                case "valignment": textMarkerSymbol.VAlignment = (VerticalAlignment)ArcXmlEnumConverter.ToEnum(typeof(VerticalAlignment), value); break;
              }
            }
          }

          reader.MoveToElement();
        }

        return textMarkerSymbol;
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

		public double Angle = 0;
		public HorizontalAlignment HAlignment = HorizontalAlignment.Right;
		public bool Overlap = true;
		public VerticalAlignment VAlignment = VerticalAlignment.Top;

		public TextMarkerSymbol() { }

		public TextMarkerSymbol(string font, int fontSize) : base(font, fontSize) { }

		public TextMarkerSymbol(string font, int fontSize, FontStyle fontStyle, Color fontColor)
			: base(font, fontSize, fontStyle, fontColor) { }

		public override object Clone()
		{
			TextMarkerSymbol clone = (TextMarkerSymbol)this.MemberwiseClone();
			return clone;
		}

		public override void WriteTo(ArcXmlWriter writer)
		{
			try
			{
				writer.WriteStartElement(XmlName);

				if (Angle > 0)
				{
					writer.WriteAttributeString("angle", Angle.ToString());
				}

				if (Antialiasing)
				{
					writer.WriteAttributeString("antialiasing", "true");
				}

				if (!BlockOut.IsEmpty)
				{
					writer.WriteAttributeString("blockout", ColorConverter.ToArcXml(BlockOut));
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

				if (HAlignment != HorizontalAlignment.Right)
				{
					writer.WriteAttributeString("halignment", ArcXmlEnumConverter.ToArcXml(typeof(HorizontalAlignment), HAlignment));
				}

				if (Interval > 0)
				{
					writer.WriteAttributeString("interval", Interval.ToString());
				}

				if (!Outline.IsEmpty)
				{
					writer.WriteAttributeString("outline", ColorConverter.ToArcXml(Outline));
				}

				if (!Overlap)
				{
					writer.WriteAttributeString("overlap", "false");
				}

				if (PrintMode != PrintMode.None)
				{
					writer.WriteAttributeString("printmode", ArcXmlEnumConverter.ToArcXml(typeof(PrintMode), PrintMode));
				}

        if (0 <= Transparency && Transparency < 1)
        {
          writer.WriteAttributeString("transparency", Transparency.ToString("0.000"));
        }

				if (VAlignment != VerticalAlignment.Top)
				{
					writer.WriteAttributeString("valignment", ArcXmlEnumConverter.ToArcXml(typeof(VerticalAlignment), VAlignment));
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
