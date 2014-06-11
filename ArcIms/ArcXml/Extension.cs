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
	public class Extension : ICloneable
	{
		public const string XmlName = "EXTENSION";

		public static Extension ReadFrom(ArcXmlReader reader)
		{
			try
			{
				Extension extension = new Extension();

				if (reader.HasAttributes)
				{
					while (reader.MoveToNextAttribute())
					{
						string value = reader.ReadContentAsString();

						if (value.Length > 0)
						{
							switch (reader.Name)
							{
								case "type": extension.Type = (ExtensionType)ArcXmlEnumConverter.ToEnum(typeof(ExtensionType), value); break;
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
								case GcStyle.XmlName: extension.GcStyle = GcStyle.ReadFrom(reader); break;
							}
						}

						reader.Read();
					}
				}

				return extension;
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

		public ExtensionType Type = ExtensionType.Geocode;
		public GcStyle GcStyle = null;

		public Extension() { }

		public object Clone()
		{
			Extension clone = (Extension)this.MemberwiseClone();

			if (GcStyle != null)
			{
				clone.GcStyle = (GcStyle)GcStyle.Clone();
			}

			return clone;
		}
	}
}
