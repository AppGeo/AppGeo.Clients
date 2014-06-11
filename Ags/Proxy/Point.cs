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
using GeoAPI.Geometries;

namespace AppGeo.Clients.Ags.Proxy
{
  public abstract partial class Point
  {
    public static PointN FromCommon(IPoint commonPoint)
    {
      return FromCoordinate(commonPoint.Coordinate);
    }

    public static PointN FromCoordinate(Coordinate coordinate)
    {
      PointN agsPoint = new PointN();

      agsPoint.X = coordinate.X;
      agsPoint.Y = coordinate.Y;

      return agsPoint;
    }

    public virtual Coordinate ToCoordinate()
    {
      return null;
    }
  }
}
