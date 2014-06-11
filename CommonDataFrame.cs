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
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using GeoAPI.Geometries;

namespace AppGeo.Clients
{
  [Serializable]
  public abstract class CommonDataFrame
  {
    private int _dpi = 96;
    private List<CommonLayer> _layers = new List<CommonLayer>();
    private string _name = null;
    private bool _isDefault = false;
    private CommonMapService _service = null;

    public int Dpi
    {
      get
      {
        return _dpi;
      }
      protected set
      {
        _dpi = value;
      }
    }

    public bool IsDefault
    {
      get
      {
        return _isDefault;
      }
      protected set
      {
        _isDefault = value;
      }
    }

    public List<CommonLayer> Layers
    {
      get
      {
        return _layers;
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

    public CommonMapService Service
    {
      get
      {
        return _service;
      }
      protected set
      {
        _service = value;
      }
    }

    public List<CommonLayer> TopLevelLayers
    {
      get
      {
        return Layers.Where(lyr => lyr.Parent == null).ToList();
      }
    }

    public byte[] GetCompiledSwatchImage()
    {
      return GetCompiledSwatchImage(Color.White);
    }

    public byte[] GetCompiledSwatchImage(int tileWidth, int tileHeight)
    {
      return GetCompiledSwatchImage(tileWidth, tileHeight, Color.White);
    }

    public byte[] GetCompiledSwatchImage(Color backgroundColor)
    {
      return GetCompiledSwatchImage(20, 16, backgroundColor);
    }

    public byte[] GetCompiledSwatchImage(int tileWidth, int tileHeight, Color backgroundColor)
    {
      int maxClasses = 1;

      for (int i = 0; i < Layers.Count; ++i)
      {
        if (Layers[i].Legend != null && Layers[i].Legend.Groups.Count > 0)
        {
          int classes = 0;

          for (int j = 0; j < Layers[i].Legend.Groups.Count; ++j)
          {
            classes += Layers[i].Legend.Groups[j].Classes.Count;
          }

          if (classes > maxClasses)
          {
            maxClasses = classes;
          }
        }
      }

      Bitmap bitmap = new Bitmap(Layers.Count * tileWidth, maxClasses * tileHeight, PixelFormat.Format24bppRgb);
      Graphics graphics = Graphics.FromImage(bitmap);
      graphics.Clear(backgroundColor);

      for (int i = 0; i < Layers.Count; ++i)
      {
        float x = i * tileWidth;

        if (Layers[i].Legend != null && Layers[i].Legend.Groups.Count > 0)
        {
          int c = 0;

          for (int j = 0; j < Layers[i].Legend.Groups.Count; ++j)
          {
            for (int k = 0; k < Layers[i].Legend.Groups[j].Classes.Count; ++k)
            {
              float y = c * tileHeight;

              Bitmap swatch = new Bitmap(new MemoryStream(Layers[i].Legend.Groups[j].Classes[k].Image));
              graphics.DrawImage(swatch, x, y);

              c += 1;
            }
          }
        }
      }

      MemoryStream memoryStream = new MemoryStream();
      bitmap.Save(memoryStream, ImageFormat.Png);
      return memoryStream.ToArray();
    }

    public abstract CommonMap GetMap(int width, int height, Envelope extent);
  }
}
