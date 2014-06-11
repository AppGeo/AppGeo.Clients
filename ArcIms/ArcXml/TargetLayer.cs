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
	public class TargetLayer : ICloneable
	{
		public const string XmlName = "TARGETLAYER";

		public string ID = null;
		public LayerType Type = LayerType.None;
		public Dataset Dataset = null;

		public TargetLayer() { }

		public object Clone()
		{
			TargetLayer clone = (TargetLayer)this.MemberwiseClone();

			if (Dataset != null)
			{
				clone.Dataset = (Dataset)Dataset.Clone();
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

				if (Type != LayerType.None)
				{
					writer.WriteAttributeString("type", ArcXmlEnumConverter.ToArcXml(typeof(LayerType), Type));
				}

				if (Dataset != null)
				{
					Dataset.WriteTo(writer);
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
