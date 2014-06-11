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
using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace AppGeo.Clients.ArcIms.ArcXml
{
  [Serializable]
	public class Services : Response, IList<Service>, ICloneable
	{
		public const string XmlName = "SERVICES";

		public static Services ReadFrom(ArcXmlReader reader)
		{
			try
			{
				Services services = new Services();

				if (!reader.IsEmptyElement)
				{
					reader.Read();

					while (!(reader.NodeType == XmlNodeType.EndElement && reader.Name == XmlName))
					{
						if (reader.NodeType == XmlNodeType.Element)
						{
							switch (reader.Name)
							{
								case Service.XmlName: services.Add(Service.ReadFrom(reader)); break;
							}
						}

						reader.Read();
					}
				}

				return services;
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

		private List<Service> _list = new List<Service>();

		public Services() { }

		public Service this[int index]
		{
			get
			{
				return _list[index];
			}
			set
			{
				_list[index] = value;
			}
		}

		public Service this[string name]
		{
			get
			{
				foreach (Service service in _list)
				{
					if (String.Compare(service.Name, name, true) == 0)
					{
						return service;
					}
				}

				return null;
			}
		}

		public int Count
		{
			get
			{
				return _list.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public void Add(Service item)
		{
			_list.Add(item);
		}

		public void Clear()
		{
			_list.Clear();
		}

		public bool Contains(Service item)
		{
			return _list.Contains(item);
		}

		public void CopyTo(Service[] array, int arrayIndex)
		{
			_list.CopyTo(array, arrayIndex);
		}

		IEnumerator<Service> IEnumerable<Service>.GetEnumerator()
		{
			return _list.GetEnumerator();
		}

		public IEnumerator GetEnumerator()
		{
			return _list.GetEnumerator();
		}

		public int IndexOf(Service item)
		{
			return _list.IndexOf(item);
		}

		public void Insert(int index, Service item)
		{
			_list.Insert(index, item);
		}

		public bool Remove(Service item)
		{
			return _list.Remove(item);
		}

		public void RemoveAt(int index)
		{
			_list.RemoveAt(index);
		}

		public override object Clone()
		{
			Services clone = (Services)this.MemberwiseClone();

			clone._list = new List<Service>();

			foreach (Service service in _list)
			{
				clone._list.Add((Service)service.Clone());
			}

			return clone;
		}
	}
}
