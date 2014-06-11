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
  public partial class Geometry
  {
    public static Geometry FromCommon(IGeometry commonGeometry)
    {
      switch (commonGeometry.OgcGeometryType)
      {
        case OgcGeometryType.Point: return PointN.FromCommon((IPoint)commonGeometry);
        case OgcGeometryType.MultiPoint: return MultipointN.FromCommon((IMultiPoint)commonGeometry);
        case OgcGeometryType.LineString: return PolylineN.FromCommon((ILineString)commonGeometry);
        case OgcGeometryType.MultiLineString: return PolylineN.FromCommon((IMultiLineString)commonGeometry);
        case OgcGeometryType.Polygon: return PolygonN.FromCommon((IPolygon)commonGeometry);
        case OgcGeometryType.MultiPolygon: return PolygonN.FromCommon((IMultiPolygon)commonGeometry);

        default:
          throw new NotSupportedException("Conversion from an IGeometryCollection to an AppGeo.Ags.Geometry is not supported.");
      }
    }

    public virtual IGeometry ToCommon()
    {
      if (this is PointN)
      {
        return ((PointN)this).ToCommon();
      }
      else if (this is MultipointN)
      {
        return ((MultipointN)this).ToCommon();
      }
      else if (this is PolylineN)
      {
        return ((PolylineN)this).ToCommon();
      }
      else if (this is PolygonN)
      {
        return ((PolygonN)this).ToCommon();
      }

      throw new NotSupportedException(String.Format("Conversion from an {0} to an AppGeo.Ags.Geometry is not supported.", GetType().FullName));
    }
  }
}
