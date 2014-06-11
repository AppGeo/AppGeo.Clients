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
	public class FeatureCount : ICloneable
	{
		public const string XmlName = "FEATURECOUNT";

		public static FeatureCount ReadFrom(ArcXmlReader reader)
		{
			try
			{
				FeatureCount featureCount = new FeatureCount();

				if (reader.HasAttributes)
				{
					while (reader.MoveToNextAttribute())
					{
						string value = reader.ReadContentAsString();

						if (value.Length > 0)
						{
							switch (reader.Name)
							{
								case "count": featureCount.Count = Convert.ToInt32(value); break;
								case "hasmore": featureCount.HasMore = Convert.ToBoolean(value); break;
							}
						}
					}

					reader.MoveToElement();
				}

				return featureCount;
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

		public int Count = 0;
		public bool HasMore = false;

		public FeatureCount() { }

		public object Clone()
		{
			FeatureCount clone = (FeatureCount)this.MemberwiseClone();
			return clone;
		}
	}
}
