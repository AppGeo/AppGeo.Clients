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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using GeoAPI.Geometries;
using AppGeo.Clients.Transform;

namespace AppGeo.Clients
{
  public class MapGraphics : IDisposable
  {
    // ===== MapGraphics API =====

    private const int DefaultImageQuality = 85;

    public static MapGraphics FromImage(Image image, Envelope extent)
    {
      return new MapGraphics(image, extent);
    }

    public static MapGraphics FromImage(Image image, Transformation transform)
    {
      return new MapGraphics(image, transform);
    }

    public static IntPtr GetHalftonePalette()
    {
      return Graphics.GetHalftonePalette();
    }

    private Image _image = null;
    private Graphics _graphics = null;
    private Transformation _mapTransform = null;

    private MapGraphics(Image image, Envelope extent)
      : this(image, new AffineTransformation(image.Width, image.Height, extent)) { }

    private MapGraphics(Image image, Transformation transform)
    {
      _image = image;
      _graphics = Graphics.FromImage(image);
      _mapTransform = transform;
    }

    ~MapGraphics()
    {
      Dispose();

      _mapTransform = null;
      _graphics = null;
      _image = null;
    }

    public Transformation MapTransform
    {
      get
      {
        return _mapTransform;
      }
    }

    public void DrawCoordinate(Pen pen, Coordinate coordinate)
    {
      PointF p1 = ToPointF(coordinate);
      PointF p2 = new PointF(p1.X, p1.Y + 0.001f);
      _graphics.DrawLine(pen, p1, p2);
    }

    public void DrawEnvelope(Pen pen, Envelope envelope)
    {
      RectangleF rect = ToRectangleF(envelope);
      _graphics.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);
    }

    public void DrawGeometry(Pen pen, IGeometry geometry)
    {
      switch (geometry.OgcGeometryType)
      {
        case OgcGeometryType.Point: DrawGeometry(pen, (IPoint)geometry); break;
        case OgcGeometryType.MultiPoint: DrawGeometry(pen, (IMultiPoint)geometry); break;
        case OgcGeometryType.LineString: DrawGeometry(pen, (ILineString)geometry); break;
        case OgcGeometryType.MultiLineString: DrawGeometry(pen, (IMultiLineString)geometry); break;
        case OgcGeometryType.Polygon: DrawGeometry(pen, (IPolygon)geometry); break;
        case OgcGeometryType.MultiPolygon: DrawGeometry(pen, (IMultiPolygon)geometry); break;
      }
    }

    public void DrawGeometry(Pen pen, IPoint point)
    {
      DrawCoordinate(pen, point.Coordinate);
    }

    public void DrawGeometry(Pen pen, IMultiPoint multiPoint)
    {
      foreach (IPoint point in multiPoint.Geometries.Cast<IPoint>())
      {
        DrawCoordinate(pen, point.Coordinate);
      }
    }

    public void DrawGeometry(Pen pen, ILineString lineString)
    {
      _graphics.DrawLines(pen, ToPointF(lineString.Coordinates));
    }

    public void DrawGeometry(Pen pen, IMultiLineString multiLineString)
    {
      foreach (ILineString lineString in multiLineString.Geometries.Cast<ILineString>())
      {
        DrawGeometry(pen, lineString);
      }
    }

    public void DrawGeometry(Pen pen, IPolygon polygon)
    {
      DrawGeometry(pen, polygon.ExteriorRing);

      foreach (ILineString interiorRing in polygon.InteriorRings)
      {
        DrawGeometry(pen, interiorRing);
      }
    }

    public void DrawGeometry(Pen pen, IMultiPolygon multiPolygon)
    {
      foreach (IPolygon polygon in multiPolygon.Geometries.Cast<IPolygon>())
      {
        DrawGeometry(pen, polygon);
      }
    }

    public void DrawString(String s, Font font, Brush brush, Coordinate coordinate)
    {
      _graphics.DrawString(s, font, brush, ToPointF(coordinate));
    }

    public void DrawString(String s, Font font, Brush brush, Coordinate coordinate, StringFormat format)
    {
      _graphics.DrawString(s, font, brush, ToPointF(coordinate), format);
    }

    public void DrawString(String s, Font font, Brush brush, Coordinate coordinate, float xOffset, float yOffset)
    {
      PointF p = ToPointF(coordinate);
      p.X += xOffset;
      p.Y += yOffset;

      _graphics.DrawString(s, font, brush, p);
    }

    public void DrawString(String s, Font font, Brush brush, Coordinate coordinate, float xOffset, float yOffset, StringFormat format)
    {
      PointF p = ToPointF(coordinate);
      p.X += xOffset;
      p.Y += yOffset;

      _graphics.DrawString(s, font, brush, p, format);
    }

    public void DrawString(String s, Font font, Brush brush, Envelope envelope)
    {
      _graphics.DrawString(s, font, brush, ToRectangleF(envelope));
    }

    public void DrawString(String s, Font font, Brush brush, Envelope envelope, StringFormat format)
    {
      _graphics.DrawString(s, font, brush, ToRectangleF(envelope), format);
    }

    public void FillEnvelope(Brush brush, Envelope envelope)
    {
      _graphics.FillRectangle(brush, ToRectangleF(envelope));
    }

    public void FillGeometry(Brush brush, IGeometry geometry)
    {
      switch (geometry.OgcGeometryType)
      {
        case OgcGeometryType.Polygon: FillGeometry(brush, (IPolygon)geometry); break;
        case OgcGeometryType.MultiPolygon: FillGeometry(brush, (IMultiPolygon)geometry); break;
      }
    }

    public void FillGeometry(Brush brush, IPolygon polygon)
    {
      List<Coordinate> coordinates = new List<Coordinate>(polygon.ExteriorRing.Coordinates);

      foreach (ILineString interiorRing in polygon.InteriorRings)
      {
        coordinates.AddRange(interiorRing.Coordinates);
      }

      PointF[] points = ToPointF(coordinates);
      _graphics.FillPolygon(brush, points);
    }

    public void FillGeometry(Brush brush, IMultiPolygon multiPolygon)
    {
      foreach (IPolygon polygon in multiPolygon.Geometries.Cast<IPolygon>())
      {
        FillGeometry(brush, polygon);
      }
    }

    private ImageCodecInfo GetEncoderInfo(string mimeType)
    {
      ImageCodecInfo[] encoders = ImageCodecInfo.GetImageEncoders();

      for (int i = 0; i < encoders.Length; ++i)
      {
        if (encoders[i].MimeType == mimeType)
        {
          return encoders[i];
        }
      }

      return null;
    }

    public byte[] GetImageBytes()
    {
      MemoryStream memoryStream = new MemoryStream();
      WriteImage(memoryStream);
      return memoryStream.ToArray();
    }

    public byte[] GetImageBytes(CommonImageType imageType)
    {
      MemoryStream memoryStream = new MemoryStream();
      WriteImage(memoryStream, imageType);
      return memoryStream.ToArray();
    }

    public byte[] GetImageBytes(CommonImageType imageType, int quality)
    {
      MemoryStream memoryStream = new MemoryStream();
      WriteImage(memoryStream, imageType, quality);
      return memoryStream.ToArray();
    }

    private PointF ToPointF(Coordinate c)
    {
      Coordinate p = _mapTransform.ReverseTransform(c);
      return new System.Drawing.PointF(Convert.ToSingle(p.X), Convert.ToSingle(p.Y));
    }

    private PointF[] ToPointF(IEnumerable<Coordinate> coordinates)
    {
      return coordinates.Select(o => ToPointF(o)).ToArray();
    }

    private RectangleF ToRectangleF(Envelope envelope)
    {
      PointF p1 = ToPointF(new Coordinate(envelope.MinX, envelope.MaxY));
      PointF p2 = ToPointF(new Coordinate(envelope.MaxX, envelope.MinY));

      float width = p2.X - p1.X;
      float height = p2.Y - p1.Y;

      return new RectangleF(p1.X, p1.Y, width, height);
    }

    public void WriteImage(Stream stream)
    {
      WriteImage(stream, _image.RawFormat, DefaultImageQuality);
    }

    public void WriteImage(Stream stream, CommonImageType imageType)
    {
      WriteImage(stream, imageType, DefaultImageQuality);
    }

    public void WriteImage(Stream stream, CommonImageType imageType, int quality)
    {
      ImageFormat format = _image.RawFormat;

      switch (imageType)
      {
        case CommonImageType.Jpg: format = ImageFormat.Jpeg; break;
        case CommonImageType.Png: format = ImageFormat.Png; break;
      }

      WriteImage(stream, format, quality);
    }

    private void WriteImage(Stream stream, ImageFormat format, int quality)
    {
      if (format.Guid == ImageFormat.Jpeg.Guid)
      {
        ImageCodecInfo imageCodecInfo = GetEncoderInfo("image/jpeg");
        EncoderParameters encoderParameters = new EncoderParameters(1);
        encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, quality);
        _image.Save(stream, imageCodecInfo, encoderParameters);
      }
      else
      {
        _image.Save(stream, format);
      }
    }

    // ===== Wrapped Graphics API =====

    // Note: static FromN methods on Graphics are not exposed

    public Region Clip
    {
      get
      {
        return _graphics.Clip;
      }
      set
      {
        _graphics.Clip = value;
      }
    }

    public RectangleF ClipBounds
    {
      get
      {
        return _graphics.ClipBounds;
      }
    }

    public CompositingMode CompositingMode
    {
      get
      {
        return _graphics.CompositingMode;
      }
      set
      {
        _graphics.CompositingMode = value;
      }
    }

    public CompositingQuality CompositingQuality
    {
      get
      {
        return _graphics.CompositingQuality;
      }
      set
      {
        _graphics.CompositingQuality = value;
      }
    }

    public float DpiX
    {
      get
      {
        return _graphics.DpiX;
      }
    }

    public float DpiY
    {
      get
      {
        return _graphics.DpiY;
      }
    }

    public InterpolationMode InterpolationMode
    {
      get
      {
        return _graphics.InterpolationMode;
      }
      set
      {
        _graphics.InterpolationMode = value;
      }
    }

    public bool IsClipEmpty
    {
      get
      {
        return _graphics.IsClipEmpty;
      }
    }

    public bool IsVisibleClipEmpty
    {
      get
      {
        return _graphics.IsVisibleClipEmpty;
      }
    }

    public float PageScale
    {
      get
      {
        return _graphics.PageScale;
      }
      set
      {
        _graphics.PageScale = value;
      }
    }

    public GraphicsUnit PageUnit
    {
      get
      {
        return _graphics.PageUnit;
      }
      set
      {
        _graphics.PageUnit = value;
      }
    }

    public PixelOffsetMode PixelOffsetMode
    {
      get
      {
        return _graphics.PixelOffsetMode;
      }
      set
      {
        _graphics.PixelOffsetMode = value;
      }
    }

    public System.Drawing.Point RenderingOrigin
    {
      get
      {
        return _graphics.RenderingOrigin;
      }
      set
      {
        _graphics.RenderingOrigin = value;
      }
    }

    public SmoothingMode SmoothingMode
    {
      get
      {
        return _graphics.SmoothingMode;
      }
      set
      {
        _graphics.SmoothingMode = value;
      }
    }

    public int TextContrast
    {
      get
      {
        return _graphics.TextContrast;
      }
      set
      {
        _graphics.TextContrast = value;
      }
    }

    public TextRenderingHint TextRenderingHint
    {
      get
      {
        return _graphics.TextRenderingHint;
      }
      set
      {
        _graphics.TextRenderingHint = value;
      }
    }

    public Matrix Transform
    {
      get
      {
        return _graphics.Transform;
      }
      set
      {
        _graphics.Transform = value;
      }
    }

    public RectangleF VisibleClipBounds
    {
      get
      {
        return _graphics.VisibleClipBounds;
      }
    }

    public void AddMetafileComment(byte[] data)
    {
      _graphics.AddMetafileComment(data);
    }

    public GraphicsContainer BeginContainer()
    {
      return _graphics.BeginContainer();
    }

    public GraphicsContainer BeginContainer(RectangleF dstrect, RectangleF srcrect, GraphicsUnit unit)
    {
      return _graphics.BeginContainer(dstrect, srcrect, unit);
    }

    public GraphicsContainer BeginContainer(Rectangle dstrect, Rectangle srcrect, GraphicsUnit unit)
    {
      return _graphics.BeginContainer(dstrect, srcrect, unit);
    }

    public void Clear(Color color)
    {
      _graphics.Clear(color);
    }

    public void CopyFromScreen(System.Drawing.Point upperLeftSource, System.Drawing.Point upperLeftDestination, Size blockRegionSize)
    {
      _graphics.CopyFromScreen(upperLeftSource, upperLeftDestination, blockRegionSize);
    }

    public void CopyFromScreen(int sourceX, int sourceY, int destinationX, int destinationY, Size blockRegionSize, CopyPixelOperation copyPixelOperation)
    {
      _graphics.CopyFromScreen(sourceX, sourceY, destinationX, destinationY, blockRegionSize, copyPixelOperation);
    }

    public void CopyFromScreen(int sourceX, int sourceY, int destinationX, int destinationY, Size blockRegionSize)
    {
      _graphics.CopyFromScreen(sourceX, sourceY, destinationX, destinationY, blockRegionSize);
    }

    public void CopyFromScreen(System.Drawing.Point upperLeftSource, System.Drawing.Point upperLeftDestination, Size blockRegionSize, CopyPixelOperation copyPixelOperation)
    {
      _graphics.CopyFromScreen(upperLeftSource, upperLeftDestination, blockRegionSize, copyPixelOperation);
    }

    public System.Runtime.Remoting.ObjRef CreateObjRef(Type requestedType)
    {
      return _graphics.CreateObjRef(requestedType);
    }

    public void Dispose()
    {
      _graphics.Dispose();
    }

    public void DrawArc(Pen pen, Rectangle rect, float startAngle, float sweepAngle)
    {
      _graphics.DrawArc(pen, rect, startAngle, sweepAngle);
    }

    public void DrawArc(Pen pen, int x, int y, int width, int height, int startAngle, int sweepAngle)
    {
      _graphics.DrawArc(pen, x, y, width, height, startAngle, sweepAngle);
    }

    public void DrawArc(Pen pen, float x, float y, float width, float height, float startAngle, float sweepAngle)
    {
      _graphics.DrawArc(pen, x, y, width, height, startAngle, sweepAngle);
    }

    public void DrawArc(Pen pen, RectangleF rect, float startAngle, float sweepAngle)
    {
      _graphics.DrawArc(pen, rect, startAngle, sweepAngle);
    }

    public void DrawBezier(Pen pen, PointF pt1, PointF pt2, PointF pt3, PointF pt4)
    {
      _graphics.DrawBezier(pen, pt1, pt2, pt3, pt4);
    }

    public void DrawBezier(Pen pen, System.Drawing.Point pt1, System.Drawing.Point pt2, System.Drawing.Point pt3, System.Drawing.Point pt4)
    {
      _graphics.DrawBezier(pen, pt1, pt2, pt3, pt4);
    }

    public void DrawBezier(Pen pen, float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4)
    {
      _graphics.DrawBezier(pen, x1, y1, x2, y2, x3, y3, x4, y4);
    }

    public void DrawBeziers(Pen pen, PointF[] points)
    {
      _graphics.DrawBeziers(pen, points);
    }

    public void DrawBeziers(Pen pen, System.Drawing.Point[] points)
    {
      _graphics.DrawBeziers(pen, points);
    }

    public void DrawClosedCurve(Pen pen, PointF[] points)
    {
      _graphics.DrawClosedCurve(pen, points);
    }

    public void DrawClosedCurve(Pen pen, PointF[] points, float tension, FillMode fillmode)
    {
      _graphics.DrawClosedCurve(pen, points, tension, fillmode);
    }

    public void DrawClosedCurve(Pen pen, System.Drawing.Point[] points, float tension, FillMode fillmode)
    {
      _graphics.DrawClosedCurve(pen, points, tension, fillmode);
    }

    public void DrawClosedCurve(Pen pen, System.Drawing.Point[] points)
    {
      _graphics.DrawClosedCurve(pen, points);
    }

    public void DrawCurve(Pen pen, PointF[] points, int offset, int numberOfSegments, float tension)
    {
      _graphics.DrawCurve(pen, points, offset, numberOfSegments, tension);
    }

    public void DrawCurve(Pen pen, PointF[] points, float tension)
    {
      _graphics.DrawCurve(pen, points, tension);
    }

    public void DrawCurve(Pen pen, System.Drawing.Point[] points, int offset, int numberOfSegments, float tension)
    {
      _graphics.DrawCurve(pen, points, offset, numberOfSegments, tension);
    }

    public void DrawCurve(Pen pen, System.Drawing.Point[] points)
    {
      _graphics.DrawCurve(pen, points);
    }

    public void DrawCurve(Pen pen, System.Drawing.Point[] points, float tension)
    {
      _graphics.DrawCurve(pen, points, tension);
    }

    public void DrawCurve(Pen pen, PointF[] points)
    {
      _graphics.DrawCurve(pen, points);
    }

    public void DrawCurve(Pen pen, PointF[] points, int offset, int numberOfSegments)
    {
      _graphics.DrawCurve(pen, points, offset, numberOfSegments);
    }

    public void DrawEllipse(Pen pen, float x, float y, float width, float height)
    {
      _graphics.DrawEllipse(pen, x, y, width, height);
    }

    public void DrawEllipse(Pen pen, RectangleF rect)
    {
      _graphics.DrawEllipse(pen, rect);
    }

    public void DrawEllipse(Pen pen, Rectangle rect)
    {
      _graphics.DrawEllipse(pen, rect);
    }

    public void DrawEllipse(Pen pen, int x, int y, int width, int height)
    {
      _graphics.DrawEllipse(pen, x, y, width, height);
    }

    public void DrawIcon(Icon icon, Rectangle targetRect)
    {
      _graphics.DrawIcon(icon, targetRect);
    }

    public void DrawIcon(Icon icon, int x, int y)
    {
      _graphics.DrawIcon(icon, x, y);
    }

    public void DrawIconUnstretched(Icon icon, Rectangle targetRect)
    {
      _graphics.DrawIconUnstretched(icon, targetRect);
    }

    public void DrawImage(Image image, Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttr)
    {
      _graphics.DrawImage(image, destRect, srcX, srcY, srcWidth, srcHeight, srcUnit, imageAttr);
    }

    public void DrawImage(Image image, PointF[] destPoints, RectangleF srcRect, GraphicsUnit srcUnit, ImageAttributes imageAttr, Graphics.DrawImageAbort callback, int callbackData)
    {
      _graphics.DrawImage(image, destPoints, srcRect, srcUnit, imageAttr, callback, callbackData);
    }

    public void DrawImage(Image image, System.Drawing.Point[] destPoints, Rectangle srcRect, GraphicsUnit srcUnit)
    {
      _graphics.DrawImage(image, destPoints, srcRect, srcUnit);
    }

    public void DrawImage(Image image, Rectangle destRect, float srcX, float srcY, float srcWidth, float srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttrs, Graphics.DrawImageAbort callback, IntPtr callbackData)
    {
      _graphics.DrawImage(image, destRect, srcX, srcY, srcWidth, srcHeight, srcUnit, imageAttrs, callback, callbackData);
    }

    public void DrawImage(Image image, Rectangle destRect, float srcX, float srcY, float srcWidth, float srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttrs, Graphics.DrawImageAbort callback)
    {
      _graphics.DrawImage(image, destRect, srcX, srcY, srcWidth, srcHeight, srcUnit, imageAttrs, callback);
    }

    public void DrawImage(Image image, int x, int y, Rectangle srcRect, GraphicsUnit srcUnit)
    {
      _graphics.DrawImage(image, x, y, srcRect, srcUnit);
    }

    public void DrawImage(Image image, Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight, GraphicsUnit srcUnit)
    {
      _graphics.DrawImage(image, destRect, srcX, srcY, srcWidth, srcHeight, srcUnit);
    }

    public void DrawImage(Image image, Rectangle destRect, float srcX, float srcY, float srcWidth, float srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttrs)
    {
      _graphics.DrawImage(image, destRect, srcX, srcY, srcWidth, srcHeight, srcUnit, imageAttrs);
    }

    public void DrawImage(Image image, System.Drawing.Point[] destPoints, Rectangle srcRect, GraphicsUnit srcUnit, ImageAttributes imageAttr, Graphics.DrawImageAbort callback)
    {
      _graphics.DrawImage(image, destPoints, srcRect, srcUnit, imageAttr, callback);
    }

    public void DrawImage(Image image, System.Drawing.Point[] destPoints, Rectangle srcRect, GraphicsUnit srcUnit, ImageAttributes imageAttr)
    {
      _graphics.DrawImage(image, destPoints, srcRect, srcUnit, imageAttr);
    }

    public void DrawImage(Image image, Rectangle destRect, float srcX, float srcY, float srcWidth, float srcHeight, GraphicsUnit srcUnit)
    {
      _graphics.DrawImage(image, destRect, srcX, srcY, srcWidth, srcHeight, srcUnit);
    }

    public void DrawImage(Image image, System.Drawing.Point[] destPoints, Rectangle srcRect, GraphicsUnit srcUnit, ImageAttributes imageAttr, Graphics.DrawImageAbort callback, int callbackData)
    {
      _graphics.DrawImage(image, destPoints, srcRect, srcUnit, imageAttr, callback, callbackData);
    }

    public void DrawImage(Image image, Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttr, Graphics.DrawImageAbort callback)
    {
      _graphics.DrawImage(image, destRect, srcX, srcY, srcWidth, srcHeight, srcUnit, imageAttr, callback);
    }

    public void DrawImage(Image image, System.Drawing.Point point)
    {
      _graphics.DrawImage(image, point);
    }

    public void DrawImage(Image image, int x, int y)
    {
      _graphics.DrawImage(image, x, y);
    }

    public void DrawImage(Image image, float x, float y, float width, float height)
    {
      _graphics.DrawImage(image, x, y, width, height);
    }

    public void DrawImage(Image image, RectangleF destRect, RectangleF srcRect, GraphicsUnit srcUnit)
    {
      _graphics.DrawImage(image, destRect, srcRect, srcUnit);
    }

    public void DrawImage(Image image, RectangleF rect)
    {
      _graphics.DrawImage(image, rect);
    }

    public void DrawImage(Image image, System.Drawing.Point[] destPoints)
    {
      _graphics.DrawImage(image, destPoints);
    }

    public void DrawImage(Image image, float x, float y, RectangleF srcRect, GraphicsUnit srcUnit)
    {
      _graphics.DrawImage(image, x, y, srcRect, srcUnit);
    }

    public void DrawImage(Image image, PointF[] destPoints)
    {
      _graphics.DrawImage(image, destPoints);
    }

    public void DrawImage(Image image, Rectangle rect)
    {
      _graphics.DrawImage(image, rect);
    }

    public void DrawImage(Image image, int x, int y, int width, int height)
    {
      _graphics.DrawImage(image, x, y, width, height);
    }

    public void DrawImage(Image image, Rectangle destRect, Rectangle srcRect, GraphicsUnit srcUnit)
    {
      _graphics.DrawImage(image, destRect, srcRect, srcUnit);
    }

    public void DrawImage(Image image, Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttrs, Graphics.DrawImageAbort callback, IntPtr callbackData)
    {
      _graphics.DrawImage(image, destRect, srcX, srcY, srcWidth, srcHeight, srcUnit, imageAttrs, callback, callbackData);
    }

    public void DrawImage(Image image, float x, float y)
    {
      _graphics.DrawImage(image, x, y);
    }

    public void DrawImage(Image image, PointF[] destPoints, RectangleF srcRect, GraphicsUnit srcUnit, ImageAttributes imageAttr, Graphics.DrawImageAbort callback)
    {
      _graphics.DrawImage(image, destPoints, srcRect, srcUnit, imageAttr, callback);
    }

    public void DrawImage(Image image, PointF point)
    {
      _graphics.DrawImage(image, point);
    }

    public void DrawImage(Image image, PointF[] destPoints, RectangleF srcRect, GraphicsUnit srcUnit, ImageAttributes imageAttr)
    {
      _graphics.DrawImage(image, destPoints, srcRect, srcUnit, imageAttr);
    }

    public void DrawImage(Image image, PointF[] destPoints, RectangleF srcRect, GraphicsUnit srcUnit)
    {
      _graphics.DrawImage(image, destPoints, srcRect, srcUnit);
    }

    public void DrawImageUnscaled(Image image, System.Drawing.Point point)
    {
      _graphics.DrawImageUnscaled(image, point);
    }

    public void DrawImageUnscaled(Image image, int x, int y, int width, int height)
    {
      _graphics.DrawImageUnscaled(image, x, y, width, height);
    }

    public void DrawImageUnscaled(Image image, int x, int y)
    {
      _graphics.DrawImageUnscaled(image, x, y);
    }

    public void DrawImageUnscaled(Image image, Rectangle rect)
    {
      _graphics.DrawImageUnscaled(image, rect);
    }

    public void DrawImageUnscaledAndClipped(Image image, Rectangle rect)
    {
      _graphics.DrawImageUnscaledAndClipped(image, rect);
    }

    public void DrawLine(Pen pen, PointF pt1, PointF pt2)
    {
      _graphics.DrawLine(pen, pt1, pt2);
    }

    public void DrawLine(Pen pen, System.Drawing.Point pt1, System.Drawing.Point pt2)
    {
      _graphics.DrawLine(pen, pt1, pt2);
    }

    public void DrawLine(Pen pen, float x1, float y1, float x2, float y2)
    {
      _graphics.DrawLine(pen, x1, y1, x2, y2);
    }

    public void DrawLine(Pen pen, int x1, int y1, int x2, int y2)
    {
      _graphics.DrawLine(pen, x1, y1, x2, y2);
    }

    public void DrawLines(Pen pen, PointF[] points)
    {
      _graphics.DrawLines(pen, points);
    }

    public void DrawLines(Pen pen, System.Drawing.Point[] points)
    {
      _graphics.DrawLines(pen, points);
    }

    public void DrawPath(Pen pen, GraphicsPath path)
    {
      _graphics.DrawPath(pen, path);
    }

    public void DrawPie(Pen pen, int x, int y, int width, int height, int startAngle, int sweepAngle)
    {
      _graphics.DrawPie(pen, x, y, width, height, startAngle, sweepAngle);
    }

    public void DrawPie(Pen pen, Rectangle rect, float startAngle, float sweepAngle)
    {
      _graphics.DrawPie(pen, rect, startAngle, sweepAngle);
    }

    public void DrawPie(Pen pen, float x, float y, float width, float height, float startAngle, float sweepAngle)
    {
      _graphics.DrawPie(pen, x, y, width, height, startAngle, sweepAngle);
    }

    public void DrawPie(Pen pen, RectangleF rect, float startAngle, float sweepAngle)
    {
      _graphics.DrawPie(pen, rect, startAngle, sweepAngle);
    }

    public void DrawPolygon(Pen pen, System.Drawing.Point[] points)
    {
      _graphics.DrawPolygon(pen, points);
    }

    public void DrawPolygon(Pen pen, PointF[] points)
    {
      _graphics.DrawPolygon(pen, points);
    }

    public void DrawRectangle(Pen pen, Rectangle rect)
    {
      _graphics.DrawRectangle(pen, rect);
    }

    public void DrawRectangle(Pen pen, float x, float y, float width, float height)
    {
      _graphics.DrawRectangle(pen, x, y, width, height);
    }

    public void DrawRectangle(Pen pen, int x, int y, int width, int height)
    {
      _graphics.DrawRectangle(pen, x, y, width, height);
    }

    public void DrawRectangles(Pen pen, RectangleF[] rects)
    {
      _graphics.DrawRectangles(pen, rects);
    }

    public void DrawRectangles(Pen pen, Rectangle[] rects)
    {
      _graphics.DrawRectangles(pen, rects);
    }

    public void DrawString(String s, Font font, Brush brush, PointF point, StringFormat format)
    {
      _graphics.DrawString(s, font, brush, point, format);
    }

    public void DrawString(String s, Font font, Brush brush, RectangleF layoutRectangle)
    {
      _graphics.DrawString(s, font, brush, layoutRectangle);
    }

    public void DrawString(String s, Font font, Brush brush, RectangleF layoutRectangle, StringFormat format)
    {
      _graphics.DrawString(s, font, brush, layoutRectangle, format);
    }

    public void DrawString(String s, Font font, Brush brush, float x, float y)
    {
      _graphics.DrawString(s, font, brush, x, y);
    }

    public void DrawString(String s, Font font, Brush brush, PointF point)
    {
      _graphics.DrawString(s, font, brush, point);
    }

    public void DrawString(String s, Font font, Brush brush, float x, float y, StringFormat format)
    {
      _graphics.DrawString(s, font, brush, x, y, format);
    }

    public void EndContainer(GraphicsContainer container)
    {
      _graphics.EndContainer(container);
    }

    public void EnumerateMetafile(Metafile metafile, RectangleF destRect, RectangleF srcRect, GraphicsUnit srcUnit, Graphics.EnumerateMetafileProc callback, IntPtr callbackData)
    {
      _graphics.EnumerateMetafile(metafile, destRect, srcRect, srcUnit, callback, callbackData);
    }

    public void EnumerateMetafile(Metafile metafile, Rectangle destRect, Rectangle srcRect, GraphicsUnit srcUnit, Graphics.EnumerateMetafileProc callback)
    {
      _graphics.EnumerateMetafile(metafile, destRect, srcRect, srcUnit, callback);
    }

    public void EnumerateMetafile(Metafile metafile, RectangleF destRect, RectangleF srcRect, GraphicsUnit unit, Graphics.EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
    {
      _graphics.EnumerateMetafile(metafile, destRect, srcRect, unit, callback, callbackData, imageAttr);
    }

    public void EnumerateMetafile(Metafile metafile, RectangleF destRect, RectangleF srcRect, GraphicsUnit srcUnit, Graphics.EnumerateMetafileProc callback)
    {
      _graphics.EnumerateMetafile(metafile, destRect, srcRect, srcUnit, callback);
    }

    public void EnumerateMetafile(Metafile metafile, System.Drawing.Point destPoint, Rectangle srcRect, GraphicsUnit srcUnit, Graphics.EnumerateMetafileProc callback)
    {
      _graphics.EnumerateMetafile(metafile, destPoint, srcRect, srcUnit, callback);
    }

    public void EnumerateMetafile(Metafile metafile, PointF destPoint, RectangleF srcRect, GraphicsUnit unit, Graphics.EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
    {
      _graphics.EnumerateMetafile(metafile, destPoint, srcRect, unit, callback, callbackData, imageAttr);
    }

    public void EnumerateMetafile(Metafile metafile, System.Drawing.Point destPoint, Rectangle srcRect, GraphicsUnit unit, Graphics.EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
    {
      _graphics.EnumerateMetafile(metafile, destPoint, srcRect, unit, callback, callbackData, imageAttr);
    }

    public void EnumerateMetafile(Metafile metafile, System.Drawing.Point destPoint, Rectangle srcRect, GraphicsUnit srcUnit, Graphics.EnumerateMetafileProc callback, IntPtr callbackData)
    {
      _graphics.EnumerateMetafile(metafile, destPoint, srcRect, srcUnit, callback, callbackData);
    }

    public void EnumerateMetafile(Metafile metafile, Rectangle destRect, Rectangle srcRect, GraphicsUnit srcUnit, Graphics.EnumerateMetafileProc callback, IntPtr callbackData)
    {
      _graphics.EnumerateMetafile(metafile, destRect, srcRect, srcUnit, callback, callbackData);
    }

    public void EnumerateMetafile(Metafile metafile, System.Drawing.Point[] destPoints, Rectangle srcRect, GraphicsUnit srcUnit, Graphics.EnumerateMetafileProc callback)
    {
      _graphics.EnumerateMetafile(metafile, destPoints, srcRect, srcUnit, callback);
    }

    public void EnumerateMetafile(Metafile metafile, PointF[] destPoints, RectangleF srcRect, GraphicsUnit unit, Graphics.EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
    {
      _graphics.EnumerateMetafile(metafile, destPoints, srcRect, unit, callback, callbackData, imageAttr);
    }

    public void EnumerateMetafile(Metafile metafile, System.Drawing.Point[] destPoints, Rectangle srcRect, GraphicsUnit unit, Graphics.EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
    {
      _graphics.EnumerateMetafile(metafile, destPoints, srcRect, unit, callback, callbackData, imageAttr);
    }

    public void EnumerateMetafile(Metafile metafile, System.Drawing.Point[] destPoints, Rectangle srcRect, GraphicsUnit srcUnit, Graphics.EnumerateMetafileProc callback, IntPtr callbackData)
    {
      _graphics.EnumerateMetafile(metafile, destPoints, srcRect, srcUnit, callback, callbackData);
    }

    public void EnumerateMetafile(Metafile metafile, PointF[] destPoints, RectangleF srcRect, GraphicsUnit srcUnit, Graphics.EnumerateMetafileProc callback)
    {
      _graphics.EnumerateMetafile(metafile, destPoints, srcRect, srcUnit, callback);
    }

    public void EnumerateMetafile(Metafile metafile, Rectangle destRect, Rectangle srcRect, GraphicsUnit unit, Graphics.EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
    {
      _graphics.EnumerateMetafile(metafile, destRect, srcRect, unit, callback, callbackData, imageAttr);
    }

    public void EnumerateMetafile(Metafile metafile, PointF[] destPoints, RectangleF srcRect, GraphicsUnit srcUnit, Graphics.EnumerateMetafileProc callback, IntPtr callbackData)
    {
      _graphics.EnumerateMetafile(metafile, destPoints, srcRect, srcUnit, callback, callbackData);
    }

    public void EnumerateMetafile(Metafile metafile, PointF[] destPoints, Graphics.EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
    {
      _graphics.EnumerateMetafile(metafile, destPoints, callback, callbackData, imageAttr);
    }

    public void EnumerateMetafile(Metafile metafile, RectangleF destRect, Graphics.EnumerateMetafileProc callback, IntPtr callbackData)
    {
      _graphics.EnumerateMetafile(metafile, destRect, callback, callbackData);
    }

    public void EnumerateMetafile(Metafile metafile, RectangleF destRect, Graphics.EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
    {
      _graphics.EnumerateMetafile(metafile, destRect, callback, callbackData, imageAttr);
    }

    public void EnumerateMetafile(Metafile metafile, System.Drawing.Point destPoint, Graphics.EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
    {
      _graphics.EnumerateMetafile(metafile, destPoint, callback, callbackData, imageAttr);
    }

    public void EnumerateMetafile(Metafile metafile, RectangleF destRect, Graphics.EnumerateMetafileProc callback)
    {
      _graphics.EnumerateMetafile(metafile, destRect, callback);
    }

    public void EnumerateMetafile(Metafile metafile, Rectangle destRect, Graphics.EnumerateMetafileProc callback)
    {
      _graphics.EnumerateMetafile(metafile, destRect, callback);
    }

    public void EnumerateMetafile(Metafile metafile, PointF[] destPoints, Graphics.EnumerateMetafileProc callback)
    {
      _graphics.EnumerateMetafile(metafile, destPoints, callback);
    }

    public void EnumerateMetafile(Metafile metafile, PointF[] destPoints, Graphics.EnumerateMetafileProc callback, IntPtr callbackData)
    {
      _graphics.EnumerateMetafile(metafile, destPoints, callback, callbackData);
    }

    public void EnumerateMetafile(Metafile metafile, Rectangle destRect, Graphics.EnumerateMetafileProc callback, IntPtr callbackData)
    {
      _graphics.EnumerateMetafile(metafile, destRect, callback, callbackData);
    }

    public void EnumerateMetafile(Metafile metafile, Rectangle destRect, Graphics.EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
    {
      _graphics.EnumerateMetafile(metafile, destRect, callback, callbackData, imageAttr);
    }

    public void EnumerateMetafile(Metafile metafile, System.Drawing.Point[] destPoints, Graphics.EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
    {
      _graphics.EnumerateMetafile(metafile, destPoints, callback, callbackData, imageAttr);
    }

    public void EnumerateMetafile(Metafile metafile, System.Drawing.Point[] destPoints, Graphics.EnumerateMetafileProc callback, IntPtr callbackData)
    {
      _graphics.EnumerateMetafile(metafile, destPoints, callback, callbackData);
    }

    public void EnumerateMetafile(Metafile metafile, PointF destPoint, RectangleF srcRect, GraphicsUnit srcUnit, Graphics.EnumerateMetafileProc callback, IntPtr callbackData)
    {
      _graphics.EnumerateMetafile(metafile, destPoint, srcRect, srcUnit, callback, callbackData);
    }

    public void EnumerateMetafile(Metafile metafile, PointF destPoint, RectangleF srcRect, GraphicsUnit srcUnit, Graphics.EnumerateMetafileProc callback)
    {
      _graphics.EnumerateMetafile(metafile, destPoint, srcRect, srcUnit, callback);
    }

    public void EnumerateMetafile(Metafile metafile, PointF destPoint, Graphics.EnumerateMetafileProc callback)
    {
      _graphics.EnumerateMetafile(metafile, destPoint, callback);
    }

    public void EnumerateMetafile(Metafile metafile, System.Drawing.Point destPoint, Graphics.EnumerateMetafileProc callback)
    {
      _graphics.EnumerateMetafile(metafile, destPoint, callback);
    }

    public void EnumerateMetafile(Metafile metafile, System.Drawing.Point destPoint, Graphics.EnumerateMetafileProc callback, IntPtr callbackData)
    {
      _graphics.EnumerateMetafile(metafile, destPoint, callback, callbackData);
    }

    public void EnumerateMetafile(Metafile metafile, System.Drawing.Point[] destPoints, Graphics.EnumerateMetafileProc callback)
    {
      _graphics.EnumerateMetafile(metafile, destPoints, callback);
    }

    public void EnumerateMetafile(Metafile metafile, PointF destPoint, Graphics.EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
    {
      _graphics.EnumerateMetafile(metafile, destPoint, callback, callbackData, imageAttr);
    }

    public void EnumerateMetafile(Metafile metafile, PointF destPoint, Graphics.EnumerateMetafileProc callback, IntPtr callbackData)
    {
      _graphics.EnumerateMetafile(metafile, destPoint, callback, callbackData);
    }

    public override bool Equals(object obj)
    {
      return _graphics.Equals(obj);
    }

    public void ExcludeClip(Rectangle rect)
    {
      _graphics.ExcludeClip(rect);
    }

    public void ExcludeClip(Region region)
    {
      _graphics.ExcludeClip(region);
    }

    public void FillClosedCurve(Brush brush, PointF[] points, FillMode fillmode, float tension)
    {
      _graphics.FillClosedCurve(brush, points, fillmode, tension);
    }

    public void FillClosedCurve(Brush brush, PointF[] points, FillMode fillmode)
    {
      _graphics.FillClosedCurve(brush, points, fillmode);
    }

    public void FillClosedCurve(Brush brush, System.Drawing.Point[] points, FillMode fillmode)
    {
      _graphics.FillClosedCurve(brush, points, fillmode);
    }

    public void FillClosedCurve(Brush brush, System.Drawing.Point[] points)
    {
      _graphics.FillClosedCurve(brush, points);
    }

    public void FillClosedCurve(Brush brush, System.Drawing.Point[] points, FillMode fillmode, float tension)
    {
      _graphics.FillClosedCurve(brush, points, fillmode, tension);
    }

    public void FillClosedCurve(Brush brush, PointF[] points)
    {
      _graphics.FillClosedCurve(brush, points);
    }

    public void FillEllipse(Brush brush, int x, int y, int width, int height)
    {
      _graphics.FillEllipse(brush, x, y, width, height);
    }

    public void FillEllipse(Brush brush, RectangleF rect)
    {
      _graphics.FillEllipse(brush, rect);
    }

    public void FillEllipse(Brush brush, float x, float y, float width, float height)
    {
      _graphics.FillEllipse(brush, x, y, width, height);
    }

    public void FillEllipse(Brush brush, Rectangle rect)
    {
      _graphics.FillEllipse(brush, rect);
    }

    public void FillPath(Brush brush, GraphicsPath path)
    {
      _graphics.FillPath(brush, path);
    }

    public void FillPie(Brush brush, Rectangle rect, float startAngle, float sweepAngle)
    {
      _graphics.FillPie(brush, rect, startAngle, sweepAngle);
    }

    public void FillPie(Brush brush, float x, float y, float width, float height, float startAngle, float sweepAngle)
    {
      _graphics.FillPie(brush, x, y, width, height, startAngle, sweepAngle);
    }

    public void FillPie(Brush brush, int x, int y, int width, int height, int startAngle, int sweepAngle)
    {
      _graphics.FillPie(brush, x, y, width, height, startAngle, sweepAngle);
    }

    public void FillPolygon(Brush brush, System.Drawing.Point[] points, FillMode fillMode)
    {
      _graphics.FillPolygon(brush, points, fillMode);
    }

    public void FillPolygon(Brush brush, PointF[] points)
    {
      _graphics.FillPolygon(brush, points);
    }

    public void FillPolygon(Brush brush, System.Drawing.Point[] points)
    {
      _graphics.FillPolygon(brush, points);
    }

    public void FillPolygon(Brush brush, PointF[] points, FillMode fillMode)
    {
      _graphics.FillPolygon(brush, points, fillMode);
    }

    public void FillRectangle(Brush brush, Rectangle rect)
    {
      _graphics.FillRectangle(brush, rect);
    }

    public void FillRectangle(Brush brush, int x, int y, int width, int height)
    {
      _graphics.FillRectangle(brush, x, y, width, height);
    }

    public void FillRectangle(Brush brush, RectangleF rect)
    {
      _graphics.FillRectangle(brush, rect);
    }

    public void FillRectangle(Brush brush, float x, float y, float width, float height)
    {
      _graphics.FillRectangle(brush, x, y, width, height);
    }

    public void FillRectangles(Brush brush, RectangleF[] rects)
    {
      _graphics.FillRectangles(brush, rects);
    }

    public void FillRectangles(Brush brush, Rectangle[] rects)
    {
      _graphics.FillRectangles(brush, rects);
    }

    public void FillRegion(Brush brush, Region region)
    {
      _graphics.FillRegion(brush, region);
    }

    public void Flush(FlushIntention intention)
    {
      _graphics.Flush(intention);
    }

    public void Flush()
    {
      _graphics.Flush();
    }

    public object GetContextInfo()
    {
      return _graphics.GetContextInfo();
    }

    public override int GetHashCode()
    {
      return _graphics.GetHashCode();
    }

    public IntPtr GetHdc()
    {
      return _graphics.GetHdc();
    }

    public object GetLifetimeService()
    {
      return _graphics.GetLifetimeService();
    }

    public Color GetNearestColor(Color color)
    {
      return _graphics.GetNearestColor(color);
    }

    public object InitializeLifetimeService()
    {
      return _graphics.InitializeLifetimeService();
    }

    public void IntersectClip(Rectangle rect)
    {
      _graphics.IntersectClip(rect);
    }

    public void IntersectClip(RectangleF rect)
    {
      _graphics.IntersectClip(rect);
    }

    public void IntersectClip(Region region)
    {
      _graphics.IntersectClip(region);
    }

    public bool IsVisible(System.Drawing.Point point)
    {
      return _graphics.IsVisible(point);
    }

    public bool IsVisible(RectangleF rect)
    {
      return _graphics.IsVisible(rect);
    }

    public bool IsVisible(int x, int y)
    {
      return _graphics.IsVisible(x, y);
    }

    public bool IsVisible(float x, float y)
    {
      return _graphics.IsVisible(x, y);
    }

    public bool IsVisible(Rectangle rect)
    {
      return _graphics.IsVisible(rect);
    }

    public bool IsVisible(float x, float y, float width, float height)
    {
      return _graphics.IsVisible(x, y, width, height);
    }

    public bool IsVisible(int x, int y, int width, int height)
    {
      return _graphics.IsVisible(x, y, width, height);
    }

    public bool IsVisible(PointF point)
    {
      return _graphics.IsVisible(point);
    }

    public Region[] MeasureCharacterRanges(String text, Font font, RectangleF layoutRect, StringFormat stringFormat)
    {
      return _graphics.MeasureCharacterRanges(text, font, layoutRect, stringFormat);
    }

    public SizeF MeasureString(String text, Font font, int width, StringFormat format)
    {
      return _graphics.MeasureString(text, font, width, format);
    }

    public SizeF MeasureString(String text, Font font, PointF origin, StringFormat stringFormat)
    {
      return _graphics.MeasureString(text, font, origin, stringFormat);
    }

    public SizeF MeasureString(String text, Font font, int width)
    {
      return _graphics.MeasureString(text, font, width);
    }

    public SizeF MeasureString(String text, Font font, SizeF layoutArea)
    {
      return _graphics.MeasureString(text, font, layoutArea);
    }

    public SizeF MeasureString(String text, Font font, SizeF layoutArea, StringFormat stringFormat, out int charactersFitted, out int linesFilled)
    {
      return _graphics.MeasureString(text, font, layoutArea, stringFormat, out charactersFitted, out linesFilled);
    }

    public SizeF MeasureString(String text, Font font)
    {
      return _graphics.MeasureString(text, font);
    }

    public SizeF MeasureString(String text, Font font, SizeF layoutArea, StringFormat stringFormat)
    {
      return _graphics.MeasureString(text, font, layoutArea, stringFormat);
    }

    public void MultiplyTransform(Matrix matrix)
    {
      _graphics.MultiplyTransform(matrix);
    }

    public void MultiplyTransform(Matrix matrix, MatrixOrder order)
    {
      _graphics.MultiplyTransform(matrix, order);
    }

    public void ReleaseHdc()
    {
      _graphics.ReleaseHdc();
    }

    public void ReleaseHdc(IntPtr hdc)
    {
      _graphics.ReleaseHdc(hdc);
    }

    public void ResetClip()
    {
      _graphics.ResetClip();
    }

    public void ResetTransform()
    {
      _graphics.ResetTransform();
    }

    public void Restore(GraphicsState gstate)
    {
      _graphics.Restore(gstate);
    }

    public void RotateTransform(float angle)
    {
      _graphics.RotateTransform(angle);
    }

    public void RotateTransform(float angle, MatrixOrder order)
    {
      _graphics.RotateTransform(angle, order);
    }

    public GraphicsState Save()
    {
      return _graphics.Save();
    }

    public void ScaleTransform(float sx, float sy)
    {
      _graphics.ScaleTransform(sx, sy);
    }

    public void ScaleTransform(float sx, float sy, MatrixOrder order)
    {
      _graphics.ScaleTransform(sx, sy, order);
    }

    public void SetClip(Rectangle rect, CombineMode combineMode)
    {
      _graphics.SetClip(rect, combineMode);
    }

    public void SetClip(Region region, CombineMode combineMode)
    {
      _graphics.SetClip(region, combineMode);
    }

    public void SetClip(GraphicsPath path, CombineMode combineMode)
    {
      _graphics.SetClip(path, combineMode);
    }

    public void SetClip(GraphicsPath path)
    {
      _graphics.SetClip(path);
    }

    public void SetClip(RectangleF rect, CombineMode combineMode)
    {
      _graphics.SetClip(rect, combineMode);
    }

    public void SetClip(Rectangle rect)
    {
      _graphics.SetClip(rect);
    }

    public void SetClip(Graphics g, CombineMode combineMode)
    {
      _graphics.SetClip(g, combineMode);
    }

    public void SetClip(Graphics g)
    {
      _graphics.SetClip(g);
    }

    public void SetClip(RectangleF rect)
    {
      _graphics.SetClip(rect);
    }

    public void TransformPoints(CoordinateSpace destSpace, CoordinateSpace srcSpace, PointF[] pts)
    {
      _graphics.TransformPoints(destSpace, srcSpace, pts);
    }

    public void TransformPoints(CoordinateSpace destSpace, CoordinateSpace srcSpace, System.Drawing.Point[] pts)
    {
      _graphics.TransformPoints(destSpace, srcSpace, pts);
    }

    public void TranslateClip(float dx, float dy)
    {
      _graphics.TranslateClip(dx, dy);
    }

    public void TranslateClip(int dx, int dy)
    {
      _graphics.TranslateClip(dx, dy);
    }

    public void TranslateTransform(float dx, float dy)
    {
      _graphics.TranslateTransform(dx, dy);
    }

    public void TranslateTransform(float dx, float dy, MatrixOrder order)
    {
      _graphics.TranslateTransform(dx, dy, order);
    }
  }
}
