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
	public class Range : Classification
	{
		public const string XmlName = "RANGE";

		public static Range ReadFrom(ArcXmlReader reader)
		{
			try
			{
				Range range = new Range();

				if (reader.HasAttributes)
				{
					while (reader.MoveToNextAttribute())
					{
						string value = reader.ReadContentAsString();

						if (value.Length > 0)
						{
							switch (reader.Name)
							{
								case "equality": range.Equality = (RangeEquality)ArcXmlEnumConverter.ToEnum(typeof(RangeEquality), value); break;
								case "label": range.Label = value; break;
								case "lower": range.Lower = value; break;
								case "upper": range.Upper = value; break;
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
								case HashLineSymbol.XmlName: range.Symbol = HashLineSymbol.ReadFrom(reader); break;
								case SimpleLineSymbol.XmlName: range.Symbol = SimpleLineSymbol.ReadFrom(reader); break;
								case SimpleMarkerSymbol.XmlName: range.Symbol = SimpleMarkerSymbol.ReadFrom(reader); break;
								case SimplePolygonSymbol.XmlName: range.Symbol = SimplePolygonSymbol.ReadFrom(reader); break;
								case ShieldSymbol.XmlName: range.Symbol = ShieldSymbol.ReadFrom(reader); break;
								case TextSymbol.XmlName: range.Symbol = TextSymbol.ReadFrom(reader); break;
								case TrueTypeMarkerSymbol.XmlName: range.Symbol = TrueTypeMarkerSymbol.ReadFrom(reader); break;
							}
						}

						reader.Read();
					}
				}

				return range;
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

		public RangeEquality Equality = RangeEquality.Lower;
    public string Lower = null;
    public string Upper = null;

		public Range() { }

		public Range(string lower, string upper, Symbol symbol)
		{
			Lower = lower;
			Upper = upper;
			Symbol = (Symbol)symbol.Clone();
		}

		public override object Clone()
		{
			Range clone = (Range)this.MemberwiseClone();

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

				if (Equality != RangeEquality.Lower)
				{
					writer.WriteAttributeString("equality", ArcXmlEnumConverter.ToArcXml(typeof(RangeEquality), Equality));
				}

				if (!String.IsNullOrEmpty(Label))
				{
					writer.WriteAttributeString("label", Label);
				}

				if (!String.IsNullOrEmpty(Lower))
				{
					writer.WriteAttributeString("lower", Lower);
				}

				if (!String.IsNullOrEmpty(Upper))
				{
					writer.WriteAttributeString("upper", Upper);
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
