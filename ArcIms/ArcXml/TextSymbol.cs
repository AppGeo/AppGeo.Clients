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
  public class TextSymbol : Symbol
	{
		public const string XmlName = "TEXTSYMBOL";

		public static TextSymbol ReadFrom(ArcXmlReader reader)
		{
			try
			{
				TextSymbol textSymbol = new TextSymbol();

				if (reader.HasAttributes)
				{
					while (reader.MoveToNextAttribute())
					{
						string value = reader.ReadContentAsString();

						if (value.Length > 0)
						{
							switch (reader.Name)
							{
								case "antialiasing": textSymbol.Antialiasing = Convert.ToBoolean(value); break;
								case "blockout": textSymbol.BlockOut = ColorConverter.ToColor(value); break;
								case "font": textSymbol.Font = value; break;
								case "fontcolor": textSymbol.FontColor = ColorConverter.ToColor(value); break;
								case "fontsize": textSymbol.FontSize = reader.ReadContentAsInt(); break;
								case "fontstyle": textSymbol.FontStyle = (FontStyle)ArcXmlEnumConverter.ToEnum(typeof(FontStyle), value); break;
								case "glowing": textSymbol.Glowing = ColorConverter.ToColor(value); break;
								case "interval": textSymbol.Interval = Convert.ToInt32(value); break;
								case "outline": textSymbol.Outline = ColorConverter.ToColor(value); break;
								case "printmode": textSymbol.PrintMode = (PrintMode)ArcXmlEnumConverter.ToEnum(typeof(PrintMode), value); break;
								case "shadow": textSymbol.Shadow = ColorConverter.ToColor(value); break;
                case "transparency": textSymbol.Transparency = Convert.ToDouble(value); break;
              }
						}
					}

					reader.MoveToElement();
				}

				return textSymbol;
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
    public Color BlockOut = Color.Empty;
    public string Font = "Arial";
		public Color FontColor = Color.Black;
		public int FontSize = 12;
    public FontStyle FontStyle = FontStyle.Regular;
    public Color Glowing = Color.Empty;
    public int Interval = 0;
    public Color Outline = Color.Empty;
    public PrintMode PrintMode = PrintMode.None;
    public Color Shadow = Color.Empty;
    public double Transparency = 1;

		public TextSymbol() {}
    
    public TextSymbol(string font, int fontSize)
    {
      Font = font;
      FontSize = fontSize;
    }
    
    public TextSymbol(string font, int fontSize, FontStyle fontStyle, Color fontColor)
    {
      Font = font;
      FontSize = fontSize;
      FontStyle = fontStyle;
      FontColor = fontColor;
    }

    public override object Clone()
    {
      TextSymbol clone = (TextSymbol)this.MemberwiseClone();
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

				if (Interval > 0)
				{
					writer.WriteAttributeString("interval", Interval.ToString());
				}

				if (!Outline.IsEmpty)
				{
					writer.WriteAttributeString("outline", ColorConverter.ToArcXml(Outline));
				}

				if (PrintMode != PrintMode.None)
				{
					writer.WriteAttributeString("printmode", ArcXmlEnumConverter.ToArcXml(typeof(PrintMode), PrintMode));
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
