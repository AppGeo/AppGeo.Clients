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
using System.Drawing;
using System.IO;
using AppGeo.Clients;
using AppGeo.Clients.Ags.Proxy;

namespace AppGeo.Clients.Ags
{
  [Serializable]
  public class AgsLegendClass : CommonLegendClass
  {
    private MapServerLegendClass _mapServerLegendClass = null;

    public AgsLegendClass(MapServerLegendClass mapServerLegendClass)
    {
      _mapServerLegendClass = mapServerLegendClass;

      Label = _mapServerLegendClass.Label;

      Bitmap bitmap = new Bitmap(new MemoryStream(mapServerLegendClass.SymbolImage.ImageData));
      bool imageIsTransparent = true;

      for (int row = 0; row < bitmap.Width; ++row)
      {
        for (int col = 0; col < bitmap.Height; ++col)
        {
          if (bitmap.GetPixel(row, col).A > 0)
          {
            imageIsTransparent = false;
            break;
          }
        }

        if (!imageIsTransparent)
        {
          break;
        }
      }

      ImageIsTransparent = imageIsTransparent;
    }

    public override byte[] Image
    {
      get
      {
        return _mapServerLegendClass.SymbolImage.ImageData;
      }
    }

    public MapServerLegendClass MapServerLegendClass
    {
      get
      {
        return _mapServerLegendClass;
      }
    }
  }
}
