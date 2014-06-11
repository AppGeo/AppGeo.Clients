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
	public class FilterCoordSys : CoordSys
	{
		public new const string XmlName = "FILTERCOORDSYS";

    public new static FilterCoordSys ReadFrom(ArcXmlReader reader)
    {
      return (FilterCoordSys)ReadFrom(reader, new FilterCoordSys(), XmlName);
    }

    public FilterCoordSys() : base(XmlName, null) { }

    public FilterCoordSys(string value) : base(XmlName, value) { }
	}
}
