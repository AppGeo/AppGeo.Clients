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
	public class Separators : ICloneable
	{
		public const string XmlName = "SEPARATORS";

		public static Separators ReadFrom(ArcXmlReader reader)
		{
			try
			{
				Separators separators = new Separators();

				if (reader.HasAttributes)
				{
					while (reader.MoveToNextAttribute())
					{
						string value = reader.ReadContentAsString();

						if (value.Length > 0)
						{
							switch (reader.Name)
							{
								case "cs":
									separators.CoordinateSeparator = value[0];
									reader.CoordinateSeparator = new char[] { separators.CoordinateSeparator };
									break;

								case "ts":
									separators.TupleSeparator = value[0];
									reader.TupleSeparator = new char[] { separators.TupleSeparator };
									break;
							}
						}
					}

					reader.MoveToElement();
				}

				return separators;
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

		public char CoordinateSeparator = ' ';
		public char TupleSeparator = ';';

		public object Clone()
		{
      Separators clone = (Separators)this.MemberwiseClone();
			return clone;
		}

		public void WriteTo(ArcXmlWriter writer)
		{
			try
			{
				writer.WriteStartElement(XmlName);

				if (CoordinateSeparator != ' ')
				{
					writer.WriteAttributeString("cs", CoordinateSeparator.ToString());
					writer.CoordinateSeparator = new char[] { CoordinateSeparator };
				}

				if (TupleSeparator != ';')
				{
					writer.WriteAttributeString("ts", TupleSeparator.ToString());
					writer.TupleSeparator = new char[] { TupleSeparator };
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
