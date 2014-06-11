using System;
using GeoAPI.Geometries;

namespace AppGeo.Clients.Transform
{
  public class ConformalTransformation : AffineTransformation 
  {
    public ConformalTransformation(Coordinate[] sourceCoordinates, Coordinate[] destinationCoordinates)
    {
      if (sourceCoordinates.Length != 2)
      {
        throw new ArgumentException("The source coordinates array must contain two coordinates.");
      }

      if (destinationCoordinates.Length != 2)
      {
        throw new ArgumentException("The destination coordinates array must contain two coordinates.");
      }

      Coordinate[] s = sourceCoordinates;
      Coordinate[] src = new Coordinate[3];
      Array.Copy(s, src, 2);
      src[2] = new Coordinate(s[0].X + (s[0].Y - s[1].Y), s[0].Y - (s[0].X - s[1].X));

      Coordinate[] d = destinationCoordinates;
      Coordinate[] des = new Coordinate[3];
      Array.Copy(d, des, 2);
      des[2] = new Coordinate(d[0].X + (d[0].Y - d[1].Y), d[0].Y - (d[0].X - d[1].X));

      Initialize(src, des);
    }
  }
}
