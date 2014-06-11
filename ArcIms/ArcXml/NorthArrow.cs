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
	public class NorthArrow : ICloneable
	{
		public const string XmlName = "NORTHARROW";

		public double X = 0;
		public double Y = 0;
		public int Type = 1;

		public double Angle = 0;
		public bool Antialiasing = false;
		public Color Outline = Color.Empty;
		public bool Overlap = true;
		public Color Shadow = Color.Empty;
		public int Size = 30;
		public double Transparency = 1;

		public NorthArrow() { }

		public NorthArrow(int type, int size, double x, double y)
		{
			Type = type;
			Size = size;
			X = x;
			Y = y;
		}

		public object Clone()
		{
			NorthArrow clone = (NorthArrow)this.MemberwiseClone();
			return clone;
		}

		public void WriteTo(ArcXmlWriter writer)
		{
			try
			{
				writer.WriteStartElement(XmlName);

				writer.WriteAttributeString("coords", X.ToString() + writer.CoordinateSeparator[0] + Y.ToString());
				writer.WriteAttributeString("type", Type.ToString());

				if (Angle != 0)
				{
					writer.WriteAttributeString("angle", Angle.ToString());
				}

				if (Antialiasing)
				{
					writer.WriteAttributeString("antialiasing", "true");
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

				if (Size != 30)
				{
					writer.WriteAttributeString("size", Size.ToString());
				}

				if (0 <= Transparency && Transparency < 1)
				{
					writer.WriteAttributeString("transparency", Transparency.ToString());
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

