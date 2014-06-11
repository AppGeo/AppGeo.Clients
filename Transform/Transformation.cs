using System;
using System.Linq;
using GeoAPI.Geometries;
using NetTopologySuite.Geometries;

namespace AppGeo.Clients.Transform
{
  public abstract class Transformation
  {
    protected double[] C;

    public abstract Coordinate Transform(Coordinate c);

    public abstract Coordinate ReverseTransform(Coordinate c);

    public Coordinate[] Transform(Coordinate[] c)
    {
      return c.Select(o => Transform(o)).ToArray();
    }

    public Coordinate[] ReverseTransform(Coordinate[] c)
    {
      return c.Select(o => ReverseTransform(o)).ToArray();
    }

    public IPoint Transform(IPoint g) 
    {
      return new Point(Transform(g.Coordinate));
    }

    public IPoint ReverseTransform(IPoint g) 
    {
      return new Point(ReverseTransform(g.Coordinate));
    }

    public ILineString Transform(ILineString g) 
    {
      return new LineString(Transform(g.Coordinates));
    }

    public ILineString ReverseTransform(ILineString g) 
    {
      return new LineString(ReverseTransform(g.Coordinates));
    }

    public IPolygon Transform(IPolygon g) 
    {
      ILinearRing exteriorRing = new LinearRing(Transform(g.ExteriorRing.Coordinates));
      ILinearRing[] interiorRings = g.InteriorRings.Select(o => new LinearRing(Transform(o.Coordinates))).ToArray();
      return new Polygon(exteriorRing, interiorRings);
    }

    public IPolygon ReverseTransform(IPolygon g)
    {
      ILinearRing exteriorRing = new LinearRing(ReverseTransform(g.ExteriorRing.Coordinates));
      ILinearRing[] interiorRings = g.InteriorRings.Select(o => new LinearRing(ReverseTransform(o.Coordinates))).ToArray();
      return new Polygon(exteriorRing, interiorRings);
    }

    public IMultiPoint Transform(MultiPoint g) 
    {
      return new MultiPoint(g.Geometries.Cast<IPoint>().Select(o => Transform(o)).ToArray());
    }

    public IMultiPoint ReverseTransform(MultiPoint g)
    {
      return new MultiPoint(g.Geometries.Cast<IPoint>().Select(o => ReverseTransform(o)).ToArray());
    }

    public IMultiLineString Transform(MultiLineString g) 
    {
      return new MultiLineString(g.Geometries.Cast<ILineString>().Select(o => Transform(o)).ToArray());
    }

    public IMultiLineString ReverseTransform(MultiLineString g)
    {
      return new MultiLineString(g.Geometries.Cast<ILineString>().Select(o => ReverseTransform(o)).ToArray());
    }

    public IMultiPolygon Transform(MultiPolygon g) 
    {
      return new MultiPolygon(g.Geometries.Cast<IPolygon>().Select(o => Transform(o)).ToArray());
    }

    public IMultiPolygon ReverseTransform(MultiPolygon g)
    {
      return new MultiPolygon(g.Geometries.Cast<IPolygon>().Select(o => ReverseTransform(o)).ToArray());
    }

    protected void ComputeCoefficients(double[,] a, double[] b)
    {
      int n = b.Length;

      int[] ipvt = new int[n];
      ipvt[n - 1] = 1;

      // triangularize

      for (int k = 0; k < n - 1; ++k)
      {
        // find pivot

        int m = k;

        for (int i = k + 1; i < n; ++i)
        {
          if (Math.Abs(a[i, k]) > Math.Abs(a[m, k]))
          {
            m = i;
          }
        }

        ipvt[k] = m;

        if (m != k)
        {
          ipvt[n - 1] = -ipvt[n - 1];
        }

        double temp = a[m, k];
        a[m, k] = a[k, k];
        a[k, k] = temp;

        if (a[k, k] == 0)
        {
          continue;
        }

        // multipliers

        for (int i = k + 1; i < n; ++i)
        {
          a[i, k] /= -a[k, k];
        }

        // interchange and eliminate by columns

        for (int j = k + 1; j < n; ++j)
        {
          temp = a[m, j];
          a[m, j] = a[k, j];
          a[k, j] = temp;

          if (a[k, j] == 0)
          {
            continue;
          }

          for (int i = k + 1; i < n; ++i)
          {
            a[i, j] += a[i, k] * a[k, j];
          }
        }
      }

      // forward elimination

      for (int k = 0; k < n - 1; ++k)
      {
        int m = ipvt[k];
        double temp = b[m];
        b[m] = b[k];
        b[k] = temp;

        for (int i = k + 1; i < n; ++i)
        {
          b[i] += a[i, k] * b[k];
        }
      }

      // back substitution

      for (int k = n - 1; k > 0; --k)
      {
        b[k] /= a[k, k];

        for (int i = 0; i < k; ++i)
        {
          b[i] += -a[i, k] * b[k];
        }
      }

      b[0] /= a[0, 0];
      C = b;
    }
  }
}
