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
  public class SimpleLineSymbol : Symbol
	{
		public const string XmlName = "SIMPLELINESYMBOL";

		public static SimpleLineSymbol ReadFrom(ArcXmlReader reader)
		{
			try
			{
				SimpleLineSymbol simpleLineSymbol = new SimpleLineSymbol();

				if (reader.HasAttributes)
				{
					while (reader.MoveToNextAttribute())
					{
						string value = reader.ReadContentAsString();

						if (value.Length > 0)
						{
							switch (reader.Name)
							{
								case "antialiasing": simpleLineSymbol.Antialiasing = Convert.ToBoolean(value); break;
								case "captype": simpleLineSymbol.CapType = (CapType)ArcXmlEnumConverter.ToEnum(typeof(CapType), value); break;
								case "color": simpleLineSymbol.Color = ColorConverter.ToColor(value); break;
								case "jointype": simpleLineSymbol.JoinType = (JoinType)ArcXmlEnumConverter.ToEnum(typeof(JoinType), value); break;
								case "overlap": simpleLineSymbol.Overlap = Convert.ToBoolean(value); break;
                case "transparency": simpleLineSymbol.Transparency = Convert.ToDouble(value); break;
                case "type": simpleLineSymbol.Type = (LineType)ArcXmlEnumConverter.ToEnum(typeof(LineType), value); break;
								case "width": simpleLineSymbol.Width = Convert.ToInt32(value); break;
							}
						}
					}

					reader.MoveToElement();
				}

				return simpleLineSymbol;
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
    public CapType CapType = CapType.Butt;
		public Color Color = Color.Black;
		public JoinType JoinType = JoinType.Round;
    public bool Overlap = true;
    public double Transparency = 1;
    public LineType Type = LineType.Solid;
    public int Width = 1;
    
    public SimpleLineSymbol() {}
    
    public SimpleLineSymbol(Color color)
    {
      Color = color;
    }
    
    public SimpleLineSymbol(Color color, int width)
    {
      Color = color;
      Width = width;
    }
    
    public override object Clone()
    {
      SimpleLineSymbol clone = (SimpleLineSymbol)this.MemberwiseClone();
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

				if (CapType != CapType.Butt)
				{
					writer.WriteAttributeString("captype", ArcXmlEnumConverter.ToArcXml(typeof(CapType), CapType));
				}

        if (!Color.IsEmpty && Color != Color.Black)
        {
					writer.WriteAttributeString("color", ColorConverter.ToArcXml(Color));
				}

				if (JoinType != JoinType.Round)
				{
					writer.WriteAttributeString("jointype", ArcXmlEnumConverter.ToArcXml(typeof(JoinType), JoinType));
				}

				if (!Overlap)
				{
					writer.WriteAttributeString("overlap", "false");
				}

        if (0 <= Transparency && Transparency < 1)
        {
          writer.WriteAttributeString("transparency", Transparency.ToString("0.000"));
        }

				if (Type != LineType.Solid)
				{
					writer.WriteAttributeString("type", ArcXmlEnumConverter.ToArcXml(typeof(LineType), Type));
				}

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
