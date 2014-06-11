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
	public class Field : ICloneable
	{
		public const string XmlName = "FIELD";

		public static Field ReadFrom(ArcXmlReader reader)
		{
			try
			{
				Field field = new Field();

				if (reader.HasAttributes)
				{
					while (reader.MoveToNextAttribute())
					{
						string value = reader.ReadContentAsString();

						if (value.Length > 0)
						{
							switch (reader.Name)
							{
                case "alias": field.Alias = value; break;
                case "description": field.Description = value; break;
                case "name": field.Name = value; break;
								case "precision": field.Precision = Convert.ToInt32(value); break;
								case "size": field.Size = Convert.ToInt32(value); break;
								case "type": field.Type = (FieldType)ArcXmlEnumConverter.ToEnum(typeof(FieldType), value); break;
								case "value": field.Value = value; break;
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
								case FieldValue.XmlName: field.FieldValue = FieldValue.ReadFrom(reader); break;
							}
						}

						reader.Read();
					}
				}

				return field;
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

    public string Alias = null;
    public string Description = null;
    public string Name = "";
    public int Precision = 0;
    public int Size = 0;
    public FieldType Type = FieldType.None;
    public string Value = "";

		public FieldValue FieldValue = null;

		public Field() { }

		public object Clone()
		{
			Field clone = (Field)this.MemberwiseClone();

			if (FieldValue != null)
			{
				clone.FieldValue = (FieldValue)FieldValue.Clone();
			}

			return clone;
		}
	}
}
