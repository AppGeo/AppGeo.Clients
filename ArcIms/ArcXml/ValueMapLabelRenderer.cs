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
using System.Collections.Generic;
using System.Xml;

namespace AppGeo.Clients.ArcIms.ArcXml
{
  [Serializable]
	public class ValueMapLabelRenderer : Renderer
	{
		public const string XmlName = "VALUEMAPLABELRENDERER";

		public static ValueMapLabelRenderer ReadFrom(ArcXmlReader reader)
		{
			try
			{
				ValueMapLabelRenderer valueMapLabelRenderer = new ValueMapLabelRenderer();

				if (reader.HasAttributes)
				{
					while (reader.MoveToNextAttribute())
					{
						string value = reader.ReadContentAsString();

						if (value.Length > 0)
						{
							switch (reader.Name)
							{
								case "lookupfield": valueMapLabelRenderer.LookupField = value; break;
								case "labelfield": valueMapLabelRenderer.LabelField = value; break;
								case "featureweight": valueMapLabelRenderer.FeatureWeight = (LabelWeight)ArcXmlEnumConverter.ToEnum(typeof(LabelWeight), value); break;
								case "howmanylabels": valueMapLabelRenderer.HowManyLabels = (HowManyLabels)ArcXmlEnumConverter.ToEnum(typeof(HowManyLabels), value); break;
								case "labelbufferratio": valueMapLabelRenderer.LabelBufferRatio = Convert.ToDouble(value); break;
								case "labelpriorities": valueMapLabelRenderer.LabelPriorities = value; break;
								case "labelweight": valueMapLabelRenderer.LabelWeight = (LabelWeight)ArcXmlEnumConverter.ToEnum(typeof(LabelWeight), value); break;
								case "linelabelposition": valueMapLabelRenderer.LineLabelPosition = (LineLabelPosition)ArcXmlEnumConverter.ToEnum(typeof(LineLabelPosition), value); break;
								case "rotationalangles": valueMapLabelRenderer.RotationalAngles = value; break;
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
								case Exact.XmlName: valueMapLabelRenderer.Classifications.Add(Exact.ReadFrom(reader)); break;
								case Other.XmlName: valueMapLabelRenderer.Classifications.Add(Other.ReadFrom(reader)); break;
								case Range.XmlName: valueMapLabelRenderer.Classifications.Add(Range.ReadFrom(reader)); break;
							}
						}

						reader.Read();
					}
				}

				return valueMapLabelRenderer;
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

		public string LabelField = null;
    public string LookupField = null;
		public LabelWeight FeatureWeight = LabelWeight.NoWeight;
		public HowManyLabels HowManyLabels = HowManyLabels.OneLabelPerShape;
		public double LabelBufferRatio = 0;
		public string LabelPriorities = "2,2,1,4,5,3,2,4";
		public LabelWeight LabelWeight = LabelWeight.HighWeight;
		public LineLabelPosition LineLabelPosition = LineLabelPosition.PlaceAbove;
    public string RotationalAngles = null;

		private List<Classification> _classifications = new List<Classification>();

		public ValueMapLabelRenderer() { }

		public List<Classification> Classifications
		{
			get
			{
				return _classifications;
			}
		}

		public override object Clone()
		{
			ValueMapLabelRenderer clone = (ValueMapLabelRenderer)this.MemberwiseClone();

			clone._classifications = new List<Classification>();

			foreach (Classification classification in _classifications)
			{
				clone.Classifications.Add((Classification)classification.Clone());
			}

			return clone;
		}

		public override void WriteTo(ArcXmlWriter writer)
		{
			try
			{
				writer.WriteStartElement(XmlName);

				if (!String.IsNullOrEmpty(LookupField))
				{
					writer.WriteAttributeString("lookupfield", LookupField);
				}

				if (!String.IsNullOrEmpty(LabelField))
				{
					writer.WriteAttributeString("labelfield", LabelField);
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

				foreach (Classification classification in _classifications)
				{
					classification.WriteTo(writer);
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
