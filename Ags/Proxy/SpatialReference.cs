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
using System.Text;

namespace AppGeo.Clients.Ags.Proxy
{
  public abstract partial class SpatialReference
  {
    public static SpatialReference Create(string value)
    {
      SpatialReference spatialRef = null;
      int wkid;

      if (Int32.TryParse(value, out wkid))
      {
        spatialRef = 4000 <= wkid && wkid < 5000 ? 
          (SpatialReference)new GeographicCoordinateSystem(wkid) : 
          (SpatialReference)new ProjectedCoordinateSystem(wkid);
      }
      else
      {
        spatialRef = value.StartsWith("GEOGCS[") ?
          (SpatialReference)new GeographicCoordinateSystem(value) :
          (SpatialReference)new ProjectedCoordinateSystem(value);
      }

      return spatialRef;
    }
  }
}
