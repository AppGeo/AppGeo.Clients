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
using System.IO;
using System.Xml;

namespace AppGeo.Clients.ArcIms.ArcXml
{
  [Serializable]
	public class TocClass
	{
		public const string XmlName = "TOCCLASS";

		public static TocClass ReadFrom(ArcXmlReader reader)
		{
			try
			{
				TocClass tocClass = new TocClass();

				if (reader.HasAttributes)
				{
					while (reader.MoveToNextAttribute())
					{
						string value = reader.ReadContentAsString();

						if (value.Length > 0)
						{
							switch (reader.Name)
							{
								case "description": tocClass.Description = value; break;
								case "label": tocClass.Label = value; break;
							}
						}
					}

					reader.MoveToElement();
				}

				if (!reader.IsEmptyElement)
				{
          string imageData = reader.ReadString();
          tocClass.Image = Convert.FromBase64String(imageData);

          Bitmap bitmap = new Bitmap(new MemoryStream(tocClass.Image));
          tocClass.ImageIsTransparent = true;

          for (int row = 0; row < bitmap.Width; ++row)
          {
            for (int col = 0; col < bitmap.Height; ++col)
            {
              if (bitmap.GetPixel(row, col).A > 0)
              {
                tocClass.ImageIsTransparent = false;
                break;
              }
            }

            if (!tocClass.ImageIsTransparent)
            {
              break;
            }
          }
				}

				return tocClass;
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

		public string Description = null;
		public string Label = null;
		public byte[] Image = null;
    public bool ImageIsTransparent = false;

		public TocClass() { }

		public object Clone()
		{
			TocClass clone = (TocClass)this.MemberwiseClone();

			if (Image != null)
			{
				clone.Image = new byte[Image.Length];
				Image.CopyTo(clone.Image, 0);
			}

			return clone;
		}
	}
}
