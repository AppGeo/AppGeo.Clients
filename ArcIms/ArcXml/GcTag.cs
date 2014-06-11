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
  public class GcTag : ICloneable
	{
		public const string XmlName = "GCTAG";

		public string ID = null;
    public string Value = null;

		public GcTag() { }

		public GcTag(string id, string value)
		{
			ID = id;
			Value = value;
		}

		public object Clone()
		{
			GcTag clone = (GcTag)this.MemberwiseClone();
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

				if (!String.IsNullOrEmpty(Value))
				{
					writer.WriteAttributeString("value", Value);
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
