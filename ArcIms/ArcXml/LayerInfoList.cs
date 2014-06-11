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

namespace AppGeo.Clients.ArcIms.ArcXml
{
  [Serializable]
	public class LayerInfoList : List<LayerInfo>, ICloneable
	{
		public LayerInfoList() { }

    public LayerInfoList(IEnumerable<LayerInfo> collection) : base(collection) { }

		public LayerInfo this[string id]
		{
			get
			{
				foreach (LayerInfo layerInfo in this)
				{
					if (String.Compare(layerInfo.ID, id, false) == 0)
					{
						return layerInfo;
					}
				}

				return null;
			}
		}

    public bool Contains(string id)
    {
      return IndexOf(id) >= 0;
    }

    public bool ContainsName(string name)
    {
      return IndexOfName(name) >= 0;
    }

    public LayerInfo FindByName(string name)
    {
      foreach (LayerInfo layerInfo in this)
      {
        if (String.Compare(layerInfo.Name, name, true) == 0)
        {
          return layerInfo;
        }
      }

      return null;
    }

		public int IndexOf(string id)
		{
			for (int i = 0; i < Count; ++i)
			{
        if (String.Compare(this[i].ID, id, false) == 0)
				{
					return i;
				}
			}

			return -1;
		}

    public int IndexOfName(string name)
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

		public object Clone()
		{
      LayerInfoList clone = new LayerInfoList();

			foreach (LayerInfo layerInfo in this)
			{
				clone.Add((LayerInfo)layerInfo.Clone());
			}

			return clone;
		}
	}
}
