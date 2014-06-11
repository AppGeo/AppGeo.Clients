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
  public partial class BezierCurve
  {
    private const double SweepAngle = 3 * Math.PI / 180;

    public override List<Coordinate> ToCommonCoordinates(bool includeEndPoint)
    {
      // create a coordinate array containing the from-point, all the control points, and the to-point

      Coordinate[] c = new Coordinate[ControlPointArray.Length + 2];
      PointN p = FromPoint as PointN;
      c[0] = new Coordinate(p.X, p.Y);

      ControlPointArray.Cast<PointN>().Select(o => o.ToCoordinate()).ToArray().CopyTo(c, 1);

      p = ToPoint as PointN;
      c[c.Length - 1] = new Coordinate(p.X, p.Y);

      // determine the number of points to interpolate

      int interpolateCount = 0;

      for (int i = 1; i < c.Length - 1; ++i)
      {
        double ax = c[i - 1].X - c[i].X;
        double ay = c[i - 1].Y - c[i].Y;
        double bx = c[i + 1].X - c[i].X;
        double by = c[i + 1].Y - c[i].Y;

        double dot = ax * bx + ay * by;
        double ad = Math.Sqrt(ax * ax + ay * ay);
        double bd = Math.Sqrt(bx * bx + by * by);

        double angle = Math.PI - Math.Abs(Math.Acos(dot / (ad * bd)));
        interpolateCount += Convert.ToInt32(Math.Floor(angle / SweepAngle));
      }

      // add the from-point to the output coordinates

      List<Coordinate> coords = new List<Coordinate>();
      coords.Add(c[0]);

      // add interpolated points along the Bezier curve to the output coordinates

      for (int i = 1; i < interpolateCount; ++i)
      {
        coords.Add(Interpolate(c, (double)i / interpolateCount));
      }

      // add the to-point to the output coordinates

      if (includeEndPoint)
      {
        coords.Add(c[c.Length - 1]);
      }

      return coords;
    }

    private Coordinate Interpolate(Coordinate[] c0, double t)
    {
      while (c0.Length > 1)
      {
        Coordinate[] c1 = new Coordinate[c0.Length - 1];

        for (int i = 0; i < c0.Length - 1; ++i)
        {
          double x = c0[i].X + ((c0[i + 1].X - c0[i].X) * t);
          double y = c0[i].Y + ((c0[i + 1].Y - c0[i].Y) * t);
          c1[i] = new Coordinate(x, y);
        }

        c0 = c1;
      }

      return c0[0];
    }
  }
}
