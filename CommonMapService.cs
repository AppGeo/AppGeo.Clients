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

namespace AppGeo.Clients
{
  [Serializable]
  public abstract class CommonMapService
  {
    private List<CommonDataFrame> _dataFrames = new List<CommonDataFrame>();
    private CommonDataFrame _defaultDataFrame = null;
    private string _name = null;

    [NonSerialized]
    private CommonHost _host = null;

    public List<CommonDataFrame> DataFrames
    {
      get
      {
        return _dataFrames;
      }
    }

    public CommonDataFrame DefaultDataFrame
    {
      get
      {
        return _defaultDataFrame;
      }
      protected set
      {
        _defaultDataFrame = value;
      }
    }

    public CommonHost Host
    {
      get
      {
        return _host;
      }
      protected set
      {
        _host = value;
      }
    }

    public List<CommonLayer> Layers
    {
      get
      {
        return DefaultDataFrame.Layers;
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

    public List<CommonLayer> TopLevelLayers
    {
      get
      {
        return DefaultDataFrame.TopLevelLayers;
      }
    }

    public abstract bool IsAvailable { get; }

    public abstract void Reload();
  }
}
