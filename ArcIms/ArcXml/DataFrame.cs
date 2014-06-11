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
using GeoAPI.Geometries;

namespace AppGeo.Clients.ArcIms.ArcXml
{
  [Serializable]
	public class DataFrame : ICloneable
	{
		public const string XmlName = "DATAFRAME";

		public string ID = null;

    public Envelope Envelope = new Envelope();
    public FeatureCoordSys FeatureCoordSys = null;
		public FilterCoordSys FilterCoordSys = null;
		public LayerList LayerList = null;
		public Scale Scale = null;

		public DataFrame() { }

		public object Clone()
		{
			DataFrame clone = (DataFrame)this.MemberwiseClone();

			if (FeatureCoordSys != null)
			{
				clone.FeatureCoordSys = (FeatureCoordSys)FeatureCoordSys.Clone();
			}

			if (FilterCoordSys != null)
			{
				clone.FilterCoordSys = (FilterCoordSys)FilterCoordSys.Clone();
			}

			if (LayerList != null)
			{
				clone.LayerList = (LayerList)LayerList.Clone();
			}

			if (Scale != null)
			{
				clone.Scale = (Scale)Scale.Clone();
			}

			return clone;
		}

		public void WriteTo(ArcXmlWriter writer)
		{
			try
			{
				writer.WriteStartElement(XmlName);

				if (!String.IsNullOrEmpty(ID))
				{
					writer.WriteAttributeString("id", ID);
				}

        if (!Envelope.IsNull)
        {
          EnvelopeSerializer.WriteTo(writer, Envelope);
        }

				if (FeatureCoordSys != null)
				{
					FeatureCoordSys.WriteTo(writer);
				}

				if (FilterCoordSys != null)
				{
					FilterCoordSys.WriteTo(writer);
				}

				if (LayerList != null)
				{
					LayerList.WriteTo(writer);
				}

				if (Scale != null)
				{
					Scale.WriteTo(writer);
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