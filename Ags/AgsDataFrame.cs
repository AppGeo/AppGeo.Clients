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
using AppGeo.Clients;
using AppGeo.Clients.Ags.Proxy;

namespace AppGeo.Clients.Ags
{
  [Serializable]
  public class AgsDataFrame : CommonDataFrame
  {
    private MapServerInfo _mapServerInfo = null;

    public AgsDataFrame(AgsMapService service, MapServerInfo mapServerInfo, bool isDefault)
    {
      _mapServerInfo = mapServerInfo;

      Service = service;
      Name = mapServerInfo.Name;
      IsDefault = isDefault;
      
      ImageType imageType = new ImageType(esriImageFormat.esriImagePNG, esriImageReturnType.esriImageReturnMimeData);
      MapServerLegendInfo[] legendInfos = service.MapServer.GetLegendInfo(mapServerInfo.Name, null, null, imageType, null, null);

      foreach (MapLayerInfo mapLayerInfo in mapServerInfo.MapLayerInfos)
      {
        MapServerLegendInfo mapServerLegendInfo = legendInfos.FirstOrDefault(li => li.LayerID == mapLayerInfo.LayerID);
        LayerDescription layerDescription = mapServerInfo.DefaultMapDescription.LayerDescriptions.First(ld => ld.LayerID == mapLayerInfo.LayerID);

        Layers.Add(new AgsLayer(this, mapLayerInfo, mapServerLegendInfo, layerDescription.Visible));
      }

      CreateLayerHierarchy();
    }

    public MapServerInfo MapServerInfo
    {
      get
      {
        return _mapServerInfo;
      }
    }

    private void CreateLayerHierarchy()
    {
      IEnumerable<AgsLayer> layers = Layers.Cast<AgsLayer>();

      foreach (AgsLayer layer in layers)
      {
        if (layer.MapLayerInfo.Parent != null)
        {
          layer.Parent = layers.First(lyr => lyr.MapLayerInfo == layer.MapLayerInfo.Parent);

          if (layer.Parent.Children == null)
          {
            layer.Parent.Children = new List<CommonLayer>();
          }

          layer.Parent.Children.Add(layer);
        }
      }
    }

    public override CommonMap GetMap(int width, int height, GeoAPI.Geometries.Envelope extent)
    {
      return new AgsMap(this, width, height, extent);
    }
  }
}
