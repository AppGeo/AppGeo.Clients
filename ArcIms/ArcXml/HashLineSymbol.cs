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
	public class HashLineSymbol : Symbol
	{
		public const string XmlName = "HASHLINESYMBOL";

		public static HashLineSymbol ReadFrom(ArcXmlReader reader)
		{
			try
			{
				HashLineSymbol hashLineSymbol = new HashLineSymbol();

				if (reader.HasAttributes)
				{
					while (reader.MoveToNextAttribute())
					{
						string value = reader.ReadContentAsString();

						if (value.Length > 0)
						{
							switch (reader.Name)
							{
								case "antialiasing": hashLineSymbol.Antialiasing = Convert.ToBoolean(value); break;
								case "color": hashLineSymbol.Color = ColorConverter.ToColor(value); break;
								case "interval": hashLineSymbol.Interval = Convert.ToInt32(value); break;
								case "linethickness": hashLineSymbol.LineThickness = Convert.ToInt32(value); break;
								case "tickthickness": hashLineSymbol.TickThickness = Convert.ToInt32(value); break;
								case "overlap": hashLineSymbol.Overlap = Convert.ToBoolean(value); break;
                case "transparency": hashLineSymbol.Transparency = Convert.ToDouble(value); break;
                case "type": hashLineSymbol.Type = (HashLineType)ArcXmlEnumConverter.ToEnum(typeof(HashLineType), value); break;
								case "width": hashLineSymbol.Width = Convert.ToInt32(value); break;
							}
						}
					}

					reader.MoveToElement();
				}

				return hashLineSymbol;
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
		public Color Color = Color.Black;
		public int Interval = 8;
		public int LineThickness = 1;
		public bool Overlap = true;
    public int TickThickness = 1;
    public double Transparency = 1;
    public HashLineType Type = HashLineType.Foreground;
		public int Width = 6;

		public HashLineSymbol() { }

		public HashLineSymbol(Color color)
		{
			Color = color;
		}

		public HashLineSymbol(Color color, int thickness, int interval, int width)
		{
			Color = color;
			LineThickness = thickness;
			TickThickness = thickness;
			Interval = interval;
			Width = width;
		}

		public override object Clone()
		{
			HashLineSymbol clone = (HashLineSymbol)this.MemberwiseClone();
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

				if (!Color.IsEmpty && Color != Color.Black)
				{
					writer.WriteAttributeString("color", ColorConverter.ToArcXml(Color));
				}

				if (Interval != 8 && Interval >= 0)
				{
					writer.WriteAttributeString("interval", Interval.ToString());
				}

				if (LineThickness > 1)
				{
					writer.WriteAttributeString("linethickness", LineThickness.ToString());
				}

				if (TickThickness > 1)
				{
					writer.WriteAttributeString("tickthickness", TickThickness.ToString());
				}

				if (!Overlap)
				{
					writer.WriteAttributeString("overlap", "false");
				}

        if (0 <= Transparency && Transparency < 1)
        {
          writer.WriteAttributeString("transparency", Transparency.ToString("0.000"));
        }

				if (Type != HashLineType.Foreground)
				{
					writer.WriteAttributeString("type", ArcXmlEnumConverter.ToArcXml(typeof(HashLineType), Type));
				}

				if (Width != 6 && Width >= 0)
				{
					writer.WriteAttributeString("width", Width.ToString());
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
