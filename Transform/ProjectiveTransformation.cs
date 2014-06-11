using System;
using GeoAPI.Geometries;

namespace AppGeo.Clients.Transform
{
  public class ProjectiveTransformation : Transformation
  {
    public ProjectiveTransformation(Coordinate[] sourceCoordinates, Coordinate[] destinationCoordinates)
    {
      if (sourceCoordinates.Length != 4)
      {
        throw new ArgumentException("The source coordinates array must contain four coordinates.");
      }

      if (destinationCoordinates.Length != 4)
      {
        throw new ArgumentException("The destination coordinates array must contain four coordinates.");
      }

      Initialize(sourceCoordinates, destinationCoordinates);
    }

    private void Initialize(Coordinate[] src, Coordinate[] des)
    {
      double[,] a = new double[8, 8];

      a[0, 0] = src[0].X;
      a[0, 1] = src[0].Y;
      a[0, 2] = 1;
      a[0, 6] = -src[0].X * des[0].X;
      a[0, 7] = -src[0].Y * des[0].X;
      a[1, 3] = src[0].X;
      a[1, 4] = src[0].Y;
      a[1, 5] = 1;
      a[1, 6] = -src[0].X * des[0].Y;
      a[1, 7] = -src[0].Y * des[0].Y;
      a[2, 0] = src[1].X;
      a[2, 1] = src[1].Y;
      a[2, 2] = 1;
      a[2, 6] = -src[1].X * des[1].X;
      a[2, 7] = -src[1].Y * des[1].X;
      a[3, 3] = src[1].X;
      a[3, 4] = src[1].Y;
      a[3, 5] = 1;
      a[3, 6] = -src[1].X * des[1].Y;
      a[3, 7] = -src[1].Y * des[1].Y;
      a[4, 0] = src[2].X;
      a[4, 1] = src[2].Y;
      a[4, 2] = 1;
      a[4, 6] = -src[2].X * des[2].X;
      a[4, 7] = -src[2].Y * des[2].X;
      a[5, 3] = src[2].X;
      a[5, 4] = src[2].Y;
      a[5, 5] = 1;
      a[5, 6] = -src[2].X * des[2].Y;
      a[5, 7] = -src[2].Y * des[2].Y;
      a[6, 0] = src[3].X;
      a[6, 1] = src[3].Y;
      a[6, 2] = 1;
      a[6, 6] = -src[3].X * des[3].X;
      a[6, 7] = -src[3].Y * des[3].X;
      a[7, 3] = src[3].X;
      a[7, 4] = src[3].Y;
      a[7, 5] = 1;
      a[7, 6] = -src[3].X * des[3].Y;
      a[7, 7] = -src[3].Y * des[3].Y;

      // load result vector

      double[] b = new double[8];
      b[0] = des[0].X;
      b[1] = des[0].Y;
      b[2] = des[1].X;
      b[3] = des[1].Y;
      b[4] = des[2].X;
      b[5] = des[2].Y;
      b[6] = des[3].X;
      b[7] = des[3].Y;

      ComputeCoefficients(a, b);
    }

    public override Coordinate Transform(Coordinate c)
    {
      double x = (C[0] * c.X + C[1] * c.Y + C[2]) / (C[6] * c.X + C[7] * c.Y + 1);
      double y = (C[3] * c.X + C[4] * c.Y + C[5]) / (C[6] * c.X + C[7] * c.Y + 1);
      return new Coordinate(x, y);
    }

    public override Coordinate ReverseTransform(Coordinate c)
    {
      double x = (C[7] * (C[5] * c.X - C[2] * c.Y) + C[4] * (C[2] - c.X) + C[1] * (c.Y - C[5])) /
          (C[7] * (C[0] * c.Y - C[3] * c.X) + C[6] * (c.X * C[4] - c.Y * C[1]) - C[0] * C[4] + C[3] * C[1]);
      double y = (x * (C[0] - C[6] * c.X) + C[2] - c.X) / (C[7] * c.X - C[1]);
      return new Coordinate(x, y);
    }
  }
}
