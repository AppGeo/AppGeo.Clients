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
using AppGeo.Clients.ArcIms.ArcXml;

namespace AppGeo.Clients.ArcIms
{
  public class ArcImsDataFrame : CommonDataFrame
  {
    private DataFrameInfo _dataFrameInfo = null;

    public ArcImsDataFrame(ArcImsService service)
    {
      Service = service;
      Name = "Layers";
      IsDefault = true;

      foreach (LayerInfo layerInfo in service.ServiceInfo.LayerInfos)
      {
        Layers.Add(new ArcImsLayer(this, layerInfo));
      }

      CreateLayerHierarchy();
    }

    public ArcImsDataFrame(ArcImsService service, DataFrameInfo dataFrameInfo)
    {
      _dataFrameInfo = dataFrameInfo;

      Service = service;
      Dpi = service.ServiceInfo.Environment.Screen.Dpi;
      Name = dataFrameInfo.Name;
      IsDefault = dataFrameInfo.Default;

      foreach (LayerInfo layerInfo in dataFrameInfo.LayerInfos.Reverse<LayerInfo>())
      {
        Layers.Add(new ArcImsLayer(this, layerInfo));
      }

      CreateLayerHierarchy();
      List<CommonLayer> topLayers = TopLevelLayers;
      Layers.Clear();
      AddLayersFromHierarchy(topLayers);
    }

    public DataFrameInfo DataFrameInfo
    {
      get
      {
        return _dataFrameInfo;
      }
    }

    private void AddLayersFromHierarchy(List<CommonLayer> layers)
    {
      foreach (CommonLayer layer in layers)
      {
        Layers.Add(layer);

        if (layer.Children != null)
        {
          AddLayersFromHierarchy(layer.Children);
        }
      }
    }

    private void CreateLayerHierarchy()
    {
      IEnumerable<ArcImsLayer> layers = Layers.Cast<ArcImsLayer>();

      foreach (ArcImsLayer layer in layers)
      {
        if (!String.IsNullOrEmpty(layer.LayerInfo.ParentLayerID))
        {
          layer.Parent = layers.First(lyr => lyr.LayerInfo.ID == layer.LayerInfo.ParentLayerID);

          if (layer.Parent.Children == null)
          {
            layer.Parent.Children = new List<CommonLayer>();
          }

          layer.Parent.Children.Add(layer);

          if (layer.Parent.Type == CommonLayerType.Feature)
          {
            layer.Parent.Type = CommonLayerType.Annotation;
          }
        }
      }
    }

    public override CommonMap GetMap(int width, int height, GeoAPI.Geometries.Envelope extent)
    {
      return new ArcImsMap(this, width, height, extent);
    }
  }
}
