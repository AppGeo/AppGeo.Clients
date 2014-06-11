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
	public class Output : ICloneable
	{
		public const string XmlName = "OUTPUT";

		public static Output ReadFrom(ArcXmlReader reader)
		{
			try
			{
				Output output = new Output();

				if (reader.HasAttributes)
				{
					while (reader.MoveToNextAttribute())
					{
						string value = reader.ReadContentAsString();

						if (value.Length > 0)
						{
							switch (reader.Name)
							{
								case "file": output.File = value; break;
								case "height": output.Height = Convert.ToInt32(value); break;
								case "url": output.Url = value; break;
								case "width": output.Width = Convert.ToInt32(value); break;
								case "type": output.Type = (ImageType)ArcXmlEnumConverter.ToEnum(typeof(ImageType), value); break;
							}
						}
					}

					reader.MoveToElement();
				}

				return output;
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

		public string File = null;
    public string Url = null;
		public int Height = 0;
		public int Width = 0;
		public string BaseUrl = "";
		public string Path = "";
		public string Name = "";
		public ImageType Type = ImageType.Png8;

		public Output() { }

		public object Clone()
		{
			Output clone = (Output)this.MemberwiseClone();
			return clone;
		}

		public void WriteTo(ArcXmlWriter writer)
		{
			try
			{
				writer.WriteStartElement(XmlName);

				if (!String.IsNullOrEmpty(BaseUrl))
				{
					writer.WriteAttributeString("baseurl", BaseUrl);
				}

				if (!String.IsNullOrEmpty(Name))
				{
					writer.WriteAttributeString("name", Name);
				}

				if (!String.IsNullOrEmpty(Path))
				{
					writer.WriteAttributeString("path", Path);
				}

				if (Type != ImageType.Default)
				{
					writer.WriteAttributeString("type", ArcXmlEnumConverter.ToArcXml(typeof(ImageType), Type));
				}

				if (!String.IsNullOrEmpty(Url))
				{
					writer.WriteAttributeString("url", Url);
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
