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
using GeoAPI.Geometries;
namespace AppGeo.Clients.Ags.Proxy
{
  public partial class EllipticArc
  {
    private const double SweepAngleDegrees = 3;
    private const double SweepAngle = SweepAngleDegrees * Math.PI / 180;

    // renamed to IsCounterClockwise in AGS 10; kept here for compatibility with AGS 9.3.1

    [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public bool IsCounterClockWise
    {
      get
      {
        return this.isCounterClockwiseField;
      }
      set
      {
        this.isCounterClockwiseField = value;
      }
    }

    public override List<Coordinate> ToCommonCoordinates(bool includeEndPoint)
    {
      List<Coordinate> coords = new List<Coordinate>();

      PointN p = FromPoint as PointN;
      Coordinate fc = new Coordinate(p.X, p.Y);

      p = ToPoint as PointN;
      Coordinate tc = new Coordinate(p.X, p.Y);

      coords.Add(fc);

      if (CenterPoint != null)
      {
        p = CenterPoint as PointN;
        Coordinate cc = new Coordinate(p.X, p.Y);
        EllipticTransform transform = new EllipticTransform(cc, Rotation);

        Coordinate cc0 = new Coordinate(0, 0);
        Coordinate fc0 = EllipseStd ? fc : transform.ToStandard(fc);
        Coordinate tc0 = EllipseStd ? tc : transform.ToStandard(tc);

        Coordinate fcMaj = new Coordinate(fc0.X, fc0.Y / MinorMajorRatio);
        Coordinate tcMaj = new Coordinate(tc0.X, tc0.Y / MinorMajorRatio);

        double majRadius = Math.Sqrt(fcMaj.X * fcMaj.X + fcMaj.Y * fcMaj.Y);
        double cosSweep = Math.Cos(SweepAngle);
        double sinSweep = (IsCounterClockWise ? -1 : 1) * Math.Sin(SweepAngle);

        double dCos = (1 - cosSweep) * majRadius;
        double dSin = sinSweep * majRadius;
        double d2Sweep = (dCos * dCos) + (dSin * dSin);

        Coordinate scMaj = fcMaj;
        Coordinate scMin = new Coordinate(fc0.X * MinorMajorRatio, fc0.Y);

        while (scMaj.Distance2(tcMaj) > d2Sweep)
        {
          double dx = scMaj.X - cc.X;
          double dy = scMaj.Y - cc.Y;
          double x = cc.X + (dx * cosSweep) + (dy * sinSweep);
          double y = cc.Y + (dy * cosSweep) - (dx * sinSweep);

          scMaj = new Coordinate(x, y);

          dx = scMin.X - cc.X;
          dy = scMin.Y - cc.Y;
          x = cc.X + (dx * cosSweep) + (dy * sinSweep);
          y = cc.Y + (dy * cosSweep) - (dx * sinSweep);

          scMin = new Coordinate(x, y);

          if (!(scMin.X == 0 && scMin.Y == 0))
          {
            coords.Add(transform.ToBase(new Coordinate(scMaj.X, scMin.Y)));
          }
        }
      }

      if (includeEndPoint)
      {
        coords.Add(tc);
      }

      return coords;
    }

    private class EllipticTransform
    {
      double _c0;
      double _c1;
      double _c2;
      double _c3;
      double _c4;
      double _c5;

      public EllipticTransform(Coordinate center, double rotation)
      {
        double cos = Math.Cos(-rotation);
        double sin = Math.Sin(-rotation);

        _c0 = cos;
        _c1 = sin;
        _c2 = -center.X;
        _c3 = -sin;
        _c4 = cos;
        _c5 = -center.Y;
      }

      public Coordinate ToStandard(Coordinate c)
      {
        double x = _c0 * c.X + _c1 * c.Y + _c2;
        double y = _c3 * c.X + _c4 * c.Y + _c5;
        return new Coordinate(x, y);
      }

      public Coordinate ToBase(Coordinate c)
      {
        double x = (_c4 * (c.X - _c2) + _c1 * (_c5 - c.Y)) / (_c0 * _c4 - _c1 * _c3);
        double y = _c1 != 0 ? (c.X - x * _c0 - _c2) / _c1 : (c.Y - x * _c3 - _c5) / _c4;
        return new Coordinate(x, y);
      }
    }
  }
}
