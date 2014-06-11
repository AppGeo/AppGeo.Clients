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
	public class FClass : ICloneable
	{
		public const string XmlName = "FCLASS";

		public static FClass ReadFrom(ArcXmlReader reader)
		{
			try
			{
				FClass fClass = new FClass();

				if (reader.HasAttributes)
				{
					while (reader.MoveToNextAttribute())
					{
						string value = reader.ReadContentAsString();

						if (value.Length > 0)
						{
							switch (reader.Name)
							{
								case "type": fClass.Type = (FClassType)ArcXmlEnumConverter.ToEnum(typeof(FClassType), value); break;
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
								case "ENVELOPE": fClass.Envelope = EnvelopeSerializer.ReadFrom(reader); break;

								case Field.XmlName:
									if (fClass.Fields == null)
									{
										fClass.Fields = new Fields();
									}

									fClass.Fields.Add(Field.ReadFrom(reader)); 
									break;
							}
						}

						reader.Read();
					}
				}

				return fClass;
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

		public FClassType Type = FClassType.Point;

		public Envelope Envelope = new Envelope();
		public Fields Fields = null;

		public FClass() { }

		public object Clone()
		{
			FClass clone = (FClass)this.MemberwiseClone();

			if (Fields != null)
			{
				clone.Fields = (Fields)Fields.Clone();
			}

			return clone;
		}
	}
}
