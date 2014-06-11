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
	public class Environment : ICloneable
	{
		public const string XmlName = "ENVIRONMENT";

		public static Environment ReadFrom(ArcXmlReader reader)
		{
			try
			{
				Environment environment = new Environment();

				if (!reader.IsEmptyElement)
				{
					reader.Read();

					while (!(reader.NodeType == XmlNodeType.EndElement && reader.Name == XmlName))
					{
						if (reader.NodeType == XmlNodeType.Element)
						{
							switch (reader.Name)
							{
                case Capabilities.XmlName: environment.Capabilities = Capabilities.ReadFrom(reader); break;
                case ImageLimit.XmlName: environment.ImageLimit = ImageLimit.ReadFrom(reader); break;
								case Locale.XmlName: environment.Locale = Locale.ReadFrom(reader); break;
								case Screen.XmlName: environment.Screen = Screen.ReadFrom(reader); break;
								case Separators.XmlName: environment.Separators = Separators.ReadFrom(reader); break;
							}
						}

						reader.Read();
					}
				}

				return environment;
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

    public Capabilities Capabilities = null;
		public ImageLimit ImageLimit = null;
    public Locale Locale = null;
    public Screen Screen = null;
    public Separators Separators = null;

		public object Clone()
		{
			Environment clone = new Environment();

      if (Capabilities != null)
      {
        clone.Capabilities = (Capabilities)Capabilities.Clone();
      }
      
      if (ImageLimit != null)
			{
				clone.ImageLimit = (ImageLimit)ImageLimit.Clone();
			}

			if (Locale != null)
			{
				clone.Locale = (Locale)Locale.Clone();
			}

			if (Screen != null)
			{
				clone.Screen = (Screen)Screen.Clone();
			}

			if (Separators != null)
			{
				clone.Separators = (Separators)Separators.Clone();
			}

			return clone;
		}

		public void WriteTo(ArcXmlWriter writer)
		{
			try
			{
				writer.WriteStartElement(XmlName);

				if (Separators != null)
				{
					Separators.WriteTo(writer);
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
