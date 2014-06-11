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
	public class ShieldSymbol : Symbol
	{
		public const string XmlName = "SHIELDSYMBOL";

		public static ShieldSymbol ReadFrom(ArcXmlReader reader)
		{
			try
			{
				ShieldSymbol shieldSymbol = new ShieldSymbol();

				if (reader.HasAttributes)
				{
					while (reader.MoveToNextAttribute())
					{
						string value = reader.ReadContentAsString();

						if (value.Length > 0)
						{
							switch (reader.Name)
							{
								case "antialiasing": shieldSymbol.Antialiasing = Convert.ToBoolean(value); break;
                case "bottomcolor": shieldSymbol.BottomColor = ColorConverter.ToColor(value); break;
                case "font": shieldSymbol.Font = value; break;
								case "fontcolor": shieldSymbol.FontColor = ColorConverter.ToColor(value); break;
								case "fontsize": shieldSymbol.FontSize = Convert.ToInt32(value); break;
								case "fontstyle": shieldSymbol.FontStyle = (FontStyle)ArcXmlEnumConverter.ToEnum(typeof(FontStyle), value); break;
								case "labelmode": shieldSymbol.LabelMode = (ShieldLabelMode)ArcXmlEnumConverter.ToEnum(typeof(ShieldLabelMode),value); break;
                case "middlecolor": shieldSymbol.MiddleColor = ColorConverter.ToColor(value); break;
                case "minsize": shieldSymbol.MinSize = Convert.ToInt32(value); break;
								case "shadow": shieldSymbol.Shadow = ColorConverter.ToColor(value); break;
                case "topcolor": shieldSymbol.TopColor = ColorConverter.ToColor(value); break;
                case "transparency": shieldSymbol.Transparency = Convert.ToDouble(value); break;
              }
						}
					}

					reader.MoveToElement();
				}

				return shieldSymbol;
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

		public bool Antialiasing = true;
    public Color BottomColor = Color.FromArgb(0, 0, 250);
    public string Font = "Arial";
		public Color FontColor = Color.Black;
		public int FontSize = 12;
		public FontStyle FontStyle = FontStyle.Regular;
		public ShieldLabelMode LabelMode = ShieldLabelMode.NumericOnly;
    public Color MiddleColor = Color.Black;
    public int MinSize = 1;
		public Color Shadow = Color.Empty;
    public Color TopColor = Color.FromArgb(250, 0, 0);
    public ShieldType Type = ShieldType.Interstate;
    public double Transparency = 1;

		public ShieldSymbol() { }

		public ShieldSymbol(ShieldType type)
		{
			Type = type;
		}

		public override object Clone()
		{
			ShieldSymbol clone = (ShieldSymbol)this.MemberwiseClone();
			return clone;
		}

		public override void WriteTo(ArcXmlWriter writer)
		{
			try
			{
				writer.WriteStartElement(XmlName);

				if (!Antialiasing)
				{
					writer.WriteAttributeString("antialiasing", "false");
				}

        if (!BottomColor.IsEmpty && BottomColor != Color.FromArgb(0, 0, 250))
        {
          writer.WriteAttributeString("bottomcolor", ColorConverter.ToArcXml(BottomColor));
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

				if (LabelMode != ShieldLabelMode.NumericOnly)
				{
					writer.WriteAttributeString("labelmode", ArcXmlEnumConverter.ToArcXml(typeof(ShieldLabelMode), LabelMode));
				}

        if (!MiddleColor.IsEmpty && MiddleColor != Color.FromArgb(0, 0, 250))
        {
          writer.WriteAttributeString("middlecolor", ColorConverter.ToArcXml(MiddleColor));
        }

				if (MinSize != 12)
				{
					writer.WriteAttributeString("minsize", MinSize.ToString());
				}

				if (!Shadow.IsEmpty)
				{
					writer.WriteAttributeString("shadow", ColorConverter.ToArcXml(Shadow));
				}

        if (!TopColor.IsEmpty && TopColor != Color.FromArgb(250, 0, 0))
        {
          writer.WriteAttributeString("topcolor", ColorConverter.ToArcXml(TopColor));
        }

        if (0 <= Transparency && Transparency < 1)
        {
          writer.WriteAttributeString("transparency", Transparency.ToString("0.000"));
        }

				if (Type != ShieldType.Interstate)
				{
					writer.WriteAttributeString("type", ArcXmlEnumConverter.ToArcXml(typeof(ShieldType), Type));
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
