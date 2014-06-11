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
using System.Linq;
using System.Xml;

namespace AppGeo.Clients.ArcIms.ArcXml
{
  [Serializable]
	public class Geocode : Response
	{
		public const string XmlName = "GEOCODE";

		public static Geocode ReadFrom(ArcXmlReader reader)
		{
			try
			{
				Geocode geocode = new Geocode();

				if (!reader.IsEmptyElement)
				{
					reader.Read();

					while (!(reader.NodeType == XmlNodeType.EndElement && reader.Name == XmlName))
					{
						if (reader.NodeType == XmlNodeType.Element)
						{
							switch (reader.Name)
							{
								case GcCount.XmlName: geocode.GcCount = GcCount.ReadFrom(reader); break;
                case Feature.XmlName: geocode.Features.Add(Feature.ReadFrom(reader)); break;
              }
						}

						reader.Read();
					}
				}

				return geocode;
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

		public GcCount GcCount = null;
    public List<Feature> Features = new List<Feature>();

		public Geocode() { }

		public override object Clone()
		{
			Geocode clone = (Geocode)this.MemberwiseClone();

			if (GcCount != null)
			{
				clone.GcCount = (GcCount)GcCount.Clone();
			}

			if (Features != null)
			{
				clone.Features = Features.Select(o => (Feature)o.Clone()).ToList();
			}

			return clone;
		}
	}
}
