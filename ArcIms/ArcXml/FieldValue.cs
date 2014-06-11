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
using GeoAPI.Geometries;

namespace AppGeo.Clients.ArcIms.ArcXml
{
  [Serializable]
	public class FieldValue : ICloneable
	{
		public const string XmlName = "FIELDVALUE";

		public static FieldValue ReadFrom(ArcXmlReader reader)
		{
			try
			{
				FieldValue fieldValue = new FieldValue();

				if (reader.HasAttributes)
				{
					while (reader.MoveToNextAttribute())
					{
						string value = reader.ReadContentAsString();

						if (value.Length > 0)
						{
							switch (reader.Name)
							{
								case "valuestring": fieldValue.ValueString = value; break;
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
								case "POINT": fieldValue.Point = (IPoint)GeometrySerializer.ReadFrom(reader); break;
							}
						}

						reader.Read();
					}
				}

				return fieldValue;
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

		public string ValueString = null;
		public IPoint Point = null;

		public FieldValue() { }

		public object Clone()
		{
			FieldValue clone = (FieldValue)this.MemberwiseClone();

      if (Point != null)
      {
        clone.Point = (IPoint)Point.Clone();
      }

			return clone;
		}
	}
}
