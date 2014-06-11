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
using System.Linq;
using GeoAPI.Geometries;

namespace AppGeo.Clients.Ags.Proxy
{
  public abstract partial class Polygon
  {
    public static PolygonN FromCommon(IPolygon commonPolygon)
    {
      List<Ring> rings = new List<Ring>();
      rings.Add(Ring.FromCommon(commonPolygon.ExteriorRing));

      foreach (ILineString commonLineString in commonPolygon.InteriorRings)
      {
        rings.Add(Ring.FromCommon(commonLineString));
      }

      PolygonN agsPolygon = new PolygonN();
      agsPolygon.RingArray = rings.ToArray();

      return agsPolygon;
    }

    public static PolygonN FromCommon(IMultiPolygon commonMultiPolygon)
    {
      List<Ring> rings = new List<Ring>();

      foreach (IPolygon commonPolygon in commonMultiPolygon.Geometries.Cast<IPolygon>())
      {
        rings.Add(Ring.FromCommon(commonPolygon.ExteriorRing));

        foreach (ILineString commonLineString in commonPolygon.InteriorRings)
        {
          rings.Add(Ring.FromCommon(commonLineString));
        }
      }

      PolygonN agsPolygon = new PolygonN();
      agsPolygon.RingArray = rings.ToArray();

      return agsPolygon;
    }
  }
}
