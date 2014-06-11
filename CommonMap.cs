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
using System.IO;
using GeoAPI.Geometries;
using AppGeo.Clients.Transform;

namespace AppGeo.Clients
{
  public abstract class CommonMap
  {
    private CommonDataFrame _dataFrame = null;
    private Envelope _extent = new Envelope();
    private int _height = 0;
    private CommonImageType _imageType = CommonImageType.Default;
    private double _resolution = 1;
    private int _width = 0;

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

    public Envelope Extent
    {
      get
      {
        return _extent;
      }
      set
      {
        if (value == null || value.IsNull)
        {
          throw new ArgumentException("Extent cannot be set to null");
        }

        _extent = value;
      }
    }

    public int Height
    {
      get
      {
        return _height;
      }
      set
      {
        if (value <= 0)
        {
          throw new ArgumentException("Height must be greater than zero");
        }

        _height = value;
      }
    }

    public CommonImageType ImageType
    {
      get
      {
        return _imageType;
      }
      set
      {
        _imageType = value;
      }
    }

    public double PixelSize
    {
      get
      {
        return VisibleExtent.Width / _width;
      }
    }

    public double Resolution
    {
      get
      {
        return _resolution;
      }
      set
      {
        if (value <= 0)
        {
          throw new ArgumentException("Resolution must be greater than zero");
        }

        _resolution = value;
      }
    }

    public bool TransparentBackground { get; set; }

    public AffineTransformation Transform
    {
      get
      {
        return new AffineTransformation(_width, _height, _extent);
      }
    }

    public Envelope VisibleExtent
    {
      get
      {
        if (_extent == null || _extent.IsNull)
        {
          return new Envelope();
        }
        else
        {
          Envelope visibleExtent = _extent;
          visibleExtent.Reaspect(_width, _height);
          return visibleExtent;
        }
      }
    }

    public int Width
    {
      get
      {
        return _width;
      }
      set
      {
        if (value <= 0)
        {
          throw new ArgumentException("Width must be greater than zero");
        }

        _width = value;
      }
    }

    public abstract void AddLayer(string id);

    public abstract void AddLayer(string id, string definitionQuery);

    public abstract void AddLayer(CommonLayer layer);

    public abstract void AddLayer(CommonLayer layer, string definitionQuery);

    public void AddLayers(ICollection<String> layerIds)
    {
      foreach (string layerId in layerIds)
      {
        AddLayer(layerId);
      }
    }

    public void AddLayers(ICollection<CommonLayer> layers)
    {
      foreach (CommonLayer layer in layers)
      {
        AddLayer(layer);
      }
    }

    public abstract void AddLayerAndChildren(string id);

    public abstract void AddLayerAndChildren(CommonLayer layer);

    public void AddLayersAndChildren(ICollection<String> layerIds)
    {
      foreach (string layerId in layerIds)
      {
        AddLayerAndChildren(layerId);
      }
    }

    public void AddLayersAndChildren(ICollection<CommonLayer> layers)
    {
      foreach (CommonLayer layer in layers)
      {
        AddLayerAndChildren(layer);
      }
    }

    public abstract void Clear();

    public abstract string GetImageUrl();

    public abstract byte[] GetImageBytes();

    public MapGraphics GetMapGraphics()
    {
      Bitmap bitmap = new Bitmap(new MemoryStream(GetImageBytes()));
      return MapGraphics.FromImage(bitmap, _extent);
    }
  }

  public enum CommonImageType
  {
    Default,
    Png,
    Jpg
  }
}
