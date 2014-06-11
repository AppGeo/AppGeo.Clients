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

namespace AppGeo.Clients.ArcIms.ArcXml
{
  [Serializable]
	public class GetFeatures : Request
	{
		public const string XmlName = "GET_FEATURES";

		public bool Attributes = true;
		public int BeginRecord = 0;
		public bool CheckEsc = true;
		public bool Compact = false;
		public string DataFrame = null;
		public bool Envelope = false;
		public int FeatureLimit = 0;
		public bool Geometry = true;
		public bool GlobalEnvelope = false;
		public OutputMode OutputMode = OutputMode.NewXml;
		public bool SkipFeatures = false;

		public Layer Layer = null;
		public Query Query = null;
		public Environment Environment = null;

		public GetFeatures() { }

		public GetFeatures(LayerInfo layerInfo, Query query)
		{
			Layer = new Layer(layerInfo.ID);
			Layer.Type = LayerType.None;
			Query = query;
		}

		public GetFeatures(string layerID, Query query)
		{
			Layer = new Layer(layerID);
			Layer.Type = LayerType.None;
			Query = query;
		}

		public override object Clone()
		{
			GetFeatures clone = (GetFeatures)this.MemberwiseClone();

			if (Environment != null)
			{
				clone.Environment = (Environment)Environment.Clone();
			}

			if (Layer != null)
			{
				clone.Layer = (Layer)Layer.Clone();
			}

			if (Query != null)
			{
				clone.Query = (Query)Query.Clone();
			}

			return clone;
		}

		public override void WriteTo(ArcXmlWriter writer)
		{
			try
			{
				writer.WriteStartElement(XmlName);

				if (!Attributes)
				{
					writer.WriteAttributeString("attributes", "false");
				}

				if (BeginRecord > 0)
				{
					writer.WriteAttributeString("beginrecord", BeginRecord.ToString());
				}

				if (CheckEsc)
				{
					writer.WriteAttributeString("checkesc", "true");
				}

				if (Compact)
				{
					writer.WriteAttributeString("compact", "true");
				}

				if (!String.IsNullOrEmpty(DataFrame))
				{
					writer.WriteAttributeString("dataframe", DataFrame);
				}

				if (Envelope)
				{
					writer.WriteAttributeString("envelope", "true");
				}

				if (!Geometry)
				{
					writer.WriteAttributeString("geometry", "false");
				}

				if (GlobalEnvelope)
				{
					writer.WriteAttributeString("globalenvelope", "true");
				}

				if (OutputMode != OutputMode.Binary)
				{
					writer.WriteAttributeString("outputmode", ArcXmlEnumConverter.ToArcXml(typeof(OutputMode), OutputMode));
				}

				if (SkipFeatures)
				{
					writer.WriteAttributeString("skipfeatures", "true");
				}

				if (Environment != null)
				{
					Environment.WriteTo(writer);
				}

				if (Layer != null)
				{
					Layer.WriteTo(writer);
				}

				if (Query != null)
				{
					Query.WriteTo(writer);
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
