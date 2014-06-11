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
using System.Xml;
using GeoAPI.Geometries;
using NetTopologySuite.Geometries;

namespace AppGeo.Clients.ArcIms.ArcXml
{
	public static class GeometrySerializer
	{
    private static Coordinate ReadAsCoordinateFrom(ArcXmlReader reader)
    {
      try
      {
        double x = Double.NaN;
        double y = Double.NaN;

        if (reader.HasAttributes)
        {
          while (reader.MoveToNextAttribute())
          {
            string value = reader.ReadContentAsString();

            if (value.Length > 0)
            {
              switch (reader.Name)
              {
                case "x": x = Convert.ToDouble(value); break;
                case "y": y = Convert.ToDouble(value); break;

                case "coords":
                  string[] c = value.Split(reader.CoordinateSeparator);
                  x = Convert.ToDouble(c[0]);
                  y = Convert.ToDouble(c[1]);
                  break;
              }
            }
          }

          reader.MoveToElement();
        }

        return new Coordinate(x, y);
      }
      catch (Exception ex)
      {
        throw new ArcXmlException("Could not read POINT element.", ex);
      }
    }

    private static Coordinate[] ReadCoordsFrom(ArcXmlReader reader)
    {
      try
      {
        Coordinate[] coordinates = new Coordinate[0];

        if (!reader.IsEmptyElement)
        {
          reader.Read();

          while (!(reader.NodeType == XmlNodeType.EndElement && reader.Name == "COORDS"))
          {
            if (reader.NodeType == XmlNodeType.Text)
            {
              string[] coords = reader.ReadContentAsString().Split(reader.TupleSeparator);
              coordinates = new Coordinate[coords.Length];

              for (int i = 0; i < coords.Length; ++i)
              {
                string[] c = coords[i].Split(reader.CoordinateSeparator);
                coordinates[i] = new Coordinate(Convert.ToDouble(c[0]), Convert.ToDouble(c[1]));
              }
            }

            reader.Read();
          }
        }

        return coordinates;
      }
      catch (Exception ex)
      {
        throw new ArcXmlException("Could not read COORDS element.", ex);
      }
    }

    public static IGeometry ReadFrom(ArcXmlReader reader)
		{
			string arcXmlName = reader.Name;

			try
			{
				switch (arcXmlName)
				{
					case "POINT":
						return ReadPointFrom(reader);
					case "MULTIPOINT":
						return ReadMultiPointFrom(reader);
					case "POLYLINE":
						return ReadPolylineFrom(reader);
					case "POLYGON":
						return ReadPolygonFrom(reader);
					default:
						throw new ArcXmlException(String.Format("Could not read {0} element, unsupported.", arcXmlName));
				}
			}
			catch (Exception ex)
			{
				if (ex is ArcXmlException)
				{
					throw ex;
				}
				else
				{
          throw new ArcXmlException(String.Format("Could not read {0} element.", arcXmlName), ex);
				}
			}
		}

		private static ILineString ReadLineStringFrom(ArcXmlReader reader, string arcXmlName)
		{
			try
			{
        List<Coordinate> coordinates = new List<Coordinate>();

				if (!reader.IsEmptyElement)
				{
					reader.Read();

					while (!(reader.NodeType == XmlNodeType.EndElement && reader.Name == arcXmlName))
					{
						if (reader.NodeType == XmlNodeType.Element)
						{
							switch (reader.Name)
							{
								case "COORDS": coordinates.AddRange(ReadCoordsFrom(reader)); break;
                case "POINT": coordinates.Add(ReadAsCoordinateFrom(reader)); break;
							}
						}

						reader.Read();
					}
				}

				return new LineString(coordinates.ToArray());
			}
			catch (Exception ex)
			{
				if (ex is ArcXmlException)
				{
					throw ex;
				}
				else
				{
          throw new ArcXmlException(String.Format("Could not read {0} element.", arcXmlName), ex);
				}
			}
		}

		public static IMultiLineString ReadPolylineFrom(ArcXmlReader reader)
		{
			try
			{
        List<ILineString> lineStrings = new List<ILineString>();

				if (!reader.IsEmptyElement)
				{
					reader.Read();

					while (!(reader.NodeType == XmlNodeType.EndElement && reader.Name == "POLYLINE"))
					{
						if (reader.NodeType == XmlNodeType.Element)
						{
							switch (reader.Name)
							{
								case "PATH": lineStrings.Add(ReadLineStringFrom(reader, "PATH")); break;
							}
						}

						reader.Read();
					}
				}

        return new MultiLineString(lineStrings.ToArray());
			}
			catch (Exception ex)
			{
				if (ex is ArcXmlException)
				{
					throw ex;
				}
				else
				{
					throw new ArcXmlException("Could not read POLYLINE element.", ex);
				}
			}
		}

		public static IMultiPoint ReadMultiPointFrom(ArcXmlReader reader)
		{
			try
			{
        List<IPoint> points = new List<IPoint>();

				if (!reader.IsEmptyElement)
				{
					reader.Read();

					while (!(reader.NodeType == XmlNodeType.EndElement && reader.Name == "MULTIPOINT"))
					{
						if (reader.NodeType == XmlNodeType.Element)
						{
							switch (reader.Name)
							{
								case "POINT": points.Add(ReadPointFrom(reader)); break;

                case "COORDS":
                  Coordinate[] coordinates = ReadCoordsFrom(reader);
                  
                  foreach (Coordinate c in coordinates)
                  {
                    points.Add(new Point(c));
                  }
                  break;
              }
						}

						reader.Read();
					}
				}

				return new MultiPoint(points.ToArray());
			}
			catch (Exception ex)
			{
				if (ex is ArcXmlException)
				{
					throw ex;
				}
				else
				{
					throw new ArcXmlException("Could not read MULTIPOINT element.", ex);
				}
			}
		}

    public static IPoint ReadPointFrom(ArcXmlReader reader)
    {
      return new Point(ReadAsCoordinateFrom(reader));
    }

		public static IMultiPolygon ReadPolygonFrom(ArcXmlReader reader)
		{
			try
			{
        List<IPolygon> polygons = new List<IPolygon>();

				if (!reader.IsEmptyElement)
				{
					reader.Read();

					while (!(reader.NodeType == XmlNodeType.EndElement && reader.Name == "POLYGON"))
					{
						if (reader.NodeType == XmlNodeType.Element)
						{
							switch (reader.Name)
							{
								case "RING": polygons.Add(ReadRingFrom(reader)); break;
							}
						}

						reader.Read();
					}
				}

				return new MultiPolygon(polygons.ToArray());
			}
			catch (Exception ex)
			{
				if (ex is ArcXmlException)
				{
					throw ex;
				}
				else
				{
					throw new ArcXmlException("Could not read POLYGON element.", ex);
				}
			}
		}

		public static IPolygon ReadRingFrom(ArcXmlReader reader)
		{
			try
			{
        List<Coordinate> exteriorRing = new List<Coordinate>();
        List<ILinearRing> interiorRings = new List<ILinearRing>();

				if (!reader.IsEmptyElement)
				{
					reader.Read();

					while (!(reader.NodeType == XmlNodeType.EndElement && reader.Name == "RING"))
					{
						if (reader.NodeType == XmlNodeType.Element)
						{
							switch (reader.Name)
							{
                case "COORDS": exteriorRing.AddRange(ReadCoordsFrom(reader)); break;
                case "POINT": exteriorRing.Add(ReadAsCoordinateFrom(reader)); break;
								case "HOLE": interiorRings.Add(new LinearRing(ReadLineStringFrom(reader, "HOLE").Coordinates)); break;
							}
						}

						reader.Read();
					}
				}

				return new Polygon(new LinearRing(exteriorRing.ToArray()), interiorRings.ToArray());
			}
			catch (Exception ex)
			{
				if (ex is ArcXmlException)
				{
					throw ex;
				}
				else
				{
					throw new ArcXmlException("Could not read RING element.", ex);
				}
			}
		}

    public static void WriteAsMultiPointTo(ArcXmlWriter writer, IPoint point)
    {
      try
      {
        writer.WriteStartElement("MULTIPOINT");
        WriteTo(writer, point);
        writer.WriteEndElement();
      }
      catch (Exception ex)
      {
        if (ex is ArcXmlException)
        {
          throw ex;
        }
        else
        {
          throw new ArcXmlException("Could not write Point object.", ex);
        }
      }
    }

    private static void WriteAsPointTo(ArcXmlWriter writer, Coordinate coordinate)
    {
      writer.WriteStartElement("POINT");

      if (coordinate == null)
      {
        writer.WriteAttributeString("x", coordinate.X.ToString());
        writer.WriteAttributeString("y", coordinate.Y.ToString());
      }

      writer.WriteEndElement();
    }

		public static void WriteTo(ArcXmlWriter writer, IGeometry geometry)
		{
			switch (geometry.OgcGeometryType)
			{
        case OgcGeometryType.Point:
					WriteTo(writer, (IPoint)geometry);
					break;
        case OgcGeometryType.LineString:
          WriteTo(writer, (ILineString)geometry);
					break;
        case OgcGeometryType.Polygon:
          WriteTo(writer, (IPolygon)geometry);
					break;
        case OgcGeometryType.MultiPoint:
          WriteTo(writer, (IMultiPoint)geometry);
					break;
        case OgcGeometryType.MultiLineString:
          WriteTo(writer, (IMultiLineString)geometry);
					break;
        case OgcGeometryType.MultiPolygon:
          WriteTo(writer, (IMultiPolygon)geometry);
					break;
				default:
					throw new ArcXmlException(String.Format("Could not write {0} object, unsupported.", geometry.GetType().Name));
			}
		}

		public static void WriteTo(ArcXmlWriter writer, ILineString lineString)
		{
			try
			{
				writer.WriteStartElement("POLYLINE");

				WriteLineStringTo(writer, "PATH", lineString);

				writer.WriteEndElement();
			}
			catch (Exception ex)
			{
				if (ex is ArcXmlException)
				{
					throw ex;
				}
				else
				{
					throw new ArcXmlException("Could not write LineString object.", ex);
				}
			}
		}

		public static void WriteTo(ArcXmlWriter writer, IMultiLineString multiLineString)
		{
			try
			{
				writer.WriteStartElement("POLYLINE");

        foreach (ILineString lineString in multiLineString.Geometries.Cast<ILineString>())
				{
          WriteLineStringTo(writer, "PATH", lineString);
				}

				writer.WriteEndElement();
			}
			catch (Exception ex)
			{
				if (ex is ArcXmlException)
				{
					throw ex;
				}
				else
				{
					throw new ArcXmlException("Could not write MultiLineString object.", ex);
				}
			}
		}

		public static void WriteTo(ArcXmlWriter writer, IMultiPoint multiPoint)
		{
			try
			{
        writer.WriteStartElement("MULTIPOINT");

        foreach (IPoint point in multiPoint.Geometries.Cast<IPoint>())
        {
          WriteTo(writer, point);
        }

        writer.WriteEndElement();
      }
			catch (Exception ex)
			{
				if (ex is ArcXmlException)
				{
					throw ex;
				}
				else
				{
					throw new ArcXmlException("Could not write MultiPoint object.", ex);
				}
			}
		}

		public static void WriteTo(ArcXmlWriter writer, IMultiPolygon multiPolygon)
		{
			try
			{
				writer.WriteStartElement("POLYGON");

        foreach (IPolygon polygon in multiPolygon.Geometries.Cast<IPolygon>())
				{
          WriteRingTo(writer, polygon);
				}

				writer.WriteEndElement();
			}
			catch (Exception ex)
			{
				if (ex is ArcXmlException)
				{
					throw ex;
				}
				else
				{
					throw new ArcXmlException("Could not write Polygon object.", ex);
				}
			}
		}

		public static void WriteTo(ArcXmlWriter writer, IPoint point)
		{
			try
			{
        WriteAsPointTo(writer, point.Coordinate);
			}
			catch (Exception ex)
			{
				if (ex is ArcXmlException)
				{
					throw ex;
				}
				else
				{
					throw new ArcXmlException("Could not write Point object.", ex);
				}
			}
		}

		public static void WriteTo(ArcXmlWriter writer, IPolygon polygon)
		{
			try
			{
				writer.WriteStartElement("POLYGON");

        WriteRingTo(writer, polygon);

				writer.WriteEndElement();
			}
			catch (Exception ex)
			{
				if (ex is ArcXmlException)
				{
					throw ex;
				}
				else
				{
					throw new ArcXmlException("Could not write Polygon object.", ex);
				}
			}
		}

		private static void WriteLineStringTo(ArcXmlWriter writer, string arcXmlName, ILineString lineString)
		{
			writer.WriteStartElement(arcXmlName);

      for (int i = 0; i < lineString.Coordinates.Length; ++i)
			{
        WriteAsPointTo(writer, lineString.Coordinates[i]);
			}

			writer.WriteEndElement();
		}

		private static void WriteRingTo(ArcXmlWriter writer, IPolygon polygon)
		{
			writer.WriteStartElement("RING");

			for (int i = 0; i < polygon.ExteriorRing.Coordinates.Length; ++i)
			{
        WriteAsPointTo(writer, polygon.ExteriorRing.Coordinates[i]);
			}

			foreach (ILineString lineString in polygon.InteriorRings)
			{
				WriteLineStringTo(writer, "HOLE", lineString);
			}

			writer.WriteEndElement();
		}
	}
}
