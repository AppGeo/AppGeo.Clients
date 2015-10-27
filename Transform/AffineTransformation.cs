using System;
using GeoAPI.Geometries;

namespace AppGeo.Clients.Transform
{
  public class AffineTransformation : Transformation
  {
    protected AffineTransformation() { }

    public AffineTransformation(Coordinate[] sourceCoordinates, Coordinate[] destinationCoordinates)
    {
      if (sourceCoordinates.Length != 3)
      {
        throw new ArgumentException("The source coordinates array must contain three coordinates.");
      }

      if (destinationCoordinates.Length != 3)
      {
        throw new ArgumentException("The destination coordinates array must contain three coordinates.");
      }

      Initialize(sourceCoordinates, destinationCoordinates);
    }

    public AffineTransformation(double imageWidth, double imageHeight, Envelope mapExtent)
    {
      mapExtent.Reaspect(imageWidth, imageHeight);

      Coordinate[] src = new Coordinate[3];
      src[0] = new Coordinate(0, 0);
      src[1] = new Coordinate(imageWidth, 0);
      src[2] = new Coordinate(imageWidth, imageHeight);

      Coordinate[] des = new Coordinate[3];
      des[0] = new Coordinate(mapExtent.MinX, mapExtent.MaxY);
      des[1] = new Coordinate(mapExtent.MaxX, mapExtent.MaxY);
      des[2] = new Coordinate(mapExtent.MaxX, mapExtent.MinY);

      Initialize(src, des);
    }

    protected void Initialize(Coordinate[] src, Coordinate[] des)
    {
      double[,] a = new double[6, 6];

      a[0, 0] = src[0].X;
      a[0, 1] = src[0].Y;
      a[0, 2] = 1;
      a[1, 3] = src[0].X;
      a[1, 4] = src[0].Y;
      a[1, 5] = 1;
      a[2, 0] = src[1].X;
      a[2, 1] = src[1].Y;
      a[2, 2] = 1;
      a[3, 3] = src[1].X;
      a[3, 4] = src[1].Y;
      a[3, 5] = 1;
      a[4, 0] = src[2].X;
      a[4, 1] = src[2].Y;
      a[4, 2] = 1;
      a[5, 3] = src[2].X;
      a[5, 4] = src[2].Y;
      a[5, 5] = 1;

      // load result vector

      double[] b = new double[6];
      b[0] = des[0].X;
      b[1] = des[0].Y;
      b[2] = des[1].X;
      b[3] = des[1].Y;
      b[4] = des[2].X;
      b[5] = des[2].Y;

      ComputeCoefficients(a, b);
    }

    public override Coordinate Transform(Coordinate c)
    {
      double x = C[0] * c.X + C[1] * c.Y + C[2];
      double y = C[3] * c.X + C[4] * c.Y + C[5];
      return new Coordinate(x, y);
    }

    public override Coordinate ReverseTransform(Coordinate c)
    {
      double d = C[0] * C[4] - C[1] * C[3];
      double x = (C[4] * (c.X - C[2]) + C[1] * (C[5] - c.Y)) / d;
      double y = (-C[3] * c.X + C[0] * c.Y + C[3] * C[2] - C[0] * C[5]) / d;
      return new Coordinate(x, y);
    }
  }
}