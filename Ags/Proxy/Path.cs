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
  public partial class Path
  {
    public static Path FromCommon(ILineString commonLineString)
    {
      return FromCommon(commonLineString, new Path());
    }

    protected static Path FromCommon(ILineString commonLineString, Path agsPath)
    {
      agsPath.PointArray = commonLineString.Coordinates.Select(o => Point.FromCoordinate(o)).ToArray();
      return agsPath;
    }

    public new ILineString ToCommon()
    {
      return new NetTopologySuite.Geometries.LineString(ToCoordinates());
    }

    protected Coordinate[] ToCoordinates()
    {
      List<Coordinate> coordinates = new List<Coordinate>();

      if (PointArray != null)
      {
        coordinates = PointArray.Select(o => o.ToCoordinate()).ToList();
      }
      else
      {
        if (SegmentArray != null)
        {
          for (int i = 0; i < SegmentArray.Length; ++i)
          {
            coordinates.AddRange(SegmentArray[i].ToCommonCoordinates(i == SegmentArray.Length - 1));
          }
        }
      }

      return coordinates.ToArray();
    }
  }
}
