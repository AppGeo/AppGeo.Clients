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
using System.Xml;

namespace AppGeo.Clients.ArcIms.ArcXml
{
  [Serializable]
	public class TrueTypeMarkerSymbol : Symbol
	{
		public const string XmlName = "TRUETYPEMARKERSYMBOL";

		public static TrueTypeMarkerSymbol ReadFrom(ArcXmlReader reader)
		{
			try
			{
				TrueTypeMarkerSymbol trueTypeMarkerSymbol = new TrueTypeMarkerSymbol();

				if (reader.HasAttributes)
				{
					while (reader.MoveToNextAttribute())
					{
						string value = reader.ReadContentAsString();

						if (value.Length > 0)
						{
							switch (reader.Name)
							{
								case "angle": trueTypeMarkerSymbol.Angle = Convert.ToDouble(value); break;
								case "anglefield": trueTypeMarkerSymbol.AngleField = value; break;
								case "antialiasing": trueTypeMarkerSymbol.Antialiasing = Convert.ToBoolean(value); break;
								case "character": trueTypeMarkerSymbol.Character = Convert.ToUInt16(value); break;
								case "font": trueTypeMarkerSymbol.Font = value; break;
								case "fontcolor": trueTypeMarkerSymbol.FontColor = ColorConverter.ToColor(value); break;
								case "fontsize": trueTypeMarkerSymbol.FontSize = Convert.ToInt32(value); break;
								case "fontstyle": trueTypeMarkerSymbol.FontStyle = (FontStyle)ArcXmlEnumConverter.ToEnum(typeof(FontStyle), value); break;
								case "glowing": trueTypeMarkerSymbol.Glowing = ColorConverter.ToColor(value); break;
								case "outline": trueTypeMarkerSymbol.Outline = ColorConverter.ToColor(value); break;
								case "overlap": trueTypeMarkerSymbol.Overlap = Convert.ToBoolean(value); break;
								case "rotatemethod": trueTypeMarkerSymbol.RotateMethod = (RotateMethod)ArcXmlEnumConverter.ToEnum(typeof(RotateMethod), value); break;
								case "shadow": trueTypeMarkerSymbol.Shadow = ColorConverter.ToColor(value); break;
                case "transparency": trueTypeMarkerSymbol.Transparency = Convert.ToDouble(value); break;
                case "usecentroid": trueTypeMarkerSymbol.UseCentroid = Convert.ToBoolean(value); break;
							}
						}
					}

					reader.MoveToElement();
				}

				return trueTypeMarkerSymbol;
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
		public string AngleField = "";
		public bool Antialiasing;
		public ushort Character = 0;
		public string Font = "Arial";
		public Color FontColor = Color.Black;
		public int FontSize = 12;
		public FontStyle FontStyle = FontStyle.Regular;
		public Color Glowing = Color.Empty;
		public Color Outline = Color.Empty;
		public bool Overlap = true;
		public RotateMethod RotateMethod = RotateMethod.ModArithmetic;
		public Color Shadow = Color.Empty;
    public double Transparency = 1;
    public bool UseCentroid = false;

		public TrueTypeMarkerSymbol() { }

		public override object Clone()
		{
			TrueTypeMarkerSymbol clone = (TrueTypeMarkerSymbol)this.MemberwiseClone();
			return clone;
		}

		public override void WriteTo(ArcXmlWriter writer)
		{
			try
			{
				writer.WriteStartElement(XmlName);

				if (Angle >= 0)
				{
					writer.WriteAttributeString("angle", Angle.ToString("0.000"));
				}

				if (!String.IsNullOrEmpty(AngleField))
				{
					writer.WriteAttributeString("anglefield", AngleField);
				}

				if (Antialiasing)
				{
					writer.WriteAttributeString("antialiasing", "true");
				}

				if (Character >= 32)
				{
					writer.WriteAttributeString("character", Character.ToString());
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

				if (!Outline.IsEmpty)
				{
					writer.WriteAttributeString("outline", ColorConverter.ToArcXml(Outline));
				}

				if (!Overlap)
				{
					writer.WriteAttributeString("overlap", "false");
				}

				if (RotateMethod != RotateMethod.ModArithmetic)
				{
					writer.WriteAttributeString("type", ArcXmlEnumConverter.ToArcXml(typeof(RotateMethod), RotateMethod));
				}

				if (!Shadow.IsEmpty)
				{
					writer.WriteAttributeString("shadow", ColorConverter.ToArcXml(Shadow));
				}

        if (0 <= Transparency && Transparency < 1)
        {
          writer.WriteAttributeString("transparency", Transparency.ToString("0.000"));
        }

				writer.WriteAttributeString("usecentroid", UseCentroid ? "true" : "false");

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
