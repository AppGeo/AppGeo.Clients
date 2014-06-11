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
using GeoAPI.Geometries;

namespace AppGeo.Clients.ArcIms.ArcXml
{
  [Serializable]
	public class GetLayout : Request
	{
		public const string XmlName = "GET_LAYOUT";

		public bool AutoResize = false;
		public Properties Properties = new Properties();

		private List<DataFrame> _dataFrames = new List<DataFrame>();

		public GetLayout()
		{
			Properties.ImageSize = new ImageSize();
		}

    public List<DataFrame> DataFrames
    {
      get
      {
        return _dataFrames;
      }
    }

		public Envelope Envelope
		{
			get
			{
				return Properties.Envelope;
			}
			set
			{
				Properties.Envelope = value;
			}
		}

		public ImageSize ImageSize
		{
			get
			{
				return Properties.ImageSize;
			}
			set
			{
				Properties.ImageSize = value;
			}
		}

		public override object Clone()
		{
			GetLayout clone = (GetLayout)this.MemberwiseClone();

			clone._dataFrames = new List<DataFrame>();

			foreach (DataFrame dataFrame in _dataFrames)
			{
        clone._dataFrames.Add((DataFrame)dataFrame.Clone());
			}

			if (Properties != null)
			{
				clone.Properties = (Properties)Properties.Clone();
			}

			return clone;
		}

		public override void WriteTo(ArcXmlWriter writer)
		{
			try
			{
				writer.WriteStartElement(XmlName);

				if (AutoResize)
				{
					writer.WriteAttributeString("autoresize", "true");
				}

        foreach (DataFrame dataFrame in _dataFrames)
        {
          dataFrame.WriteTo(writer);
				}

				if (Properties != null)
				{
					Properties.WriteTo(writer);
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
