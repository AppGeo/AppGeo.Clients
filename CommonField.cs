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
using System.Data;
using GeoAPI.Geometries;
using NetTopologySuite.Geometries;

namespace AppGeo.Clients
{
  [Serializable]
  public abstract class CommonField
  {
    private string _alias = null;
    private OgcGeometryType _geometryType = OgcGeometryType.Point;
    private string _name = null;
    private bool _required = false;
    private CommonFieldType _type = CommonFieldType.ClientSpecific;

    public string Alias
    {
      get
      {
        return _alias;
      }
      protected set
      {
        _alias = value;
      }
    }

    public virtual Type DataType
    {
      get
      {
        switch (_type)
        {
          case CommonFieldType.BigInteger:
          case CommonFieldType.ID:
            return typeof(long);

          case CommonFieldType.Boolean: return typeof(bool);
          case CommonFieldType.Date: return typeof(DateTime);
          case CommonFieldType.Double: return typeof(double);

          case CommonFieldType.Geometry:
            switch (_geometryType)
            {
              case OgcGeometryType.Point: return typeof(Point);
              case OgcGeometryType.LineString: return typeof(LineString);
              case OgcGeometryType.Polygon: return typeof(Polygon);
              case OgcGeometryType.MultiPoint: return typeof(MultiPoint);
              case OgcGeometryType.MultiLineString: return typeof(MultiLineString);
              case OgcGeometryType.MultiPolygon: return typeof(MultiPolygon);
            }
            break;

          case CommonFieldType.Integer: return typeof(int);
          case CommonFieldType.Single: return typeof(float);
          case CommonFieldType.SmallInteger: return typeof(short);
          case CommonFieldType.String: return typeof(string);
        }

        return null;
      }
    }

    public OgcGeometryType GeometryType
    {
      get
      {
        return _geometryType;
      }
      protected set
      {
        _geometryType = value;
      }
    }

    public bool IsNumeric
    {
      get
      {
        return _type == CommonFieldType.Integer || _type == CommonFieldType.Double || _type == CommonFieldType.SmallInteger || 
          _type == CommonFieldType.Single || _type == CommonFieldType.ID || _type == CommonFieldType.BigInteger; 
      }
    }

    public string Name
    {
      get
      {
        return _name;
      }
      protected set
      {
        _name = value;
      }
    }

    public bool Required
    {
      get
      {
        return _required;
      }
      protected set
      {
        _required = value;
      }
    }

    public CommonFieldType Type
    {
      get
      {
        return _type;
      }
      protected set
      {
        _type = value;
      }
    }

    protected string ExtractShortName(string fullName)
    {
      int firstIndex = fullName.IndexOf('.');
      int lastIndex = fullName.LastIndexOf('.');

      if (0 < firstIndex && firstIndex < lastIndex - 1 && lastIndex < fullName.Length - 1)
      {
        return fullName.Substring(lastIndex + 1);
      }

      return null;
    }

    public DataColumn ToColumn()
    {
      return ToColumn(false);
    }

    public virtual DataColumn ToColumn(bool useAlias)
    {
      if (_type == CommonFieldType.ClientSpecific)
      {
        throw new Exception(String.Format("Cannot convert client specific field '{0}' to a DataColumn", _name));
      }

      return new DataColumn(useAlias && !String.IsNullOrEmpty(_alias) ? _alias : _name, DataType);
    }

    public override string ToString()
    {
      return Name ?? "";
    }
  }

  public enum CommonFieldType
  {
    ClientSpecific,
    BigInteger,
    Boolean,
    Date,
    Double,
    Geometry,
    ID,
    Integer,
    Single,
    SmallInteger,
    String
  }
}
