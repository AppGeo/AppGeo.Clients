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
using System.Linq;
using GeoAPI.Geometries;
using NetTopologySuite.Geometries;

namespace AppGeo.Clients
{
  [Serializable]
  public abstract class CommonLayer: IComparable<CommonLayer>
  {
    private List<CommonLayer> _children = null;
    private CommonDataFrame _dataFrame = null;
    private bool _defaultVisible = false;
    private CommonField _featureIDField = null;
    private OgcGeometryType _featureType = OgcGeometryType.Point;
    private List<CommonField> _fields = null;
    private CommonField _geometryField = null;
    private string _id = null;
    private CommonLegend _legend = null;
    private double _minimumPixelSize = 0;
    private double _maximumPixelSize = Double.PositiveInfinity;
    private string _name = null;
    private CommonLayer _parent = null;
    private bool _selectable = false;
    private CommonLayerType _type = CommonLayerType.Feature;

    public List<CommonLayer> Children
    {
      get
      {
        return _children;
      }
      internal set
      {
        _children = value;
      }
    }

    public CommonDataFrame DataFrame
    {
      get
      {
        return _dataFrame;
      }
      protected set
      {
        _dataFrame = value;
      }
    }

    public bool DefaultVisible
    {
      get
      {
        return _defaultVisible;
      }
      protected set
      {
        _defaultVisible = value;
      }
    }

    public CommonField FeatureIDField
    {
      get
      {
        return _featureIDField;
      }
      set
      {
        _featureIDField = value;
      }
    }

    public OgcGeometryType FeatureType
    {
      get
      {
        return _featureType;
      }
      protected set
      {
        _featureType = value;
      }
    }

    public List<CommonField> Fields
    {
      get
      {
        return _fields;
      }
      protected set
      {
        _fields = value;
      }
    }

    public CommonField GeometryField
    {
      get
      {
        return _geometryField;
      }
      protected set
      {
        _geometryField = value;
      }
    }

    public string ID
    {
      get
      {
        return _id;
      }
      protected set
      {
        _id = value;
      }
    }

    public CommonLegend Legend
    {
      get
      {
        return _legend;
      }
      protected set
      {
        _legend = value;
      }
    }

    public double MinimumPixelSize
    {
      get
      {
        return _minimumPixelSize;
      }
      protected set
      {
        _minimumPixelSize = value;
      }
    }

    public double MaximumPixelSize
    {
      get
      {
        return _maximumPixelSize;
      }
      protected set
      {
        _maximumPixelSize = value;
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

    public CommonLayer Parent
    {
      get
      {
        return _parent;
      }
      internal set
      {
        _parent = value;
      }
    }

    public bool Selectable
    {
      get
      {
        return _selectable;
      }
      protected set
      {
        _selectable = value;
      }
    }

    public CommonLayerType Type
    {
      get
      {
        return _type;
      }
      internal set
      {
        _type = value;
      }
    }

    public int CompareTo(CommonLayer other)
    {
      return Name.CompareTo(other.Name);
    }

    protected void CheckHasFeatureIDField()
    {
      if (FeatureIDField == null || String.IsNullOrEmpty(FeatureIDField.Name))
      {
        throw new Exception("Cannot retrieve feature IDs, the FeatureIDField has not been set on the layer.");
      }
    }

    protected void CheckIsFeatureLayer()
    {
      if (Type != CommonLayerType.Feature)
      {
        throw new Exception("Cannot perform query, layer does not contain features.");
      }
    }

    protected IGeometry CreateDistanceGeometry(double x, double y, double distance)
    {
      IPoint p = new Point(x, y);
      return distance == 0 ? p : p.Buffer(distance);
    }

    public abstract int GetFeatureCount();

    public abstract int GetFeatureCount(string where);

    public abstract int GetFeatureCount(double x, double y, double distance);

    public abstract int GetFeatureCount(string where, double x, double y, double distance);

    public abstract int GetFeatureCount(IGeometry shape);

    public abstract int GetFeatureCount(string where, IGeometry shape);

    public abstract Envelope GetFeatureExtent();

    public abstract Envelope GetFeatureExtent(string where);

    public abstract List<String> GetFeatureIDs();

    public abstract List<String> GetFeatureIDs(string where);

    public abstract List<String> GetFeatureIDs(double x, double y, double distance);

    public abstract List<String> GetFeatureIDs(string where, double x, double y, double distance);

    public abstract List<String> GetFeatureIDs(IGeometry shape);

    public abstract List<String> GetFeatureIDs(string where, IGeometry shape);

    public abstract DataTable GetFeatureTable(string fieldNames);

    public abstract DataTable GetFeatureTable(string fieldNames, string where);

    public abstract DataTable GetFeatureTable(string fieldNames, double x, double y, double distance);

    public abstract DataTable GetFeatureTable(string fieldNames, string where, double x, double y, double distance);

    public abstract DataTable GetFeatureTable(string fieldNames, IGeometry shape);

    public abstract DataTable GetFeatureTable(string fieldNames, string where, IGeometry shape);

    public abstract FeatureData GetFeatureData(string fieldNames);

    public abstract FeatureData GetFeatureData(string fieldNames, string where);

    public abstract FeatureData GetFeatureData(string fieldNames, double x, double y, double distance);

    public abstract FeatureData GetFeatureData(string fieldNames, string where, double x, double y, double distance);

    public abstract FeatureData GetFeatureData(string fieldNames, IGeometry shape);

    public abstract FeatureData GetFeatureData(string fieldNames, string where, IGeometry shape);

    public bool IsWithinScaleThresholds(double pixelSize)
    {
      return Double.IsNaN(_minimumPixelSize) || Double.IsNaN(_maximumPixelSize) ? false : _minimumPixelSize <= pixelSize && pixelSize <= _maximumPixelSize;
    }

    protected class FieldSelection
    {
      private List<bool> _useAlias = new List<bool>();
      private List<CommonField> _fields = new List<CommonField>();

      public FieldSelection(CommonLayer layer, string fields)
      {
        if (String.IsNullOrEmpty(fields))
        {
          fields = "#";
        }

        if (fields == "*" || fields == "#")
        {
          foreach (CommonField field in layer.Fields)
          {
            if (fields == "*" || field.Type != CommonFieldType.Geometry)
            {
              _fields.Add(field);
              _useAlias.Add(!String.IsNullOrEmpty(field.Alias));
            }
          }
        }
        else
        {
          List<String> fieldNames = new List<String>(fields.Split(new char[] { ',' }));

          foreach (string fieldName in fieldNames)
          {
            string trimmedFieldName = fieldName.Trim();
            CommonField field = layer.Fields.FirstOrDefault(f => String.Compare(trimmedFieldName, f.Name, true) == 0 || String.Compare(trimmedFieldName, f.Alias, true) == 0);

            if (field == null)
            {
              throw new Exception(String.Format("The field '{0}' is not present in layer '{1}'", trimmedFieldName, layer.Name));
            }

            _fields.Add(field);
            _useAlias.Add(String.Compare(fieldName, field.Alias, true) == 0);
          }
        }
      }

      public List<CommonField> Fields
      {
        get
        {
          return _fields;
        }
      }

      public List<bool> UseAlias
      {
        get
        {
          return _useAlias;
        }
      }

      public string NamesToString()
      {
        return NamesToString(",");
      }

      public string NamesToString(string separator)
      {
        return String.Join(separator, _fields.Select(f => f.Name).ToList().ToArray());
      }

      public FeatureData CreateFeatureData()
      {
        FeatureData featureData = new FeatureData();
        featureData.FieldNames = new string[_fields.Count];

        for (int i = 0; i < _fields.Count; ++i)
        {
          featureData.FieldNames[i] = _useAlias[i] ? _fields[i].Alias : _fields[i].Name;
        }

        return featureData;
      }

      public DataTable CreateTable()
      {
        DataTable table = new DataTable();

        for (int i = 0; i < _fields.Count; ++i)
        {
          table.Columns.Add(_fields[i].ToColumn(_useAlias[i]));
        }

        return table;
      }
    }
  }

  public enum CommonLayerType
  {
    Feature,
    Image,
    Annotation,
    Group
  }
}
