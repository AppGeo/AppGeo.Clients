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

namespace AppGeo.Clients.Ags.Proxy
{
  public partial class SimpleLineSymbol
  {
    public SimpleLineSymbol() { }

    public SimpleLineSymbol(Color color)
      : this(esriSimpleLineStyle.esriSLSSolid, color, 1) { }

    public SimpleLineSymbol(System.Drawing.Color color) 
      : this(new RgbColor(color)) { }

    public SimpleLineSymbol(Color color, double width)
      : this(esriSimpleLineStyle.esriSLSSolid, color, width) { }

    public SimpleLineSymbol(System.Drawing.Color color, double width)
      : this(new RgbColor(color), width) { }

    public SimpleLineSymbol(esriSimpleLineStyle style, Color color, double width)
    {
      Style = style;
      Color = color;
      Width = width;
    }

    public SimpleLineSymbol(esriSimpleLineStyle style, System.Drawing.Color color, double width)
      : this(style, new RgbColor(color), width) { }
  }
}
