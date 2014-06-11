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
using System.Collections.Generic;
using System.Xml;

namespace AppGeo.Clients.ArcIms.ArcXml
{
  [Serializable]
	public class Fields : List<Field>, ICloneable
	{
		public const string XmlName = "FIELDS";

		public static Fields ReadFrom(ArcXmlReader reader)
		{
			try
			{
				Fields fields = new Fields();

				if (!reader.IsEmptyElement)
				{
					reader.Read();

					while (!(reader.NodeType == XmlNodeType.EndElement && reader.Name == XmlName))
					{
						if (reader.NodeType == XmlNodeType.Element)
						{
							switch (reader.Name)
							{
								case Field.XmlName: fields.Add(Field.ReadFrom(reader)); break;
							}
						}

						reader.Read();
					}
				}

				return fields;
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

		public Fields() { }

    public Fields(IEnumerable<Field> collection) : base(collection) { }

		public Field this[string name]
		{
			get
			{
				foreach (Field field in this)
				{
					if (String.Compare(field.Name, name, true) == 0)
					{
						return field;
					}
				}

				throw new ArcXmlException(String.Format("Could not find a field named '{0}'", name));
			}
		}

    public bool Contains(string name)
    {
      return IndexOf(name) >= 0;
    }

    public int IndexOf(string name)
		{
			for (int i = 0; i < Count; ++i)
			{
        if (String.Compare(this[i].Name, name, true) == 0)
				{
					return i;
				}
			}

			return -1;
		}

    public List<String> GetNames()
    {
      List<String> names = new List<String>();

      foreach (Field field in this)
      {
        names.Add(field.Name);
      }

      return names;
    }

		public object Clone()
		{
			Fields clone = new Fields();

			foreach (Field field in this)
			{
				clone.Add((Field)field.Clone());
			}

			return clone;
		}
	}
}
