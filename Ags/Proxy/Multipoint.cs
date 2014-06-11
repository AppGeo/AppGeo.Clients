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
using System.Linq;
using GeoAPI.Geometries;

namespace AppGeo.Clients.Ags.Proxy
{
  public abstract partial class Multipoint
  {
    public static MultipointN FromCommon(IPoint commonPoint)
    {
      MultipointN agsMultiPoint = new MultipointN();
      agsMultiPoint.PointArray = new Point[1];
      agsMultiPoint.PointArray[0] = PointN.FromCommon(commonPoint);
      return agsMultiPoint;
    }
      
    public static MultipointN FromCommon(IMultiPoint commonMultiPoint)
    {
      MultipointN agsMultiPoint = new MultipointN();
      agsMultiPoint.PointArray = commonMultiPoint.Select(o => PointN.FromCommon(o)).Cast<Point>().ToArray();
      return agsMultiPoint;
    }
  }
}
