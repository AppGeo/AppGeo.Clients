﻿//  Copyright 2012 Applied Geographics, Inc.
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
  public partial class PolygonN
  {
    public new IMultiPolygon ToCommon()
    {
      List<PolygonBuilder> polygonBuilders = new List<PolygonBuilder>();
      PolygonBuilder currentBuilder = null;

      for (int i = 0; i < RingArray.Length; ++i)
      {
        ILinearRing commonLinearRing = RingArray[i].ToCommon();

        if (currentBuilder == null || !currentBuilder.ExteriorRing.Contains(commonLinearRing))
        {
          currentBuilder = new PolygonBuilder(commonLinearRing);
          polygonBuilders.Add(currentBuilder);
        }
        else
        {
          currentBuilder.InteriorRings.Add(commonLinearRing);
        }
      }

      return new NetTopologySuite.Geometries.MultiPolygon(polygonBuilders.Select(o => o.ToPolygon()).ToArray());
    }

    private class PolygonBuilder
    {
      public ILinearRing ExteriorRing;
      public List<ILinearRing> InteriorRings;

      public PolygonBuilder(ILinearRing exteriorRing)
      {
        ExteriorRing = exteriorRing;
        InteriorRings = new List<ILinearRing>();
      }

      public IPolygon ToPolygon()
      {
        return new NetTopologySuite.Geometries.Polygon(ExteriorRing, InteriorRings.ToArray());
      }
    }
  }
}
