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

namespace AppGeo.Clients.ArcIms.ArcXml
{
  [Serializable]
  public class Address : ICloneable
	{
		public const string XmlName = "ADDRESS";

		private List<GcTag> _gcTags = new List<GcTag>();

		public Address() { }

		public List<GcTag> GcTags
		{
			get
			{
				return _gcTags;
			}
		}

		public object Clone()
		{
			Address clone = (Address)this.MemberwiseClone();

			foreach (GcTag gcTag in _gcTags)
			{
				clone._gcTags.Add((GcTag)gcTag.Clone());
			}

			return clone;
		}

		public void WriteTo(ArcXmlWriter writer)
		{
			try
			{
				writer.WriteStartElement(XmlName);

				foreach (GcTag gcTag in _gcTags)
				{
					gcTag.WriteTo(writer);
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
