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
  public class ServiceInfo : Response
	{
		public const string XmlName = "SERVICEINFO";

		public static ServiceInfo ReadFrom(ArcXmlReader reader)
		{
			try
			{
				ServiceInfo serviceInfo = new ServiceInfo();

				if (!reader.IsEmptyElement)
				{
					reader.Read();

					while (!(reader.NodeType == XmlNodeType.EndElement && reader.Name == XmlName))
					{
						if (reader.NodeType == XmlNodeType.Element)
						{
							switch (reader.Name)
							{
								case Environment.XmlName: serviceInfo.Environment = Environment.ReadFrom(reader); break;
								case Properties.XmlName: serviceInfo.Properties = Properties.ReadFrom(reader); break;
								case LayoutInfo.XmlName: serviceInfo.LayoutInfo = LayoutInfo.ReadFrom(reader); break;
                
                case DataFrameInfo.XmlName:
                  if (serviceInfo.DataFrameInfos == null)
                  {
                    serviceInfo.DataFrameInfos = new DataFrameInfoList();
                  }

                  serviceInfo.DataFrameInfos.Add(DataFrameInfo.ReadFrom(reader)); 
                  break;

                case LayerInfo.XmlName:
                  if (serviceInfo.LayerInfos == null)
                  {
                    serviceInfo.LayerInfos = new LayerInfoList();
                  }

                  serviceInfo.LayerInfos.Add(LayerInfo.ReadFrom(reader));
                  break;
              }
						}

						reader.Read();
					}
				}

				return serviceInfo;
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

    public DataFrameInfoList DataFrameInfos = null;
    public Environment Environment = null;
    public LayerInfoList LayerInfos = null;
    public LayoutInfo LayoutInfo = null;
    public Properties Properties = null;

    public ServiceInfo() {}

    public bool IsGeocodeable
    {
      get
      {
				foreach (LayerInfo layer in LayerInfos)
				{
					if (layer.IsGeocodable)
					{
						return true;
					}
				}

        return false;
      }
    }

		public bool IsArcMap
		{
			get
			{
				return LayoutInfo != null;
			}
		}

    public override object Clone()
    {
      ServiceInfo clone = (ServiceInfo)this.MemberwiseClone();

      if (DataFrameInfos != null)
      {
        clone.DataFrameInfos = (DataFrameInfoList)DataFrameInfos.Clone();
      }

      if (Environment != null)
      {
        clone.Environment = (Environment)Environment.Clone();
      }

      if (LayerInfos != null)
      {
        clone.LayerInfos = (LayerInfoList)LayerInfos.Clone();
      }

			if (LayoutInfo != null)
			{
				clone.LayoutInfo = (LayoutInfo)LayoutInfo.Clone();
			}

      if (Properties != null)
      {
        clone.Properties = (Properties)Properties.Clone();
      }

      return clone;
    }
	}
}
