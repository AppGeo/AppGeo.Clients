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
  public class SimpleMarkerSymbol : Symbol
	{
		public const string XmlName = "SIMPLEMARKERSYMBOL";

		public static SimpleMarkerSymbol ReadFrom(ArcXmlReader reader)
		{
			try
			{
				SimpleMarkerSymbol simpleMarkerSymbol = new SimpleMarkerSymbol();

				if (reader.HasAttributes)
				{
					while (reader.MoveToNextAttribute())
					{
						string value = reader.ReadContentAsString();

						if (value.Length > 0)
						{
							switch (reader.Name)
							{
								case "antialiasing": simpleMarkerSymbol.Antialiasing = Convert.ToBoolean(value); break;
								case "color": simpleMarkerSymbol.Color = ColorConverter.ToColor(value); break;
								case "outline": simpleMarkerSymbol.Outline = ColorConverter.ToColor(value); break;
								case "overlap": simpleMarkerSymbol.Overlap = Convert.ToBoolean(value); break;
								case "shadow": simpleMarkerSymbol.Shadow = ColorConverter.ToColor(value); break;
                case "transparency": simpleMarkerSymbol.Transparency = Convert.ToDouble(value); break;
                case "type": simpleMarkerSymbol.Type = (MarkerType)ArcXmlEnumConverter.ToEnum(typeof(MarkerType), value); break;
								case "usecentroid": simpleMarkerSymbol.UseCentroid = Convert.ToBoolean(value); break;
								case "width": simpleMarkerSymbol.Width = Convert.ToInt32(value); break;
							}
						}
					}

					reader.MoveToElement();
				}

				return simpleMarkerSymbol;
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
		public Color Outline = Color.Empty;
    public bool Overlap = true;
		public Color Shadow = Color.Empty;
    public double Transparency = 1;
    public MarkerType Type = MarkerType.Circle;
    public bool UseCentroid = true;
    public int Width = 3;
    
    public SimpleMarkerSymbol() {}
    
    public SimpleMarkerSymbol(MarkerType type, Color color, int width)
    {
      Type = type;
      Color = color;
      Width = width;
    }

		public override object Clone()
		{
			SimpleMarkerSymbol clone = (SimpleMarkerSymbol)this.MemberwiseClone();
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

				if (!Outline.IsEmpty)
				{
					writer.WriteAttributeString("outline", ColorConverter.ToArcXml(Outline));
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

				if (Type != MarkerType.Circle)
				{
					writer.WriteAttributeString("type", ArcXmlEnumConverter.ToArcXml(typeof(MarkerType), Type));
				}

				writer.WriteAttributeString("usecentroid", UseCentroid ? "true" : "false");

				if (Width > 1)
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
