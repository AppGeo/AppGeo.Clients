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
	public class DataFrameInfoList : List<DataFrameInfo>, ICloneable
	{
		public DataFrameInfoList() { }

    public DataFrameInfoList(IEnumerable<DataFrameInfo> collection) : base(collection) { }

		public DataFrameInfo this[string name]
		{
			get
			{
				foreach (DataFrameInfo dataFrameInfo in this)
				{
					if (String.Compare(dataFrameInfo.Name, name, true) == 0)
					{
						return dataFrameInfo;
					}
				}
				return null;
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
        if (this[i].Name == name)
				{
					return i;
				}
			}

			return -1;
		}

		public object Clone()
		{
      DataFrameInfoList clone = new DataFrameInfoList();

			foreach (DataFrameInfo dataFrameInfo in this)
			{
				clone.Add((DataFrameInfo)dataFrameInfo.Clone());
			}

			return clone;
		}
	}
}
