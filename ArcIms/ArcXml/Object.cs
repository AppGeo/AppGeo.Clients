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
using System.Xml;
using GeoAPI.Geometries;
using NetTopologySuite.Geometries;

namespace AppGeo.Clients.ArcIms.ArcXml
{
	public class Object : ICloneable
	{
		public const string XmlName = "OBJECT";

    public ObjectAlignment Alignment = ObjectAlignment.BottomLeft;
		public ObjectUnits Units = ObjectUnits.Database;
		public string Lower = null;
		public string Upper = null;

		public CoordSys CoordSys = null;
		public Symbol Symbol = null;

		private IGeometry _shape = null;
		private NorthArrow _northArrow = null;
		private ScaleBar _scaleBar = null;
    private Text _text = null;

		public Object() { }

    public Object(Envelope envelope, Symbol symbol)
    {
      IPolygon polygon = new Polygon(new LinearRing(new Coordinate[] {
        new Coordinate(envelope.MinX, envelope.MinY),
        new Coordinate(envelope.MinX, envelope.MaxY),
        new Coordinate(envelope.MaxX, envelope.MaxY),
        new Coordinate(envelope.MaxX, envelope.MinY),
        new Coordinate(envelope.MinX, envelope.MinY)
      }));

      _shape = polygon;
      Symbol = symbol;
    }

    public Object(IGeometry shape, Symbol symbol)
    {
      _shape = shape;
      Symbol = symbol;
    }

    public Object(NorthArrow northArrow)
    {
      Units = ObjectUnits.Pixel;
      _northArrow = northArrow;
    }

    public Object(ScaleBar scaleBar)
    {
      Units = ObjectUnits.Pixel;
      _scaleBar = scaleBar;
    }

    public Object(Text text)
    {
      _text = text;
    }

    public Object(Text text, TextMarkerSymbol symbol)
    {
      _text = text;
      _text.Symbol = symbol;
    }

		public IGeometry Shape
		{
			get
			{
				return _shape;
			}
			set
			{
				_shape = value;

        if (_shape != null)
				{
					_northArrow = null;
					_scaleBar = null;
          _text = null;
				}
			}
		}

		public NorthArrow NorthArrow
		{
			get
			{
				return _northArrow;
			}
			set
			{
				_northArrow = value;

				if (_northArrow != null)
				{
          _shape = null;
					_scaleBar = null;
          _text = null;
        }
			}
		}

		public ScaleBar ScaleBar
		{
			get
			{
				return _scaleBar;
			}
			set
			{
				_scaleBar = value;

				if (_scaleBar != null)
				{
          _shape = null;
					_northArrow = null;
          _text = null;
        }
			}
		}

    public Text Text
    {
      get
      {
        return _text;
      }
      set
      {
        _text = value;

        if (_text != null)
        {
          _shape = null;
          _northArrow = null;
          _scaleBar = null;
        }
      }
    }

		public object Clone()
		{
			Object clone = (Object)this.MemberwiseClone();

			if (CoordSys != null)
			{
				clone.CoordSys = (CoordSys)CoordSys.Clone();
			}

			if (_shape != null)
			{
				clone._shape = (IGeometry)_shape.Clone();
			}

			if (_northArrow != null)
			{
				clone._northArrow = (NorthArrow)_northArrow.Clone();
			}

			if (_scaleBar != null)
			{
				clone._scaleBar = (ScaleBar)_scaleBar.Clone();
			}

      if (_text != null)
      {
        clone._text = (Text)_text.Clone();
      }

			if (Symbol != null)
			{
				clone.Symbol = (Symbol)Symbol.Clone();
			}

			return clone;
		}

		public void WriteTo(ArcXmlWriter writer)
		{
			try
			{
				writer.WriteStartElement(XmlName);

				if (!String.IsNullOrEmpty(Lower))
				{
					writer.WriteAttributeString("lower", Lower);
				}

        if (Alignment != ObjectAlignment.BottomLeft)
        {
          writer.WriteAttributeString("alignment", ArcXmlEnumConverter.ToArcXml(typeof(ObjectAlignment), Alignment));
        }

				writer.WriteAttributeString("units", ArcXmlEnumConverter.ToArcXml(typeof(ObjectUnits), Units));

				if (!String.IsNullOrEmpty(Upper))
				{
					writer.WriteAttributeString("upper", Upper);
				}

				if (CoordSys != null)
				{
					CoordSys.WriteTo(writer);
				}

				if (_shape != null)
				{
          if (_shape.OgcGeometryType == OgcGeometryType.Point)
          {
            GeometrySerializer.WriteAsMultiPointTo(writer, (IPoint)_shape);
          }
          else
          {
            GeometrySerializer.WriteTo(writer, _shape);
          }

          if (Symbol != null)
          {
            Symbol.WriteTo(writer);
          }
				}

				if (_northArrow != null)
				{
					_northArrow.WriteTo(writer);
				}

				if (_scaleBar != null)
				{
					_scaleBar.WriteTo(writer);
				}

        if (_text != null)
        {
          bool symbolAdded = false;

          if (_text.Symbol == null)
          {
            _text.Symbol = (TextMarkerSymbol)Symbol;
            symbolAdded = true;
          }

          _text.WriteTo(writer);

          if (symbolAdded)
          {
            _text.Symbol = null;
          }
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
					throw new ArcXmlException(String.Format("Could not write {0} object.", GetType().Name), ex);
				}
			}
		}
	}
}
