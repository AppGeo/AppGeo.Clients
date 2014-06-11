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
using System.Drawing;

namespace AppGeo.Clients.ArcIms.ArcXml
{
  [Serializable]
  public class Background : ICloneable
	{
		public const string XmlName = "BACKGROUND";

		public static Background ReadFrom(ArcXmlReader reader)
		{
			try
			{
				Background background = new Background();

				if (reader.HasAttributes)
				{
					while (reader.MoveToNextAttribute())
					{
						string value = reader.ReadContentAsString();

						if (value.Length > 0)
						{
							switch (reader.Name)
							{
								case "color": background.Color = ColorConverter.ToColor(value); break;
								case "transcolor": background.TransparentColor = ColorConverter.ToColor(value); break;
							}
						}
					}

					reader.MoveToElement();
				}

				return background;
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

		public Color Color = Color.White;
		public Color TransparentColor = Color.Empty;

		public Background() { }

		public Background(Color color)
		{
			Color = color;
		}

		public object Clone()
		{
			Background clone = (Background)this.MemberwiseClone();
			return clone;
		}

		public void WriteTo(ArcXmlWriter writer)
		{
			try
			{
				writer.WriteStartElement(XmlName);

        if (!Color.IsEmpty)
        {
          writer.WriteAttributeString("color", ColorConverter.ToArcXml(Color));
        }
        
        if (!TransparentColor.IsEmpty)
				{
          writer.WriteAttributeString("transcolor", ColorConverter.ToArcXml(TransparentColor));
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
