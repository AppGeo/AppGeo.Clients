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

namespace AppGeo.Clients.ArcIms.ArcXml
{
  [Serializable]
	public class Other : Classification
	{
		public const string XmlName = "OTHER";

		public static Other ReadFrom(ArcXmlReader reader)
		{
			try
			{
				Other other = new Other();

				if (reader.HasAttributes)
				{
					while (reader.MoveToNextAttribute())
					{
						string value = reader.ReadContentAsString();

						if (value.Length > 0)
						{
							switch (reader.Name)
							{
								case "label": other.Label = value; break;
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
								case HashLineSymbol.XmlName: other.Symbol = HashLineSymbol.ReadFrom(reader); break;
								case SimpleLineSymbol.XmlName: other.Symbol = SimpleLineSymbol.ReadFrom(reader); break;
								case SimpleMarkerSymbol.XmlName: other.Symbol = SimpleMarkerSymbol.ReadFrom(reader); break;
								case SimplePolygonSymbol.XmlName: other.Symbol = SimplePolygonSymbol.ReadFrom(reader); break;
								case ShieldSymbol.XmlName: other.Symbol = ShieldSymbol.ReadFrom(reader); break;
								case TextSymbol.XmlName: other.Symbol = TextSymbol.ReadFrom(reader); break;
								case TrueTypeMarkerSymbol.XmlName: other.Symbol = TrueTypeMarkerSymbol.ReadFrom(reader); break;
							}
						}

						reader.Read();
					}
				}

				return other;
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

		public Other() { }

		public Other(Symbol symbol)
		{
			Symbol = symbol;
		}

		public override object Clone()
		{
			Other clone = (Other)this.MemberwiseClone();

			if (Symbol != null)
			{
				clone.Symbol = (Symbol)Symbol.Clone();
			}

			return clone;
		}

		public override void WriteTo(ArcXmlWriter writer)
		{
			try
			{
				writer.WriteStartElement(XmlName);

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
