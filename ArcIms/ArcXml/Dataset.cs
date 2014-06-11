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
	public class Dataset : ICloneable
	{
		public const string XmlName = "DATASET";

		public string FromLayer = null;
    public string Name = null;
		public DatasetType Type = DatasetType.None;
    public string Workspace = null;

		public Dataset() { }

		public Dataset(string fromLayer)
		{
			FromLayer = fromLayer;
		}

		public object Clone()
		{
			Dataset clone = (Dataset)this.MemberwiseClone();
			return clone;
		}

		public void WriteTo(ArcXmlWriter writer)
		{
			try
			{
				writer.WriteStartElement(XmlName);

				if (!String.IsNullOrEmpty(FromLayer))
				{
					writer.WriteAttributeString("fromlayer", FromLayer);
				}

        if (!String.IsNullOrEmpty(Name))
				{
					writer.WriteAttributeString("name", Name);
				}

				if (Type != DatasetType.None)
				{
					writer.WriteAttributeString("method", ArcXmlEnumConverter.ToArcXml(typeof(DatasetType), Type));
				}

				if (!String.IsNullOrEmpty(Workspace))
				{
					writer.WriteAttributeString("workspace", Workspace);
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
