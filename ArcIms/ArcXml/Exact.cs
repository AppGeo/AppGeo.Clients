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
	public class Exact : Classification
	{
		public const string XmlName = "EXACT";

		public static Exact ReadFrom(ArcXmlReader reader)
		{
			try
			{
				Exact exact = new Exact();

				if (reader.HasAttributes)
				{
					while (reader.MoveToNextAttribute())
					{
						string value = reader.ReadContentAsString();

						if (value.Length > 0)
						{
							switch (reader.Name)
							{
								case "label": exact.Label = value; break;
								case "method": exact.Method = (ExactMethod)ArcXmlEnumConverter.ToEnum(typeof(ExactMethod), value); break;
								case "value": exact.Value = value; break;
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
                case CallOutMarkerSymbol.XmlName: exact.Symbol = CallOutMarkerSymbol.ReadFrom(reader); break;
                case ChartSymbol.XmlName: exact.Symbol = ChartSymbol.ReadFrom(reader); break;
                case GradientFillSymbol.XmlName: exact.Symbol = GradientFillSymbol.ReadFrom(reader); break;
                case HashLineSymbol.XmlName: exact.Symbol = HashLineSymbol.ReadFrom(reader); break;
                case RasterFillSymbol.XmlName: exact.Symbol = RasterFillSymbol.ReadFrom(reader); break;
                case RasterMarkerSymbol.XmlName: exact.Symbol = RasterMarkerSymbol.ReadFrom(reader); break;
                case RasterShieldSymbol.XmlName: exact.Symbol = RasterShieldSymbol.ReadFrom(reader); break;
                case SimpleLineSymbol.XmlName: exact.Symbol = SimpleLineSymbol.ReadFrom(reader); break;
								case SimpleMarkerSymbol.XmlName: exact.Symbol = SimpleMarkerSymbol.ReadFrom(reader); break;
								case SimplePolygonSymbol.XmlName: exact.Symbol = SimplePolygonSymbol.ReadFrom(reader); break;
                case ShieldSymbol.XmlName: exact.Symbol = ShieldSymbol.ReadFrom(reader); break;
                case TextSymbol.XmlName: exact.Symbol = TextSymbol.ReadFrom(reader); break;
								case TrueTypeMarkerSymbol.XmlName: exact.Symbol = TrueTypeMarkerSymbol.ReadFrom(reader); break;
							}
						}

						reader.Read();
					}
				}

				return exact;
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

		public ExactMethod Method = ExactMethod.IsExact;
		public string Value = null;

		public Exact() { }

		public Exact(string value, Symbol symbol)
		{
			Value = value;
			Label = value;
			Symbol = (Symbol)symbol.Clone();
		}

		public override object Clone()
		{
			Exact clone = (Exact)this.MemberwiseClone();

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

				if (Method != ExactMethod.IsExact)
				{
					writer.WriteAttributeString("method", ArcXmlEnumConverter.ToArcXml(typeof(ExactMethod), Method));
				}

				if (!String.IsNullOrEmpty(Value))
				{
					writer.WriteAttributeString("value", Value);
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
