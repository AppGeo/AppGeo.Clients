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
  public class SimplePolygonSymbol : Symbol
	{
		public const string XmlName = "SIMPLEPOLYGONSYMBOL";

		public static SimplePolygonSymbol ReadFrom(ArcXmlReader reader)
		{
			try
			{
				SimplePolygonSymbol simplePolygonSymbol = new SimplePolygonSymbol();

				if (reader.HasAttributes)
				{
					while (reader.MoveToNextAttribute())
					{
						string value = reader.ReadContentAsString();

						if (value.Length > 0)
						{
							switch (reader.Name)
							{
								case "antialiasing": simplePolygonSymbol.Antialiasing = Convert.ToBoolean(value); break;
								case "boundary": simplePolygonSymbol.Boundary = Convert.ToBoolean(value); break;
								case "boundarycaptype": simplePolygonSymbol.BoundaryCapType = (CapType)ArcXmlEnumConverter.ToEnum(typeof(CapType), value); break;
								case "boundarycolor": simplePolygonSymbol.BoundaryColor = Color.FromArgb(simplePolygonSymbol.BoundaryColor.A, ColorConverter.ToColor(value)); break;
								case "boundaryjointype": simplePolygonSymbol.BoundaryJoinType = (JoinType)ArcXmlEnumConverter.ToEnum(typeof(JoinType), value); break;
								case "boundarytype": simplePolygonSymbol.BoundaryType = (LineType)ArcXmlEnumConverter.ToEnum(typeof(LineType), value); break;
								case "boundarywidth": simplePolygonSymbol.BoundaryWidth = Convert.ToInt32(value); break;
								case "fillcolor": simplePolygonSymbol.FillColor = Color.FromArgb(simplePolygonSymbol.FillColor.A, ColorConverter.ToColor(value)); break;
								case "fillinterval": simplePolygonSymbol.FillInterval = Convert.ToInt32(value); break;
								case "filltype": simplePolygonSymbol.FillType = (FillType)ArcXmlEnumConverter.ToEnum(typeof(FillType), value); break;
								case "overlap": simplePolygonSymbol.Overlap = Convert.ToBoolean(value); break;
								case "transparency": simplePolygonSymbol.Transparency = Convert.ToDouble(value); break;
							}
						}
					}

					reader.MoveToElement();
				}

				return simplePolygonSymbol;
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
    public bool Boundary = true;
    public CapType BoundaryCapType = CapType.Butt;
		public Color BoundaryColor = Color.Black;
		public JoinType BoundaryJoinType = JoinType.Round;
    public LineType BoundaryType = LineType.Solid;
    public int BoundaryWidth = 1;
		public Color FillColor = Color.FromArgb(0, 200, 0);
		public int FillInterval = 6;
    public FillType FillType = FillType.Solid;
    public bool Overlap = true;
    public double Transparency = 1;
    
    public SimplePolygonSymbol() {}
    
    public SimplePolygonSymbol(Color fillColor)
    {
      FillColor = fillColor;
    }
    
    public SimplePolygonSymbol(Color fillColor, Color boundaryColor)
    {
      FillColor = fillColor;
      BoundaryColor = boundaryColor;
    }
    
    public SimplePolygonSymbol(Color fillColor, Color boundaryColor, int boundaryWidth)
    {
      FillColor = fillColor;
      BoundaryColor = boundaryColor;
      BoundaryWidth = boundaryWidth;
    }

    public override object Clone()
    {
			SimplePolygonSymbol clone = (SimplePolygonSymbol)this.MemberwiseClone();
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

				if (!Boundary)
				{
					writer.WriteAttributeString("boundary", "false");
				}
				else
				{
					if (BoundaryCapType != CapType.Butt)
					{
						writer.WriteAttributeString("boundarycaptype", ArcXmlEnumConverter.ToArcXml(typeof(CapType), BoundaryCapType));
					}

					if (Color.FromArgb(0, BoundaryColor) != Color.Black)
					{
						writer.WriteAttributeString("boundarycolor", ColorConverter.ToArcXml(BoundaryColor));
					}

					if (BoundaryColor.A > 0)
					{
						writer.WriteAttributeString("boundarytransparency", (BoundaryColor.A / 255.0).ToString("0.000"));
					}

					if (BoundaryJoinType != JoinType.Round)
					{
						writer.WriteAttributeString("boundaryjointype", ArcXmlEnumConverter.ToArcXml(typeof(JoinType), BoundaryJoinType));
					}

					if (BoundaryType != LineType.Solid)
					{
						writer.WriteAttributeString("boundarytype", ArcXmlEnumConverter.ToArcXml(typeof(LineType), BoundaryType));
					}

					if (BoundaryWidth > 1)
					{
						writer.WriteAttributeString("boundarywidth", BoundaryWidth.ToString());
					}
				}

				if (Color.FromArgb(0, FillColor) != Color.FromArgb(0, 200, 0))
				{
					writer.WriteAttributeString("fillcolor", ColorConverter.ToArcXml(FillColor));
				}

				if (FillColor.A > 0)
				{
					writer.WriteAttributeString("filltransparency", (FillColor.A / 255.0).ToString("0.000"));
				}

				if (FillType != FillType.Solid)
				{
					writer.WriteAttributeString("filltype", ArcXmlEnumConverter.ToArcXml(typeof(FillType), FillType));
				}

				if (!Overlap)
				{
					writer.WriteAttributeString("overlap", "false");
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
