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
  public class GetImage : Request
	{
		public const string XmlName = "GET_IMAGE";

		public bool AutoResize = false;
		public string DataFrame = null;
    public ShowType Show = ShowType.None;
    public bool UseServiceDatum = false;

		public Environment Environment = null;
		public FeatureCoordSys FeatureCoordSys = null;
		public FilterCoordSys FilterCoordSys = null;
		public Properties Properties = null;
		public Layers Layers = null;

		public GetImage()
		{
			Properties = new Properties();
			Properties.ImageSize = new ImageSize();
			Properties.LayerList = new LayerList();
		}

		public override object Clone()
		{
			GetImage clone = (GetImage)this.MemberwiseClone();

			if (Environment != null)
			{
				clone.Environment = (Environment)Environment.Clone();
			}

			if (FeatureCoordSys != null)
			{
				clone.FeatureCoordSys = (FeatureCoordSys)FeatureCoordSys.Clone();
			}

			if (FilterCoordSys != null)
			{
				clone.FilterCoordSys = (FilterCoordSys)FilterCoordSys.Clone();
			}

			if (Properties != null)
			{
				clone.Properties = (Properties)Properties.Clone();
			}

			if (Layers != null)
			{
				clone.Layers = (Layers)Layers.Clone();
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

				if (!String.IsNullOrEmpty(DataFrame))
				{
					writer.WriteAttributeString("dataframe", DataFrame);
				}

				if (Show != ShowType.None)
				{
					writer.WriteAttributeString("show", ArcXmlEnumConverter.ToArcXml(typeof(ShowType), Show));
				}

        if (UseServiceDatum)
        {
          writer.WriteAttributeString("useservicedatum", "true");
        }

				if (Environment != null)
				{
					Environment.WriteTo(writer);
				}

				if (FeatureCoordSys != null)
				{
					FeatureCoordSys.WriteTo(writer);
				}

				if (FilterCoordSys != null)
				{
					FilterCoordSys.WriteTo(writer);
				}

				if (Properties != null)
				{
					Properties.WriteTo(writer);
				}

				if (Layers != null)
				{
					for (int i = 0; i < Layers.Count; ++i)
					{
						Layers[i].WriteTo(writer);
					}
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
