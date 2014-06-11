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

namespace AppGeo.Clients.ArcIms.ArcXml
{
  [Serializable]
  public abstract class RasterClassification : ICloneable
  {
    private Color _color = Color.Empty;
    private string _label = null;
    private double _transparency = 1;

    public Color Color
    {
      get
      {
        return _color;
      }
      set
      {
        _color = value;
      }
    }

    public string Label
    {
      get
      {
        return _label;
      }
      set
      {
        _label = value;
      }
    }

    public double Transparency
    {
      get
      {
        return _transparency;
      }
      set
      {
        _transparency = value;
      }
    }

    public abstract object Clone();
  }
}

