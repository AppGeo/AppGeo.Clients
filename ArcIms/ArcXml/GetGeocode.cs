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
	public class GetGeocode : Request
	{
		public const string XmlName = "GET_GEOCODE";

		public int MaxCandidates = 20;
		public int MinScore = 60;
    public int SpellingSensitivity = 80;

		public Address Address = new Address();
		public Layer Layer = new Layer(null, LayerType.None);
		public FeatureCoordSys FeatureCoordSys = null;

		public GetGeocode() { }

		public GetGeocode(LayerInfo layer) : this(layer.ID) { }

		public GetGeocode(string layerID)
		{
			Layer.ID = layerID;
		}

		public override object Clone()
		{
			GetGeocode clone = (GetGeocode)this.MemberwiseClone();

			if (Address != null)
			{
				clone.Address = (Address)Address.Clone();
			}

			if (Layer != null)
			{
				clone.Layer = (Layer)Layer.Clone();
			}

			if (FeatureCoordSys != null)
			{
				clone.FeatureCoordSys = (FeatureCoordSys)FeatureCoordSys.Clone();
			}

			return clone;
		}

		public override void WriteTo(ArcXmlWriter writer)
		{
			try
			{
				writer.WriteStartElement(XmlName);

				writer.WriteAttributeString("maxcandidates", MaxCandidates.ToString());
				writer.WriteAttributeString("minscore", MinScore.ToString());
        writer.WriteAttributeString("spellingsensitivity", SpellingSensitivity.ToString());

				if (Address != null)
				{
					Address.WriteTo(writer);
				}

				if (Layer != null)
				{
					Layer.WriteTo(writer);
				}

				if (FeatureCoordSys != null)
				{
					FeatureCoordSys.WriteTo(writer);
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
