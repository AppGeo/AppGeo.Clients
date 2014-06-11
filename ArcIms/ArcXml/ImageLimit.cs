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
	public class ImageLimit : ICloneable
	{
		public const string XmlName = "IMAGELIMIT";

		public static ImageLimit ReadFrom(ArcXmlReader reader)
		{
			try
			{
				ImageLimit imageLimit = new ImageLimit();

				if (reader.HasAttributes)
				{
					while (reader.MoveToNextAttribute())
					{
						string value = reader.ReadContentAsString();

						if (value.Length > 0)
						{
							switch (reader.Name)
							{
								case "pixelcount": imageLimit.PixelCount = Convert.ToInt32(value); break;
							}
						}
					}

					reader.MoveToElement();
				}

				return imageLimit;
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

		public int PixelCount;

		public ImageLimit() { }

		public object Clone()
		{
			ImageLimit clone = (ImageLimit)this.MemberwiseClone();
			return clone;
		}
	}
}
