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
	public class ImageSize : ICloneable
	{
		public const string XmlName = "IMAGESIZE";

		public static ImageSize ReadFrom(ArcXmlReader reader)
		{
			try
			{
				ImageSize imageSize = new ImageSize();

				if (reader.HasAttributes)
				{
					while (reader.MoveToNextAttribute())
					{
						string value = reader.ReadContentAsString();

						if (value.Length > 0)
						{
							switch (reader.Name)
							{
								case "height": imageSize.Height = Convert.ToInt32(value); break;
								case "width": imageSize.Width = Convert.ToInt32(value); break;
								case "dpi": imageSize.Dpi = Convert.ToInt32(value); break;
								case "printheight": imageSize.PrintHeight = Convert.ToInt32(value); break;
								case "printwidth": imageSize.PrintWidth = Convert.ToInt32(value); break;
								case "scalesymbols": imageSize.ScaleSymbols = Convert.ToBoolean(value); break;
							}
						}
					}

					reader.MoveToElement();
				}

				return imageSize;
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

		public int Height = 200;
		public int Width = 200;
		public int Dpi = 0;
		public int PrintHeight = 0;
		public int PrintWidth = 0;
		public bool ScaleSymbols = false;

		public ImageSize() { }

		public ImageSize(int width, int height)
		{
			Width = width;
			Height = height;
		}

		public object Clone()
		{
			ImageSize clone = (ImageSize)this.MemberwiseClone();
			return clone;
		}

		public void WriteTo(ArcXmlWriter writer)
		{
			try
			{
				writer.WriteStartElement(XmlName);

				if (Height > 0)
				{
					writer.WriteAttributeString("height", Height.ToString());
				}

				if (Width > 0)
				{
					writer.WriteAttributeString("width", Width.ToString());
				}

				if (Dpi > 0)
				{
					writer.WriteAttributeString("dpi", Dpi.ToString());
				}

				if (PrintHeight > 0)
				{
					writer.WriteAttributeString("printheight", PrintHeight.ToString());
				}

				if (PrintWidth > 0)
				{
					writer.WriteAttributeString("printwidth", PrintWidth.ToString());
				}

				if (ScaleSymbols)
				{
					writer.WriteAttributeString("scalesymbols", "true");
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
