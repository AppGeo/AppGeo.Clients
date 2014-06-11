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
  public partial class SpatialFilter
  {
    public SpatialFilter() { }

    public SpatialFilter(string where) 
      : base(where) { }

    public SpatialFilter(string subFields, string where) 
      : base(subFields, where) { }

    public SpatialFilter(Geometry agsGeometry)
    {
      InitializeGeometry(agsGeometry);
    }

    public SpatialFilter(IGeometry commonGeometry) 
      : this(Geometry.FromCommon(commonGeometry)) { }

    public SpatialFilter(Geometry agsGeometry, string where)
      : this(where)
    {
      InitializeGeometry(agsGeometry);
    }

    public SpatialFilter(IGeometry commonGeometry, string where) 
      : this(Geometry.FromCommon(commonGeometry), where) { }

    public SpatialFilter(Geometry agsGeometry, string subFields, string where)
      : this(subFields, where)
    {
      InitializeGeometry(agsGeometry);
    }

    public SpatialFilter(IGeometry commonGeometry, string subFields, string where) 
      : this(Geometry.FromCommon(commonGeometry), subFields, where) { }

    private void InitializeGeometry(Geometry agsGeometry)
    {
      FilterGeometry = agsGeometry;
      SpatialRel = agsGeometry is Polygon ? esriSpatialRelEnum.esriSpatialRelContains : esriSpatialRelEnum.esriSpatialRelIntersects;
    }
  }
}
