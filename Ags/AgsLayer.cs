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
using AppGeo.Clients.Ags.Proxy;

namespace AppGeo.Clients.Ags
{
  [Serializable]
  public class AgsLayer : CommonLayer
  {
    private const double MetersPerFoot = 0.3048;

    private AgsMapService _service = null;
    private MapLayerInfo _mapLayerInfo = null;
    private MapServerLegendInfo _mapServerLegendInfo = null;

    public AgsLayer(AgsDataFrame dataFrame, MapLayerInfo mapLayerInfo, MapServerLegendInfo mapServerLegendInfo, bool defaultVisible)
    {
      _service = dataFrame.Service as AgsMapService;
      _mapLayerInfo = mapLayerInfo;
      _mapServerLegendInfo = mapServerLegendInfo;

      DataFrame = dataFrame;
      DefaultVisible = defaultVisible;
      ID = mapLayerInfo.LayerID.ToString();
      Name = mapLayerInfo.Name;

      double toPixels = 1.0 / (12 * dataFrame.Dpi);

      if (dataFrame.MapServerInfo.Units == esriUnits.esriMeters)
      {
        toPixels *= MetersPerFoot;
      }

      if (mapLayerInfo.MaxScale > 0)
      {
        MinimumPixelSize = mapLayerInfo.MaxScale * toPixels;
      }

      if (mapLayerInfo.MinScale > 0)
      {
        MaximumPixelSize = mapLayerInfo.MinScale * toPixels;
      }

      if (mapLayerInfo.IsFeatureLayer && mapLayerInfo.LayerType != "Annotation Layer" && mapLayerInfo.LayerType != "Raster Catalog Layer")
      {
        Selectable = mapLayerInfo.CanSelect;
        Fields = new List<CommonField>();

        foreach (Field proxyField in mapLayerInfo.Fields.FieldArray)
        {
          AgsField field = new AgsField(proxyField);
          Fields.Add(field);

          if (field.Type == CommonFieldType.ID)
          {
            FeatureIDField = field;
          }
          else if (field.Type == CommonFieldType.Geometry)
          {
            GeometryField = field;

            switch (proxyField.GeometryDef.GeometryType)
            {
              case esriGeometryType.esriGeometryPoint: FeatureType = OgcGeometryType.Point; break;
              case esriGeometryType.esriGeometryMultipoint: FeatureType = OgcGeometryType.MultiPoint; break;
              case esriGeometryType.esriGeometryPolyline: FeatureType = OgcGeometryType.MultiLineString; break;
              case esriGeometryType.esriGeometryPolygon: FeatureType = OgcGeometryType.MultiPolygon; break;
            }
          }
        }

        Legend = new AgsLegend(mapServerLegendInfo);
      }
      else
      {
        switch (mapLayerInfo.LayerType)
        {
          case "Raster Catalog Layer":
          case "Raster Layer":
            Type = CommonLayerType.Image;
            break;

          case "Annotation Layer": Type = CommonLayerType.Annotation; break;
          case "Group Layer": Type = CommonLayerType.Group; break;
        }
      }
    }

    public MapLayerInfo MapLayerInfo
    {
      get
      {
        return _mapLayerInfo;
      }
    }

    public MapServerLegendInfo MapServerLegendInfo
    {
      get
      {
        return _mapServerLegendInfo;
      }
    }

    public override int GetFeatureCount()
    {
      return GetFeatureCount("");
    }

    public override int GetFeatureCount(string where)
    {
      QueryFilter queryFilter = new QueryFilter(where);
      return _service.MapServer.QueryFeatureCount(DataFrame.Name, _mapLayerInfo.LayerID, queryFilter);
    }

    public override int GetFeatureCount(double x, double y, double distance)
    {
      return GetFeatureCount("", x, y, distance);
    }

    public override int GetFeatureCount(string where, double x, double y, double distance)
    {
      return GetFeatureCount(where, CreateDistanceGeometry(x, y, distance));
    }

    public override int GetFeatureCount(IGeometry shape)
    {
      return GetFeatureCount("", shape);
    }

    public override int GetFeatureCount(string where, IGeometry shape)
    {
      SpatialFilter spatialFilter = new SpatialFilter(shape, where);
      spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
      return _service.MapServer.QueryFeatureCount(DataFrame.Name, _mapLayerInfo.LayerID, spatialFilter);
    }

    public override GeoAPI.Geometries.Envelope GetFeatureExtent()
    {
      return GetFeatureExtent("");
    }

    public override GeoAPI.Geometries.Envelope GetFeatureExtent(string where)
    {
      FieldSelection fieldSelection = new FieldSelection(this, GeometryField.Name);
      QueryFilter queryFilter = new QueryFilter(GeometryField.Name, where);
      RecordSet recordSet = _service.MapServer.QueryFeatureData(DataFrame.Name, _mapLayerInfo.LayerID, queryFilter);
      DataTable table = RecordSetToTable(recordSet, fieldSelection);

      GeoAPI.Geometries.Envelope extent = new GeoAPI.Geometries.Envelope();

      foreach (DataRow row in table.Rows)
      {
        if (!row.IsNull(0))
        {
          extent.ExpandToInclude(((IGeometry)row[0]).EnvelopeInternal);
        }
      }

      return extent;
    }

    public override List<String> GetFeatureIDs()
    {
      return GetFeatureIDs("");
    }

    public override List<String> GetFeatureIDs(string where)
    {
      CheckIsFeatureLayer();
      CheckHasFeatureIDField();

      QueryFilter queryFilter = new QueryFilter(FeatureIDField.Name, where);
      RecordSet recordSet = _service.MapServer.QueryFeatureData(DataFrame.Name, _mapLayerInfo.LayerID, queryFilter);
      return RecordSetToIds(recordSet);
    }

    public override List<String> GetFeatureIDs(double x, double y, double distance)
    {
      return GetFeatureIDs("", x, y, distance);
    }

    public override List<String> GetFeatureIDs(string where, double x, double y, double distance)
    {
      return GetFeatureIDs(where, CreateDistanceGeometry(x, y, distance));
    }

    public override List<String> GetFeatureIDs(IGeometry shape)
    {
      return GetFeatureIDs("", shape);
    }

    public override List<String> GetFeatureIDs(string where, IGeometry shape)
    {
      CheckIsFeatureLayer();
      CheckHasFeatureIDField();

      SpatialFilter spatialFilter = new SpatialFilter(shape, FeatureIDField.Name, where);
      spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
      RecordSet recordSet = _service.MapServer.QueryFeatureData(DataFrame.Name, _mapLayerInfo.LayerID, spatialFilter);
      return RecordSetToIds(recordSet);
    }

    public override FeatureData GetFeatureData(string fieldNames)
    {
      return GetFeatureData(fieldNames, "");
    }

    public override FeatureData GetFeatureData(string fieldNames, string where)
    {
      CheckIsFeatureLayer();

      FieldSelection fieldSelection = new FieldSelection(this, fieldNames);
      QueryFilter queryFilter = new QueryFilter(fieldSelection.NamesToString(), where);
      RecordSet recordSet = _service.MapServer.QueryFeatureData(DataFrame.Name, _mapLayerInfo.LayerID, queryFilter);
      return RecordSetToFeatureData(recordSet, fieldSelection);
    }

    public override FeatureData GetFeatureData(string fieldNames, double x, double y, double distance)
    {
      return GetFeatureData(fieldNames, "", x, y, distance);
    }

    public override FeatureData GetFeatureData(string fieldNames, string where, double x, double y, double distance)
    {
      return GetFeatureData(fieldNames, where, CreateDistanceGeometry(x, y, distance));
    }

    public override FeatureData GetFeatureData(string fieldNames, IGeometry shape)
    {
      return GetFeatureData(fieldNames, "", shape);
    }

    public override FeatureData GetFeatureData(string fieldNames, string where, IGeometry shape)
    {
      CheckIsFeatureLayer();

      FieldSelection fieldSelection = new FieldSelection(this, fieldNames);
      SpatialFilter spatialFilter = new SpatialFilter(shape, fieldSelection.NamesToString(), where);
      spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
      RecordSet recordSet = _service.MapServer.QueryFeatureData(DataFrame.Name, _mapLayerInfo.LayerID, spatialFilter);
      return RecordSetToFeatureData(recordSet, fieldSelection);
    }

    public override DataTable GetFeatureTable(string fieldNames)
    {
      return GetFeatureTable(fieldNames, "");
    }

    public override DataTable GetFeatureTable(string fieldNames, string where)
    {
      CheckIsFeatureLayer();

      FieldSelection fieldSelection = new FieldSelection(this, fieldNames);
      QueryFilter queryFilter = new QueryFilter(fieldSelection.NamesToString(), where);
      RecordSet recordSet = _service.MapServer.QueryFeatureData(DataFrame.Name, _mapLayerInfo.LayerID, queryFilter);
      return RecordSetToTable(recordSet, fieldSelection);
    }

    public override DataTable GetFeatureTable(string fieldNames, double x, double y, double distance)
    {
      return GetFeatureTable(fieldNames, "", x, y, distance);
    }

    public override DataTable GetFeatureTable(string fieldNames, string where, double x, double y, double distance)
    {
      return GetFeatureTable(fieldNames, where, CreateDistanceGeometry(x, y, distance));
    }

    public override DataTable GetFeatureTable(string fieldNames, IGeometry shape)
    {
      return GetFeatureTable(fieldNames, "", shape);
    }

    public override DataTable GetFeatureTable(string fieldNames, string where, IGeometry shape)
    {
      CheckIsFeatureLayer();

      FieldSelection fieldSelection = new FieldSelection(this, fieldNames);
      SpatialFilter spatialFilter = new SpatialFilter(shape, fieldSelection.NamesToString(), where);
      spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
      RecordSet recordSet = _service.MapServer.QueryFeatureData(DataFrame.Name, _mapLayerInfo.LayerID, spatialFilter);
      return RecordSetToTable(recordSet, fieldSelection);
    }

    public string GetRasterValue(double x, double y)
    {
      if (Type != CommonLayerType.Image)
      {
        throw new AgsException("GetRasterValue can only be performed on image layers.");
      }

      string rasterValue = null;

      MapDescription mapDescription = ((AgsDataFrame)DataFrame).MapServerInfo.NewMapDescription(new GeoAPI.Geometries.Envelope(x - 1, y - 1, x + 1, y + 1));
      ImageDisplay imageDisplay = new ImageDisplay(2, 2);

      MapServerIdentifyResult[] result = _service.MapServer.Identify(mapDescription, imageDisplay, PointN.FromCoordinate(new Coordinate(x, y)), 0,
          esriIdentifyOption.esriIdentifyAllLayers, new int[] { _mapLayerInfo.LayerID });

      if (result.Length > 0)
      {
        rasterValue = String.Join(",", result[0].Properties.PropertyArray.Select(o => o.Value.ToString()).ToArray());
      }

      return rasterValue;
    }

    private List<String> RecordSetToIds(RecordSet recordSet)
    {
      List<String> idList = new List<String>();

      List<Field> fields = recordSet.Fields.FieldArray.ToList();
      Field idField = fields.FirstOrDefault(f => String.Compare(f.Name, FeatureIDField.Name) == 0);

      if (idField != null)
      {
        int c = fields.IndexOf(idField);

        foreach (Record record in recordSet.Records)
        {
          idList.Add(record.Values[c].ToString());
        }
      }

      return idList;
    }

    private FeatureData RecordSetToFeatureData(RecordSet recordSet, FieldSelection fieldSelection)
    {
      int fieldCount = fieldSelection.Fields.Count;

      int[] fieldIndex = new int[fieldCount];
      var select = recordSet.Fields.FieldArray.Select((field, index) => new { field, index });

      for (int i = 0; i < fieldCount; ++i)
      {
        var result = select.Where(x => String.Compare(x.field.Name, fieldSelection.Fields[i].Name, true) == 0).FirstOrDefault();
        fieldIndex[i] = result == null ? -1 : result.index;
      }

      FeatureData featureData = fieldSelection.CreateFeatureData();
      featureData.LayerName = Name;
      featureData.Rows = new FeatureRow[recordSet.Records.Length];

      for (int r = 0; r < recordSet.Records.Length; ++r)
      {
        FeatureRow featureRow = new FeatureRow();
        featureRow.Values = new object[fieldCount];

        for (int c = 0; c < fieldSelection.Fields.Count; ++c)
        {
          if (fieldIndex[c] >= 0)
          {
            object value = recordSet.Records[r].Values[fieldIndex[c]];

            if (value != null)
            {
              if (fieldSelection.Fields[c].Type == CommonFieldType.Geometry)
              {
                featureRow.Values[c] = ((AppGeo.Clients.Ags.Proxy.Geometry)value).ToCommon();
              }
              else
              {
                featureRow.Values[c] = value;
              }
            }
          }
        }

        featureData.Rows[r] = featureRow;
      }

      return featureData;
    }

    private DataTable RecordSetToTable(RecordSet recordSet, FieldSelection fieldSelection)
    {
      int[] fieldIndex = new int[fieldSelection.Fields.Count];
      var select = recordSet.Fields.FieldArray.Select((field, index) => new { field, index });

      for (int i = 0; i < fieldSelection.Fields.Count; ++i)
      {
        var result = select.Where(x => String.Compare(x.field.Name, fieldSelection.Fields[i].Name, true) == 0).FirstOrDefault();
        fieldIndex[i] = result == null ? -1 : result.index;
      }

      DataTable table = fieldSelection.CreateTable();

      foreach (Record record in recordSet.Records)
      {
        DataRow row = table.NewRow();

        for (int c = 0; c < fieldSelection.Fields.Count; ++c)
        {
          if (fieldIndex[c] >= 0)
          {
            object value = record.Values[fieldIndex[c]];

            if (value != null)
            {
              if (fieldSelection.Fields[c].Type == CommonFieldType.Geometry)
              {
                row[c] = ((AppGeo.Clients.Ags.Proxy.Geometry)value).ToCommon();
              }
              else
              {
                row[c] = value;
              }
            }
          }
        }

        table.Rows.Add(row);
      }

      return table;
    }
  }
}
