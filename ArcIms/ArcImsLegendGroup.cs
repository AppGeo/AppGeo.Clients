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
  public class ArcImsLegendGroup : CommonLegendGroup
  {
    private TocGroup _tocGroup = null;

    public ArcImsLegendGroup(TocGroup tocGroup)
    {
      _tocGroup = tocGroup;

      Heading = _tocGroup.Heading;

      foreach (TocClass tocClass in tocGroup)
      {
        Classes.Add(new ArcImsLegendClass(tocClass));
      }
    }

    public TocGroup TocGroup
    {
      get
      {
        return _tocGroup;
      }
    }
  }
}
