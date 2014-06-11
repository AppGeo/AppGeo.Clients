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
	public class Layers : List<Layer>, ICloneable
	{
		public const string XmlName = "LAYERS";

		public static Layers ReadFrom(ArcXmlReader reader)
		{
			try
			{
				Layers layers = new Layers();

				if (!reader.IsEmptyElement)
				{
					reader.Read();

					while (!(reader.NodeType == XmlNodeType.EndElement && reader.Name == XmlName))
					{
						if (reader.NodeType == XmlNodeType.Element)
						{
							switch (reader.Name)
							{
								case Layer.XmlName: layers.Add(Layer.ReadFrom(reader)); break;
							}
						}

						reader.Read();
					}
				}

				return layers;
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

		public Layers() { }

    public Layers(IEnumerable<Layer> collection) : base(collection) { }

		public Layer this[string id]
		{
			get
			{
				foreach (Layer layer in this)
				{
					if (String.Compare(layer.ID, id, false) == 0)
					{
						return layer;
					}
				}

				foreach (Layer layer in this)
				{
					if (String.Compare(layer.Name, id, true) == 0)
					{
						return layer;
					}
				}

				return null;
			}
		}

		public int IndexOf(string item)
		{
			for (int i = 0; i < Count; ++i)
			{
				if (this[i].ID == item)
				{
					return i;
				}
			}

			for (int i = 0; i < Count; ++i)
			{
				if (this[i].Name == item)
				{
					return i;
				}
			}

			return -1;
		}

		public object Clone()
		{
			Layers clone = new Layers();

			foreach (Layer layer in this)
			{
				clone.Add((Layer)layer.Clone());
			}

			return clone;
		}
	}
}
