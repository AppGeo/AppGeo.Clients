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
	public class DataFrameInfo : ICloneable
	{
		public const string XmlName = "DATAFRAMEINFO";

		public static DataFrameInfo ReadFrom(ArcXmlReader reader)
		{
			try
			{
				DataFrameInfo dataFrameInfo = new DataFrameInfo();

				if (reader.HasAttributes)
				{
					while (reader.MoveToNextAttribute())
					{
						string value = reader.ReadContentAsString();

						if (value.Length > 0)
						{
							switch (reader.Name)
							{
								case "default": dataFrameInfo.Default = Convert.ToBoolean(value); break;
                case "name": dataFrameInfo.Name = value; break;
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
								case Properties.XmlName: dataFrameInfo.Properties = Properties.ReadFrom(reader); break;
								case LayerInfo.XmlName: dataFrameInfo.LayerInfos.Add(LayerInfo.ReadFrom(reader)); break;
							}
						}

						reader.Read();
					}
				}

				return dataFrameInfo;
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

		private LayerInfoList _layerInfos = new LayerInfoList();

    public bool Default = false;
		public string Name = null;
		public Properties Properties = null;

		public DataFrameInfo() { }

		public LayerInfoList LayerInfos
		{
			get
			{
				return _layerInfos;
			}
		}

		public object Clone()
		{
			DataFrameInfo clone = (DataFrameInfo)this.MemberwiseClone();

			clone._layerInfos = (LayerInfoList)_layerInfos.Clone();

			if (Properties != null)
			{
				clone.Properties = (Properties)Properties.Clone();
			}

			return clone;
		}
	}
}
