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
using GeoAPI.Geometries;

namespace AppGeo.Clients.ArcIms.ArcXml
{
  [Serializable]
	public class Features : Response, IList<Feature>, ICloneable
	{
		public const string XmlName = "FEATURES";

		public static Features ReadFrom(ArcXmlReader reader)
		{
			try
			{
				Features features = new Features();

				if (!reader.IsEmptyElement)
				{
					reader.Read();

					while (!(reader.NodeType == XmlNodeType.EndElement && reader.Name == XmlName))
					{
						if (reader.NodeType == XmlNodeType.Element)
						{
							switch (reader.Name)
							{
                case "ENVELOPE": features.Envelope = EnvelopeSerializer.ReadFrom(reader); break;
                case FeatureCount.XmlName: features.FeatureCount = FeatureCount.ReadFrom(reader); break;
								case Feature.XmlName: features.Add(Feature.ReadFrom(reader)); break;
							}
						}

						reader.Read();
					}
				}

				return features;
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

		public FeatureCount FeatureCount = null;
		public Envelope Envelope = new Envelope();

		private List<Feature> _list = new List<Feature>();

		public Features() { }

		public Feature this[int index]
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

		public void Add(Feature item)
		{
			_list.Add(item);
		}

    public void Add(object item)
    {
      Add((Feature)item);
    }

		public void Clear()
		{
			_list.Clear();
		}

		public bool Contains(Feature item)
		{
			return _list.Contains(item);
		}

		public void CopyTo(Feature[] array, int arrayIndex)
		{
			_list.CopyTo(array, arrayIndex);
		}

		IEnumerator<Feature> IEnumerable<Feature>.GetEnumerator()
		{
			return _list.GetEnumerator();
		}

		public IEnumerator GetEnumerator()
		{
			return _list.GetEnumerator();
		}

		public int IndexOf(Feature item)
		{
			return _list.IndexOf(item);
		}

		public void Insert(int index, Feature item)
		{
			_list.Insert(index, item);
		}

		public bool Remove(Feature item)
		{
			return _list.Remove(item);
		}

		public void RemoveAt(int index)
		{
			_list.RemoveAt(index);
		}

		public override object Clone()
		{
			Features clone = (Features)this.MemberwiseClone();

			if (FeatureCount != null)
			{
				clone.FeatureCount = (FeatureCount)FeatureCount.Clone();
			}

			clone._list = new List<Feature>();

			foreach (Feature feature in _list)
			{
				clone.Add((Feature)feature.Clone());
			}

			return clone;
		}
	}
}
