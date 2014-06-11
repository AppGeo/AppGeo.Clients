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
using NetTopologySuite.Geometries;

namespace AppGeo.Clients
{
  public static class GeometryExtensions
  {
    public static double Distance2(this Coordinate thisCoordinate, Coordinate otherCoordinate)
    {
      double dx = thisCoordinate.X - otherCoordinate.X;
      double dy = thisCoordinate.Y - otherCoordinate.Y;
      return dx * dx + dy * dy;
    }

    public static void Reaspect(this Envelope envelope, double newAspectRatio)
    {
      if (envelope.IsNull)
      {
        return;
      }

      if (envelope.Width == 0 || envelope.Height == 0 || newAspectRatio <= 0)
      {
        return;
      }

      if (envelope.Width / envelope.Height > newAspectRatio)
      {
        envelope.ExpandBy(0, envelope.Width / newAspectRatio - envelope.Height);
      }
      else
      {
        envelope.ExpandBy(envelope.Height * newAspectRatio - envelope.Width, 0);
      }
    }

    public static void Reaspect(this Envelope envelope, double width, double height)
    {
      if (width <= 0 && height <= 0)
      {
        return;
      }

      Reaspect(envelope, width / height);
    }

    public static void Reaspect(this Envelope envelope, Envelope other)
    {
      if (other.IsNull)
      {
        return;
      }

      Reaspect(envelope, other.Width, other.Height);
    }
  }
}
