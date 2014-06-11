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
  public partial class RgbColor
  {
    public RgbColor() { }

    public RgbColor(int r, int g, int b)
    {
      Red = Convert.ToByte(r);
      Green = Convert.ToByte(g);
      Blue = Convert.ToByte(b);
    }

    public RgbColor(int a, int r, int g, int b)
      : this(r, g, b)
    {
      AlphaValue = Convert.ToByte(a);
      AlphaValueSpecified = AlphaValue != Byte.MaxValue;
    }

    public RgbColor(System.Drawing.Color c) 
      : this(c.A, c.R, c.G, c.B) { }

    public System.Drawing.Color ToDrawingColor()
    {
      return AlphaValueSpecified ? System.Drawing.Color.FromArgb(AlphaValue, Red, Green, Blue) : System.Drawing.Color.FromArgb(Red, Green, Blue);
    }
  }
}
