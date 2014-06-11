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
	public class SimpleLabelRenderer : Renderer
	{
		public const string XmlName = "SIMPLELABELRENDERER";

		public static SimpleLabelRenderer ReadFrom(ArcXmlReader reader)
		{
			try
			{
				SimpleLabelRenderer simpleLabelRenderer = new SimpleLabelRenderer();

				if (reader.HasAttributes)
				{
					while (reader.MoveToNextAttribute())
					{
						string value = reader.ReadContentAsString();

						if (value.Length > 0)
						{
							switch (reader.Name)
							{
								case "field": simpleLabelRenderer.Field = value; break;
								case "featureweight": simpleLabelRenderer.FeatureWeight = (LabelWeight)ArcXmlEnumConverter.ToEnum(typeof(LabelWeight), value); break;
								case "howmanylabels": simpleLabelRenderer.HowManyLabels = (HowManyLabels)ArcXmlEnumConverter.ToEnum(typeof(HowManyLabels), value); break;
								case "labelbufferratio": simpleLabelRenderer.LabelBufferRatio = Convert.ToDouble(value); break;
								case "labelpriorities": simpleLabelRenderer.LabelPriorities = value; break;
								case "labelweight": simpleLabelRenderer.LabelWeight = (LabelWeight)ArcXmlEnumConverter.ToEnum(typeof(LabelWeight), value); break;
								case "linelabelposition": simpleLabelRenderer.LineLabelPosition = (LineLabelPosition)ArcXmlEnumConverter.ToEnum(typeof(LineLabelPosition), value); break;
								case "rotationalangles": simpleLabelRenderer.RotationalAngles = value; break;
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
								case ShieldSymbol.XmlName: simpleLabelRenderer.Symbol = ShieldSymbol.ReadFrom(reader); break;
								case TextSymbol.XmlName: simpleLabelRenderer.Symbol = TextSymbol.ReadFrom(reader); break;
							}
						}

						reader.Read();
					}
				}

				return simpleLabelRenderer;
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

		public string Field = null;
		public LabelWeight FeatureWeight = LabelWeight.NoWeight;
		public HowManyLabels HowManyLabels = HowManyLabels.OneLabelPerShape;
		public double LabelBufferRatio = 0;
		public string LabelPriorities = "2,2,1,4,5,3,2,4";
		public LabelWeight LabelWeight = LabelWeight.HighWeight;
		public LineLabelPosition LineLabelPosition = LineLabelPosition.PlaceAbove;
		public string RotationalAngles = "";

		public Symbol Symbol = null;

		public SimpleLabelRenderer() { }

		public override object Clone()
		{
			SimpleLabelRenderer clone = (SimpleLabelRenderer)this.MemberwiseClone();

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

				if (!String.IsNullOrEmpty(Field))
				{
					writer.WriteAttributeString("field", Field);
				}

				if (FeatureWeight != LabelWeight.NoWeight)
				{
					writer.WriteAttributeString("featureweight", ArcXmlEnumConverter.ToArcXml(typeof(LabelWeight), FeatureWeight));
				}

				if (HowManyLabels != HowManyLabels.OneLabelPerShape)
				{
					writer.WriteAttributeString("howmanylabels", ArcXmlEnumConverter.ToArcXml(typeof(HowManyLabels), HowManyLabels));
				}

				if (LabelBufferRatio > 0)
				{
					writer.WriteAttributeString("labelbufferratio", LabelBufferRatio.ToString("0.000"));
				}

				if (!String.IsNullOrEmpty(LabelPriorities))
				{
					writer.WriteAttributeString("labelpriorities", LabelPriorities);
				}

				if (LabelWeight != LabelWeight.HighWeight)
				{
					writer.WriteAttributeString("labelweight", ArcXmlEnumConverter.ToArcXml(typeof(LabelWeight), LabelWeight));
				}

				if (LineLabelPosition != LineLabelPosition.PlaceAbove)
				{
					writer.WriteAttributeString("linelabelposition", ArcXmlEnumConverter.ToArcXml(typeof(LineLabelPosition), LineLabelPosition));
				}

				if (!String.IsNullOrEmpty(RotationalAngles))
				{
					writer.WriteAttributeString("rotationalangles", RotationalAngles);
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