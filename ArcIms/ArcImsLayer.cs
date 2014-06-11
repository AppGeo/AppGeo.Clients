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
using AppGeo.Clients;
using AppGeo.Clients.ArcIms.ArcXml;

namespace AppGeo.Clients.ArcIms
{
  public class ArcImsLayer : CommonLayer
  {
    private ArcImsService _service = null;
    private LayerInfo _layerInfo = null;

    public ArcImsLayer(ArcImsDataFrame dataFrame, LayerInfo layerInfo)
    {
      _service = dataFrame.Service as ArcImsService;
      _layerInfo = layerInfo;

      DataFrame = dataFrame;
      DefaultVisible = layerInfo.Visible;
      ID = layerInfo.ID;
      MinimumPixelSize = ConvertIfDouble(layerInfo.MinScale, MinimumPixelSize);
      MaximumPixelSize = ConvertIfDouble(layerInfo.MaxScale, MaximumPixelSize);
      Name = layerInfo.Name;

      if (layerInfo.Type == LayerType.FeatureClass)
      {
        Selectable = true;

        switch (layerInfo.FClass.Type)
        {
          case FClassType.Line: FeatureType = OgcGeometryType.MultiLineString; break;
          case FClassType.Polygon: FeatureType = OgcGeometryType.MultiPolygon; break;
        }

        Fields = new List<CommonField>();

        foreach (Field axlField in layerInfo.FClass.Fields)
        {
          ArcImsField field = new ArcImsField(layerInfo.FClass, axlField);
          Fields.Add(field);

          if (field.Type == CommonFieldType.ID)
          {
            FeatureIDField = field;
          }
          else if (field.Type == CommonFieldType.Geometry)
          {
            GeometryField = field;
          }
        }

        if (layerInfo.Toc != null)
        {
          Legend = new ArcImsLegend(layerInfo.Toc);
        }
      }
      else
      {
        switch (layerInfo.ArcMapType)
        {
          case ArcMapType.Group: Type = CommonLayerType.Group; break;
          case ArcMapType.Annotation: Type = CommonLayerType.Annotation; break;

          default:
            if (layerInfo.Type == LayerType.Image)
            {
              Type = layerInfo.Toc != null && layerInfo.Toc.Count > 0 ? CommonLayerType.Image : CommonLayerType.Annotation;
            }
            break;
        }
      }
    }

    public LayerInfo LayerInfo
    {
      get
      {
        return _layerInfo;
      }
    }

    private double ConvertIfDouble(string s, double defaultValue)
    {
      if (!String.IsNullOrEmpty(s))
      {
        try
        {
          return Convert.ToDouble(s);
        }
        catch { }
      }

      return defaultValue;
    }

    public override int GetFeatureCount()
    {
      return GetFeatureCount("1 = 1");
    }

    public override int GetFeatureCount(string where)
    {
      return GetFeatureCount(where, null);
    }

    public override int GetFeatureCount(double x, double y, double distance)
    {
      return GetFeatureCount("1 = 1", x, y, distance);
    }

    public override int GetFeatureCount(string where, double x, double y, double distance)
    {
      return GetFeatureCount(where, CreateDistanceGeometry(x, y, distance));
    }

    public override int GetFeatureCount(IGeometry shape)
    {
      return GetFeatureCount("1 = 1", shape);
    }

    public override int GetFeatureCount(string where, IGeometry shape)
    {
      CheckIsFeatureLayer();

      SpatialQuery query = new SpatialQuery(where);
      query.Subfields = "#ID#";

      GetFeatures getFeatures = new GetFeatures(ID, query);
      getFeatures.Attributes = false;
      getFeatures.Geometry = false;
      getFeatures.DataFrame = DataFrame.Name;

      if (shape != null)
      {
        query.SpatialFilter = new SpatialFilter(shape);
      }

      Features features = (Features)_service.Send(getFeatures);
      return features.FeatureCount.Count;
    }

    public override Envelope GetFeatureExtent()
    {
      return GetFeatureExtent("1 = 1");
    }

    public override Envelope GetFeatureExtent(string where)
    {
      CheckIsFeatureLayer();

      Query query = new Query(where);

      GetFeatures getFeatures = new GetFeatures(ID, query);
      getFeatures.GlobalEnvelope = true;
      getFeatures.Envelope = false;
      getFeatures.Geometry = false;
      getFeatures.Attributes = false;
      getFeatures.DataFrame = DataFrame.Name;

      Features features = (Features)_service.Send(getFeatures);
      return features.FeatureCount.Count > 0 ? features.Envelope : new Envelope();
    }

    public override List<String> GetFeatureIDs()
    {
      return GetFeatureIDs("1 = 1", null);
    }

    public override List<String> GetFeatureIDs(string where)
    {
      return GetFeatureIDs(where, null);
    }

    public override List<String> GetFeatureIDs(double x, double y, double distance)
    {
      return GetFeatureIDs(null, x, y, distance);
    }

    public override List<String> GetFeatureIDs(string where, double x, double y, double distance)
    {
      return GetFeatureIDs(where, CreateDistanceGeometry(x, y, distance));
    }

    public override List<String> GetFeatureIDs(IGeometry shape)
    {
      return GetFeatureIDs(null, shape);
    }

    public override List<String> GetFeatureIDs(string where, IGeometry shape)
    {
      SpatialQuery query = new SpatialQuery(where, shape);
      query.Subfields = FeatureIDField.Name;

      GetFeatures getFeatures = new GetFeatures(ID, query);
      getFeatures.Geometry = false;
      getFeatures.DataFrame = DataFrame.Name;

      return GetFeatureIDs(getFeatures);
    }

    private List<String> GetFeatureIDs(GetFeatures getFeatures)
    {
      CheckIsFeatureLayer();
      CheckHasFeatureIDField();

      Features features = (Features)_service.Send(getFeatures);
      List<String> idList = GetIDList(features);

      if (features.FeatureCount.HasMore)
      {
        getFeatures.BeginRecord = 1;

        do
        {
          getFeatures.BeginRecord += features.FeatureCount.Count;
          features = (Features)_service.Send(getFeatures);
          idList.AddRange(GetIDList(features));
        }
        while (features.FeatureCount.HasMore);
      }

      idList.Sort();

      for (int i = idList.Count - 1; i >= 1; --i)
      {
        if (String.Compare(idList[i], idList[i - 1], false) == 0)
        {
          idList.RemoveAt(i);
        }
      }

      return idList;
    }

    public override FeatureData GetFeatureData(string fieldNames)
    {
      return GetFeatureData(fieldNames, "1 = 1");
    }

    public override FeatureData GetFeatureData(string fieldNames, string where)
    {
      return GetFeatureData(fieldNames, where, null);
    }

    public override FeatureData GetFeatureData(string fieldNames, double x, double y, double distance)
    {
      return GetFeatureData(fieldNames, null, x, y, distance);
    }

    public override FeatureData GetFeatureData(string fieldNames, string where, double x, double y, double distance)
    {
      return GetFeatureData(fieldNames, where, CreateDistanceGeometry(x, y, distance));
    }

    public override FeatureData GetFeatureData(string fieldNames, IGeometry shape)
    {
      return GetFeatureData(fieldNames, null, shape);
    }

    public override FeatureData GetFeatureData(string fieldNames, string where, IGeometry shape)
    {
      FieldSelection fieldSelection = new FieldSelection(this, fieldNames);
      SpatialQuery query = new SpatialQuery(where, shape);
      query.Subfields = fieldSelection.NamesToString(" ");

      GetFeatures getFeatures = new GetFeatures(ID, query);
      getFeatures.DataFrame = DataFrame.Name;
      getFeatures.Geometry = fieldSelection.Fields.Any(lyr => lyr.Type == CommonFieldType.Geometry);
      return GetFeatureData(getFeatures, fieldSelection);
    }

    private FeatureData GetFeatureData(GetFeatures getFeatures, FieldSelection fieldSelection)
    {
      CheckIsFeatureLayer();

      Features features = (Features)_service.Send(getFeatures);

      FeatureData featureData = fieldSelection.CreateFeatureData();
      featureData.LayerName = Name;
      LoadFeatureData(features, featureData, fieldSelection);

      if (features.FeatureCount.HasMore)
      {
        getFeatures.BeginRecord = 1;

        do
        {
          getFeatures.BeginRecord += features.FeatureCount.Count;
          features = (Features)_service.Send(getFeatures);
          LoadFeatureData(features, featureData, fieldSelection);
        }
        while (features.FeatureCount.HasMore);
      }

      return featureData;
    }

    public override DataTable GetFeatureTable(string fieldNames)
    {
      return GetFeatureTable(fieldNames, "1 = 1");
    }

    public override DataTable GetFeatureTable(string fieldNames, string where)
    {
      return GetFeatureTable(fieldNames, where, null);
    }

    public override DataTable GetFeatureTable(string fieldNames, double x, double y, double distance)
    {
      return GetFeatureTable(fieldNames, null, x, y, distance);
    }

    public override DataTable GetFeatureTable(string fieldNames, string where, double x, double y, double distance)
    {
      return GetFeatureTable(fieldNames, where, CreateDistanceGeometry(x, y, distance));
    }

    public override DataTable GetFeatureTable(string fieldNames, IGeometry shape)
    {
      return GetFeatureTable(fieldNames, null, shape);
    }

    public override DataTable GetFeatureTable(string fieldNames, string where, IGeometry shape)
    {
      FieldSelection fieldSelection = new FieldSelection(this, fieldNames);
      SpatialQuery query = new SpatialQuery(where, shape);
      query.Subfields = fieldSelection.NamesToString(" ");

      GetFeatures getFeatures = new GetFeatures(ID, query);
      getFeatures.DataFrame = DataFrame.Name;
      getFeatures.Geometry = fieldSelection.Fields.Any(lyr => lyr.Type == CommonFieldType.Geometry);
      return GetFeatureTable(getFeatures, fieldSelection);
    }

    private DataTable GetFeatureTable(GetFeatures getFeatures, FieldSelection fieldSelection)
    {
      CheckIsFeatureLayer();

      Features features = (Features)_service.Send(getFeatures);

      DataTable table = fieldSelection.CreateTable();
      LoadFeatureTable(features, table, fieldSelection);

      if (features.FeatureCount.HasMore)
      {
        getFeatures.BeginRecord = 1;

        do
        {
          getFeatures.BeginRecord += features.FeatureCount.Count;
          features = (Features)_service.Send(getFeatures);
          LoadFeatureTable(features, table, fieldSelection);
        }
        while (features.FeatureCount.HasMore);
      }

      return table;
    }

    private List<String> GetIDList(Features features)
    {
      List<String> idList = new List<String>();

      foreach (Feature feature in features)
      {
        if (feature.Fields[0].Value.Length > 0)
        {
          idList.Add(feature.Fields[0].Value);
        }
      }

      return idList;
    }

    private void LoadFeatureData(Features features, FeatureData featureData, FieldSelection fieldSelection)
    {
      if (features.Count == 0)
      {
        return;
      }

      int[] fieldIndex = new int[fieldSelection.Fields.Count];
      var select = features[0].Fields.Select((field, index) => new { field, index });

      for (int i = 0; i < fieldSelection.Fields.Count; ++i)
      {
        var result = select.Where(x => String.Compare(x.field.Name, fieldSelection.Fields[i].Name, true) == 0).FirstOrDefault();
        fieldIndex[i] = result == null ? -1 : result.index;
      }

      int offset = 0;

      if (featureData.Rows == null)
      {
        featureData.Rows = new FeatureRow[features.Count];
      }
      else
      {
        offset = featureData.Rows.Length;
        Array.Resize(ref featureData.Rows, offset + features.Count);
      }

      for (int r = 0; r < features.Count; ++r)
      {
        FeatureRow featureRow = new FeatureRow();
        featureRow.Values = new object[fieldSelection.Fields.Count];

        for (int c = 0; c < fieldSelection.Fields.Count; ++c)
        {
          if (fieldIndex[c] >= 0)
          {
            string value = features[r].Fields[fieldIndex[c]].Value;

            if (!String.IsNullOrEmpty(value) || features[r].Shape != null)
            {
              switch (fieldSelection.Fields[c].Type)
              {
                case CommonFieldType.Geometry:
                  if (features[r].Shape.OgcGeometryType == OgcGeometryType.MultiPoint && fieldSelection.Fields[c].GeometryType == OgcGeometryType.Point)
                  {
                    featureRow.Values[c] = ((IMultiPoint)features[r].Shape)[0];
                  }
                  else
                  {
                    featureRow.Values[c] = features[r].Shape;
                  }
                  break;

                case CommonFieldType.Date:
                  featureRow.Values[c] = new DateTime((Convert.ToInt64(value) * 10000) + 621355968000000000);
                  break;

                default:
                  featureRow.Values[c] = Convert.ChangeType(value, fieldSelection.Fields[c].DataType);
                  break;
              }
            }
          }
        }

        featureData.Rows[r + offset] = featureRow;
      }
    }

    private void LoadFeatureTable(Features features, DataTable table, FieldSelection fieldSelection)
    {
      if (features.Count == 0)
      {
        return;
      }

      int[] fieldIndex = new int[fieldSelection.Fields.Count];
      var select = features[0].Fields.Select((field, index) => new { field, index });

      for (int i = 0; i < fieldSelection.Fields.Count; ++i)
      {
        var result = select.Where(x => String.Compare(x.field.Name, fieldSelection.Fields[i].Name, true) == 0).FirstOrDefault();
        fieldIndex[i] = result == null ? -1 : result.index;
      }

      foreach (Feature feature in features)
      {
        DataRow row = table.NewRow();

        for (int c = 0; c < fieldSelection.Fields.Count; ++c)
        {
          if (fieldIndex[c] >= 0)
          {
            string value = feature.Fields[fieldIndex[c]].Value;

            if (!String.IsNullOrEmpty(value) || feature.Shape != null)
            {
              switch (fieldSelection.Fields[c].Type)
              {
                case CommonFieldType.Geometry:
                  if (feature.Shape.OgcGeometryType == OgcGeometryType.MultiPoint && fieldSelection.Fields[c].GeometryType == OgcGeometryType.Point)
                  {
                    row[c] = ((IMultiPoint)feature.Shape)[0];
                  }
                  else
                  {
                    row[c] = feature.Shape;
                  }
                  break;

                case CommonFieldType.Date:
                  row[c] = new DateTime((Convert.ToInt64(value) * 10000) + 621355968000000000);
                  break;

                default:
                  row[c] = Convert.ChangeType(value, table.Columns[c].DataType);
                  break;
              }
            }
          }
        }

        table.Rows.Add(row);
      }
    }

    internal void ReloadLegend()
    {
      if (Type == CommonLayerType.Feature)
      {
        Legend = null;

        if (_layerInfo.Toc != null)
        {
          Legend = new ArcImsLegend(_layerInfo.Toc);
        }
      }
    }
  }
}
