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
	public class ScaleBar : ICloneable
	{
		public const string XmlName = "SCALEBAR";

		public double X = 0;
		public double Y = 0;

		public bool Antialiasing = false;
		public Color BarColor = Color.FromArgb(255, 162, 115);
		public int BarWidth = 5;
		public double Distance = 0;
		public string Font = "Arial";
		public Color FontColor = Color.Black;
		public int FontSize = 10;
		public FontStyle FontStyle = FontStyle.Regular;
		public Units MapUnits = Units.Degrees;
		public ScaleBarMode Mode = ScaleBarMode.Cartesian;
		public Color Outline = Color.Empty;
		public bool Overlap = true;
		public int Precision = 0;
		public int Round = 0;
		public ScaleUnits ScaleUnits = ScaleUnits.Miles;
		public int ScreenLength = 0;

		public ScaleBar() { }

		public object Clone()
		{
			ScaleBar clone = (ScaleBar)this.MemberwiseClone();
			return clone;
		}

		public void WriteTo(ArcXmlWriter writer)
		{
			try
			{
				writer.WriteStartElement(XmlName);

				writer.WriteAttributeString("coords", X.ToString() + writer.CoordinateSeparator[0] + Y.ToString());

				if (Antialiasing)
				{
					writer.WriteAttributeString("antialiasing", "true");
				}

				if (BarColor != Color.FromArgb(255, 162, 115))
				{
					writer.WriteAttributeString("barcolor", ColorConverter.ToArcXml(BarColor));
				}

				if (BarColor.A > 0)
				{
					writer.WriteAttributeString("bartransparency", (BarColor.A / 255.0).ToString("0.000"));
				}

				if (Distance > 0)
				{
					writer.WriteAttributeString("distance", Distance.ToString());
				}

				if (!String.IsNullOrEmpty(Font) && Font != "Arial")
				{
					writer.WriteAttributeString("font", Font);
				}

				if (FontColor != Color.Black)
				{
					writer.WriteAttributeString("fontcolor", ColorConverter.ToArcXml(FontColor));
				}

				if (FontColor.A > 255)
				{
					writer.WriteAttributeString("texttransparency", (FontColor.A / 255.0).ToString("0.000"));
				}

				if (FontSize != 10)
				{
					writer.WriteAttributeString("fontsize", FontSize.ToString());
				}

				if (FontStyle != FontStyle.Regular)
				{
					writer.WriteAttributeString("fontstyle", ArcXmlEnumConverter.ToArcXml(typeof(FontStyle), FontStyle));
				}

				if (MapUnits != Units.Degrees)
				{
					writer.WriteAttributeString("mapunits", ArcXmlEnumConverter.ToArcXml(typeof(Units), MapUnits));
				}

				if (Mode != ScaleBarMode.Cartesian)
				{
					writer.WriteAttributeString("mode", ArcXmlEnumConverter.ToArcXml(typeof(ScaleBarMode), Mode));
				}

				if (!Outline.IsEmpty)
				{
					writer.WriteAttributeString("outline", ColorConverter.ToArcXml(Outline));
				}

				if (!Overlap)
				{
					writer.WriteAttributeString("overlap", "false");
				}

				if (Precision > 0)
				{
					writer.WriteAttributeString("precision", Precision.ToString());
				}

				if (Round > 0)
				{
					writer.WriteAttributeString("round", Round.ToString());
				}

				if (ScaleUnits != ScaleUnits.Miles)
				{
					writer.WriteAttributeString("scaleunits", ArcXmlEnumConverter.ToArcXml(typeof(ScaleUnits), ScaleUnits));
				}

				if (ScreenLength > 0)
				{
					writer.WriteAttributeString("screenlength", ScreenLength.ToString());
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
