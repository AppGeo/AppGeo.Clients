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
using AppGeo.Clients;
using AppGeo.Clients.ArcIms.ArcXml;

namespace AppGeo.Clients.ArcIms
{
  public class ArcImsField : CommonField
  {
    private Field _field = null;
    private GcInput _gcInput = null;

    public ArcImsField(FClass fClass, Field field)
    {
      _field = field;

      if (field.Name == "#SHAPE#")
      {
        Alias = "SHAPE";
      }
      else if (String.IsNullOrEmpty(field.Alias))
      {
        Alias = ExtractShortName(field.Name);
      }
      else
      {
        Alias = field.Alias;
      }

      Name = field.Name;

      switch (field.Type)
      {
        case FieldType.BigInteger:
          Type = CommonFieldType.BigInteger;
          break;

        case FieldType.Boolean:
          Type = CommonFieldType.Boolean;
          break;

        case FieldType.Date:
          Type = CommonFieldType.Date; 
          break;

        case FieldType.Double:
          Type = CommonFieldType.Double;
          break;

        case FieldType.Shape:
          Type = CommonFieldType.Geometry;

          switch (fClass.Type)
          {
            case FClassType.Line: GeometryType = OgcGeometryType.MultiLineString; break;
            case FClassType.Polygon: GeometryType = OgcGeometryType.MultiPolygon; break;
          }
          break;

        case FieldType.Integer: 
          Type = CommonFieldType.Integer;
          break;

        case FieldType.RowID: 
          Type = CommonFieldType.ID;
          break;

        case FieldType.Float: 
          Type = CommonFieldType.Single;
          break;

        case FieldType.SmallInteger: 
          Type = CommonFieldType.SmallInteger;
          break;

        case FieldType.Character:
        case FieldType.String:
        case FieldType.Clob:
        case FieldType.NVarChar:
        case FieldType.NClob:
          Type = CommonFieldType.String; 
          break;
      }
    }

    public ArcImsField(GcInput gcInput)
    {
      _gcInput = gcInput;

      Alias = gcInput.Label;
      Name = gcInput.ID;
      Type = CommonFieldType.String;
    }

    public Field Field
    {
      get
      {
        return _field;
      }
    }

    public GcInput GcInput
    {
      get
      {
        return _gcInput;
      }
    }
  }
}
