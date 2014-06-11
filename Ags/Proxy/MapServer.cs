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

namespace AppGeo.Clients.Ags.Proxy
{
  [Serializable]
  public partial class MapServer
  {
    public MapServer(string url)
    {
      Url = url;
    }

    public MapServerDefinition GetMapServerDefinition()
    {
      ImageType imageType = new ImageType();
      imageType.ImageFormat = esriImageFormat.esriImagePNG24;
      imageType.ImageReturnType = esriImageReturnType.esriImageReturnMimeData;

      MapServerDefinition def = new MapServerDefinition(Url);
      int mapCount = GetMapCount();

      for (int i = 0; i < mapCount; ++i)
      {
        string mapName = GetMapName(i);
        MapServerInfo mapServerInfo = GetServerInfo(mapName);

        // create parent/child layer relationships

        foreach (MapLayerInfo mapLayerInfo in mapServerInfo.MapLayerInfos)
        {
          if (mapLayerInfo.ParentLayerID >= 0)
          {
            MapLayerInfo parent = mapServerInfo.MapLayerInfos.First(lyr => lyr.LayerID == mapLayerInfo.ParentLayerID);
            mapLayerInfo.Parent = parent;

            if (parent.Children == null)
            {
              parent.Children = new List<MapLayerInfo>();
            }

            parent.Children.Add(mapLayerInfo);
          }
        }

        def.MapServerInfos.Add(mapServerInfo);
      }

      return def;
    }
  }
}
