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
  public partial class CircularArc
  {
    private const double SweepAngleDegrees = 3;
    private const double SweepAngle = SweepAngleDegrees * Math.PI / 180;

    public override List<Coordinate> ToCommonCoordinates(bool includeEndPoint)
    {
      List<Coordinate> coords = new List<Coordinate>();

      PointN p = FromPoint as PointN;
      Coordinate fc = new Coordinate(p.X, p.Y);

      p = ToPoint as PointN;
      Coordinate tc = new Coordinate(p.X, p.Y);

      coords.Add(fc);

      if (!IsLine)
      {
        p = CenterPoint as PointN;
        Coordinate cc = new Coordinate(p.X, p.Y);

        double angle = 0;
        double twoPI = Math.PI * 2;

        if (fc == tc)
        {
          angle = twoPI;
        }
        else
        {
          angle = ((Math.Atan2(tc.Y - cc.Y, tc.X - cc.X) - Math.Atan2(fc.Y - cc.Y, fc.X - cc.X)) + twoPI) % twoPI;

          if (!IsCounterClockwise)
          {
            angle = twoPI - angle;
          }
        }

        int pointCount = Convert.ToInt32(Math.Floor(angle / SweepAngle)) - (angle % SweepAngle < 0.000001 ? 1 : 0);

        double radius = fc.Distance(cc);

        double cosSweep = Math.Cos(SweepAngle);
        double sinSweep = (IsCounterClockwise ? -1 : 1) * Math.Sin(SweepAngle);

        Coordinate sc = fc;

        for (int n = 0; n < pointCount; ++n)
        {
          double dx = sc.X - cc.X;
          double dy = sc.Y - cc.Y;
          double x = cc.X + (dx * cosSweep) + (dy * sinSweep);
          double y = cc.Y + (dy * cosSweep) - (dx * sinSweep);

          sc = new Coordinate(x, y);

          if (sc != cc)
          {
            coords.Add(sc);
          }
        }
      }

      if (includeEndPoint)
      {
        coords.Add(tc);
      }

      return coords;
    }
  }
}
