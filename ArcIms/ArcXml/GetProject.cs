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

namespace AppGeo.Clients.ArcIms.ArcXml
{
  [Serializable]
  public class GetProject : Request, ICloneable
  {
    public const string XmlName = "GET_PROJECT";

    public double Accuracy = 0;
    public bool Compact = false;
    public double DensifyTolerance = 0;
    public bool Envelope = false;

    private FromCoordSys _fromCoordSys = new FromCoordSys();
    private ToCoordSys _toCoordSys = new ToCoordSys();
    private List<IGeometry> _shapes = new List<IGeometry>();

    public Environment Environment = null;

    public GetProject(string fromCoordSysID, string toCoordSysID, IGeometry shape)
    {
      _fromCoordSys.ID = fromCoordSysID;
      _toCoordSys.ID = toCoordSysID;
      _shapes.Add(shape);
    }

    public GetProject(FromCoordSys fromCoordSys, ToCoordSys toCoordSys, IGeometry shape)
    {
      _fromCoordSys = fromCoordSys;
      _toCoordSys = toCoordSys;
      _shapes.Add(shape);
    }

    public GetProject(string fromCoordSysID, string toCoordSysID, IEnumerable<IGeometry> shapes)
    {
      _fromCoordSys.ID = fromCoordSysID;
      _toCoordSys.ID = toCoordSysID;
      _shapes.AddRange(shapes);
    }

    public GetProject(FromCoordSys fromCoordSys, ToCoordSys toCoordSys, IEnumerable<IGeometry> shapes)
    {
      _fromCoordSys = fromCoordSys;
      _toCoordSys = toCoordSys;
      _shapes.AddRange(shapes);
    }

    public FromCoordSys FromCoordSys
    {
      get
      {
        return _fromCoordSys;
      }
    }

    public ToCoordSys ToCoordSys
    {
      get
      {
        return _toCoordSys;
      }
    }

    public List<IGeometry> Shapes
    {
      get
      {
        return _shapes;
      }
    }

    public override object Clone()
    {
      GetProject clone = (GetProject)this.MemberwiseClone();

      if (Environment != null)
      {
        clone.Environment = (Environment)Environment.Clone();
      }

      clone._fromCoordSys = (FromCoordSys)_fromCoordSys.Clone();
      clone._toCoordSys = (ToCoordSys)_toCoordSys.Clone();

      clone._shapes = new List<IGeometry>();

      foreach (IGeometry shape in _shapes)
      {
        clone._shapes.Add((IGeometry)shape.Clone());
      }

      return clone;
    }

    public override void WriteTo(ArcXmlWriter writer)
    {
      try
      {
        writer.WriteStartElement(XmlName);

        if (Accuracy != 0)
        {
          writer.WriteAttributeString("accuracy", Accuracy.ToString());
        }

        if (Compact)
        {
          writer.WriteAttributeString("compact", "true");
        }

        if (DensifyTolerance != 0)
        {
          writer.WriteAttributeString("densifytolerance", DensifyTolerance.ToString());
        }

        if (Envelope)
        {
          writer.WriteAttributeString("envelope", "true");
        }

        if (Environment != null)
        {
          Environment.WriteTo(writer);
        }

        _fromCoordSys.WriteTo(writer);
        _toCoordSys.WriteTo(writer);

        foreach (IGeometry shape in _shapes)
        {
          if (shape.OgcGeometryType == OgcGeometryType.Point)
          {
            GeometrySerializer.WriteAsMultiPointTo(writer, (IPoint)shape);
          }
          else
          {
            GeometrySerializer.WriteTo(writer, shape);
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
