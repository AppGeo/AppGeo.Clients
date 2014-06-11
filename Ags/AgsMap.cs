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
using GeoAPI.Geometries;
using AppGeo.Clients;
using AppGeo.Clients.Ags.Proxy;

namespace AppGeo.Clients.Ags
{
  public class AgsMap : CommonMap
  {
    private AgsMapService _service = null;
    private List<AgsLayer> _layerList = new List<AgsLayer>();
    private List<String> _queryList = new List<String>();

    public AgsMap(AgsMapService service, int width, int height, GeoAPI.Geometries.Envelope extent) : this((AgsDataFrame)service.DefaultDataFrame, width, height, extent) { }

    public AgsMap(AgsDataFrame dataFrame, int width, int height, GeoAPI.Geometries.Envelope extent)
	  {
      _service = dataFrame.Service as AgsMapService;
      
      DataFrame = dataFrame;
      Width = width;
      Height = height;
      Extent = extent;
    }

    public override void AddLayer(string layerId)
    {
      AddLayer(layerId, null);
    }

    public override void AddLayer(string layerId, string definitionQuery)
    {
      AgsLayer layer = DataFrame.Layers.FirstOrDefault(lyr => lyr.ID == layerId) as AgsLayer;

      if (layer == null)
      {
        throw new AgsException(String.Format("No layer with an ID of \"{0}\" exists in the dataFrame of this AgsMap.", layerId));
      }

      AddLayer(layer);
    }

    public override void AddLayer(CommonLayer layer)
    {
      AddLayer(layer, null);
    }

    public override void AddLayer(CommonLayer layer, string definitionQuery)
    {
      AgsLayer agsLayer = layer as AgsLayer;

      if (agsLayer == null)
      {
        throw new AgsException(String.Format("A {0} cannot be added to an AgsMap.", layer.GetType().Name));
      }

      AddLayer(agsLayer, definitionQuery);
    }

    public void AddLayer(AgsLayer layer)
    {
      AddLayer(layer, null);
    }

    public void AddLayer(AgsLayer layer, string definitionQuery)
    {
      if (!String.IsNullOrEmpty(definitionQuery) && layer.Type != CommonLayerType.Feature)
      {
        throw new AgsException("Definition queries are only allowed on feature layers");
      }

      _layerList.Add(layer);
      _queryList.Add(definitionQuery);
    }

    public void AddLayers(ICollection<AgsLayer> layers)
    {
      AddLayers(layers.Cast<CommonLayer>().ToList());
    }

    public override void AddLayerAndChildren(string layerId)
    {
      AgsLayer layer = DataFrame.Layers.FirstOrDefault(lyr => lyr.ID == layerId) as AgsLayer;

      if (layer == null)
      {
        throw new AgsException(String.Format("No layer with an ID of \"{0}\" exists in the dataFrame of this AgsMap.", layerId));
      }

      AddLayerAndChildren(layer);
    }

    public override void AddLayerAndChildren(CommonLayer layer)
    {
      AgsLayer agsLayer = layer as AgsLayer;

      if (agsLayer == null)
      {
        throw new AgsException(String.Format("A {0} cannot be added to an AgsMap.", layer.GetType().Name));
      }

      AddLayer(agsLayer);

      if (agsLayer.Children != null && agsLayer.Children.Count > 0)
      {
        foreach (CommonLayer child in agsLayer.Children)
        {
          AddLayerAndChildren(child);
        }
      }
    }

    public override void Clear()
    {
      _layerList.Clear();
    }

    public override string GetImageUrl()
    {
      MapImage mapImage = GetMapImage(esriImageReturnType.esriImageReturnURL);
      return mapImage.ImageURL;
    }

    public override byte[] GetImageBytes()
    {
      MapImage mapImage = GetMapImage(esriImageReturnType.esriImageReturnMimeData);
      return mapImage.ImageData;
    }

    private MapImage GetMapImage(esriImageReturnType returnType)
    {
      MapDescription mapDescription = ((AgsDataFrame)DataFrame).MapServerInfo.NewMapDescription(Extent);
      mapDescription.SetAllLayersNotVisible();

      for (int i = 0; i < _layerList.Count; ++i)
      {
        AgsLayer layer = _layerList[i];
        string query = _queryList[i];

        MapLayerInfo mapLayerInfo = layer.MapLayerInfo;

        while (mapLayerInfo != null)
        {
          LayerDescription layerDescription = mapDescription.LayerDescriptions.First(ld => ld.LayerID == mapLayerInfo.LayerID);
          layerDescription.Visible = true;

          if (!String.IsNullOrEmpty(query))
          {
            layerDescription.DefinitionExpression = query;
          }

          mapLayerInfo = mapLayerInfo.Parent;
          query = null;
        }
      }

      ImageDescription imageDescription = new ImageDescription();
      imageDescription.ImageDisplay = new ImageDisplay(Convert.ToInt32(Width * Resolution), Convert.ToInt32(Height * Resolution), DataFrame.Dpi * Resolution);
      imageDescription.ImageType = new ImageType(returnType);

      switch (ImageType)
      {
        case CommonImageType.Png:
          imageDescription.ImageType.ImageFormat = esriImageFormat.esriImagePNG32;

          if (TransparentBackground)
          {
            mapDescription.TransparentColor = mapDescription.BackgroundSymbol.Color;
          }
          break;

        default:
          imageDescription.ImageType.ImageFormat = esriImageFormat.esriImageJPG;
          break;
      }

      return _service.MapServer.ExportMapImage(mapDescription, imageDescription);
    }
  }
}
