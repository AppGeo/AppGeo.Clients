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
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace AppGeo.Clients.Ags.Proxy
{
  public partial class MapDescription
  {
    public void AddCustomGraphics(GraphicElement element)
    {
      GraphicElement[] customGraphics = CustomGraphics;

      if (customGraphics == null)
      {
        customGraphics = new GraphicElement[1];
      }
      else
      {
        Array.Resize<GraphicElement>(ref customGraphics, customGraphics.Length + 1);
      }

      customGraphics[customGraphics.Length - 1] = element;
      CustomGraphics = customGraphics;
    }

    public void AddCustomGraphics(IList<GraphicElement> elements)
    {
      GraphicElement[] customGraphics = CustomGraphics;
      
      if (customGraphics == null)
      {
        customGraphics = new GraphicElement[elements.Count];
      }
      else
      {
        Array.Resize<GraphicElement>(ref customGraphics, customGraphics.Length + elements.Count);
      }

      elements.CopyTo(customGraphics, customGraphics.Length - elements.Count);
      CustomGraphics = customGraphics;
    }

    public MapDescription Copy()
    {
      BinaryFormatter formatter = new BinaryFormatter();
      MemoryStream stream = new MemoryStream();
      formatter.Serialize(stream, this);
      stream.Seek(0, SeekOrigin.Begin);
      return (MapDescription)formatter.Deserialize(stream);
    }

    public void SetAllLayersNotVisible()
    {
      for (int i = 0; i < LayerDescriptions.Length; ++i)
      {
        LayerDescriptions[i].Visible = false;
      }
    }

    public void SetAllLayersVisible()
    {
      for (int i = 0; i < LayerDescriptions.Length; ++i)
      {
        LayerDescriptions[i].Visible = true;
      }
    }
  }
}
