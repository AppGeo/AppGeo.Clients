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
using System.Xml;
using GeoAPI.Geometries;
using NetTopologySuite.Geometries;

namespace AppGeo.Clients.ArcIms.ArcXml
{
  [Serializable]
  public class Text : ICloneable 
	{
		public const string XmlName = "TEXT";

    public static Text ReadFrom(ArcXmlReader reader)
    {
      try
      {
        Text text = new Text();

        if (reader.HasAttributes)
        {
          while (reader.MoveToNextAttribute())
          {
            string value = reader.ReadContentAsString();

            if (value.Length > 0)
            {
              switch (reader.Name)
              {
                case "label": text.Label = value; break;

                case "coords":
                  string[] p = value.Split(new char[] { reader.CoordinateSeparator[0] });
                  text.X = Convert.ToDouble(p[0]);
                  text.Y = Convert.ToDouble(p[1]);
                  break;
              }
            }
          }

          reader.MoveToElement();
        }

        if (!reader.IsEmptyElement)
        {
          reader.Read();

          while (!(reader.NodeType == XmlNodeType.EndElement && reader.Name == XmlName))
          {
            if (reader.NodeType == XmlNodeType.Element)
            {
              switch (reader.Name)
              {
                case TextMarkerSymbol.XmlName: text.Symbol = TextMarkerSymbol.ReadFrom(reader); break;
              }
            }

            reader.Read();
          }
        }

        return text;
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

		public double X = 0;
		public double Y = 0;
		public string Label = "";

		public TextMarkerSymbol Symbol = new TextMarkerSymbol();

		public Text() { }

    public Text(string label, IPoint p) : this(label, p.Coordinate.X, p.Coordinate.Y) { }

		public Text(string label, double x, double y)
		{
			X = x;
			Y = y;
			Label = label;
		}

    public Text(string label, IPoint p, TextMarkerSymbol symbol) : this(label, p.Coordinate.X, p.Coordinate.Y, symbol) { }

		public Text(string label, double x, double y, TextMarkerSymbol symbol)
		{
			X = x;
			Y = y;
			Label = label;
			Symbol = symbol;
		}

		public object Clone()
		{
			Text clone = (Text)this.MemberwiseClone();

			if (Symbol != null)
			{
				clone.Symbol = (TextMarkerSymbol)Symbol.Clone();
			}

			return clone;
		}

    public IPoint ToPoint()
    {
      return new Point(X, Y);
    }

		public void WriteTo(ArcXmlWriter writer)
		{
			try
			{
				writer.WriteStartElement(XmlName);

				writer.WriteAttributeString("coords", X.ToString() + writer.CoordinateSeparator[0] + Y.ToString());

				if (!String.IsNullOrEmpty(Label))
				{
					writer.WriteAttributeString("label", Label);
				}

				if (Symbol != null)
				{
					Symbol.WriteTo(writer);
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
