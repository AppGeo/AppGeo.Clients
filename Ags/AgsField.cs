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
using AppGeo.Clients.Ags.Proxy;

namespace AppGeo.Clients.Ags
{
  [Serializable]
  public class AgsField : CommonField
  {
    private Field _field = null;

    public AgsField(Field field)
    {
      _field = field;

      Alias = String.IsNullOrEmpty(field.AliasName) ? ExtractShortName(field.Name) : field.AliasName;
      Name = field.Name;
      Required = field.RequiredSpecified ? field.Required : false;

      switch (field.Type)
      {
        case esriFieldType.esriFieldTypeDate: 
          Type = CommonFieldType.Date; 
          break;

        case esriFieldType.esriFieldTypeDouble: 
          Type = CommonFieldType.Double;
          break;

        case esriFieldType.esriFieldTypeGeometry: 
          Type = CommonFieldType.Geometry;

          switch (field.GeometryDef.GeometryType)
          {
            case esriGeometryType.esriGeometryMultipoint: GeometryType = OgcGeometryType.MultiPoint; break;
            case esriGeometryType.esriGeometryPolyline: GeometryType = OgcGeometryType.MultiLineString; break;
            case esriGeometryType.esriGeometryPolygon: GeometryType = OgcGeometryType.MultiPolygon; break;
          }
          break;

        case esriFieldType.esriFieldTypeInteger: 
          Type = CommonFieldType.Integer;
          break;

        case esriFieldType.esriFieldTypeOID: 
          Type = CommonFieldType.ID;
          break;

        case esriFieldType.esriFieldTypeSingle: 
          Type = CommonFieldType.Single;
          break;

        case esriFieldType.esriFieldTypeSmallInteger: 
          Type = CommonFieldType.SmallInteger;
          break;

        case esriFieldType.esriFieldTypeString: 
          Type = CommonFieldType.String; 
          break;
      }
    }

    public Field Field
    {
      get
      {
        return _field;
      }
    }
  }
}
