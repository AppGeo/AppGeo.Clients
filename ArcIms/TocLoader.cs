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
using System.Text;
using AppGeo.Clients.ArcIms.ArcXml;

namespace AppGeo.Clients.ArcIms
{
  public class TocLoader
  {
    ArcImsService _service;
    SwatchMaker _swatchMaker;

    public TocLoader(ArcImsService service) : this(service, 16, 16) { }

    public TocLoader(ArcImsService service, int swatchWidth, int swatchHeight)
    {
      _service = service;
      _swatchMaker = new SwatchMaker(service, swatchWidth, swatchHeight);
    }

    public void LoadLayerToc(LayerInfo layer, bool includeImages)
    {
      if (layer.Type == LayerType.FeatureClass)
      {
        layer.Toc = new Toc(LoadTocGroups(layer.Renderer, _swatchMaker, includeImages));
      }
      else
      {
        layer.Toc = new Toc();
      }
    }

    public void LoadServiceTocs(bool includeImage)
    {
      foreach (ArcImsLayer layer in _service.Layers)
      {
        if (layer.Type == CommonLayerType.Feature)
        {
          LoadLayerToc(layer.LayerInfo, includeImage);
          layer.ReloadLegend();
        }
      }
    }

    private TocClass LoadTocClass(Symbol symbol, SwatchMaker swatchMaker, bool includeImage)
    {
      TocClass tocClass = new TocClass();

      if (includeImage)
      {
        tocClass.Image = swatchMaker.GetSwatchBytes(symbol);
      }

      return tocClass;
    }

    private TocClass LoadTocClass(Symbol symbol, SwatchMaker swatchMaker, string label, bool includeImage)
    {
      TocClass tocClass = LoadTocClass(symbol, swatchMaker, includeImage);
      tocClass.Label = label;
      return tocClass;
    }

    private List<TocGroup> LoadTocGroups(Renderer renderer, SwatchMaker swatchMaker, bool includeImages)
    {
      List<TocGroup> tocGroups = new List<TocGroup>();

      switch (renderer.GetType().Name)
      {
        case "SimpleRenderer":
          tocGroups.Add(new TocGroup());
          SimpleRenderer sr = (SimpleRenderer)renderer;
          tocGroups[0].Add(LoadTocClass(sr.Symbol, swatchMaker, includeImages));
          break;

        case "ValueMapRenderer":
          tocGroups.Add(new TocGroup());
          ValueMapRenderer vmr = (ValueMapRenderer)renderer;

          foreach (Classification classification in vmr.Classifications)
          {
            string label = "";

            switch (classification.GetType().Name)
            {
              case "Exact":
                Exact exact = (Exact)classification;
                label = !String.IsNullOrEmpty(exact.Label) ? exact.Label : exact.Value;
                break;

              case "Range":
                Range range = (Range)classification;
                label = !String.IsNullOrEmpty(range.Label) ? range.Label : range.Lower + " to " + range.Upper;
                break;

              case "Other":
                label = "(other)";
                break;
            }

            tocGroups[0].Add(LoadTocClass(classification.Symbol, swatchMaker, label, includeImages));
          }
          break;

        case "ScaleDependentRenderer":
          ScaleDependentRenderer scd = (ScaleDependentRenderer)renderer;
          tocGroups = LoadTocGroups(scd.Renderer, swatchMaker, includeImages);

          foreach (TocGroup tocGroup in tocGroups)
          {
            if (!String.IsNullOrEmpty(scd.Lower))
            {
              tocGroup.MinScale = Convert.ToDouble(scd.Lower);
            }
            if (!String.IsNullOrEmpty(scd.Upper))
            {
              tocGroup.MaxScale = Convert.ToDouble(scd.Upper);
            }
          }
          break;

        case "GroupRenderer":
          foreach (Renderer subRenderer in (GroupRenderer)renderer)
          {
            List<TocGroup> subGroups = LoadTocGroups(subRenderer, swatchMaker, includeImages);

            foreach (TocGroup tocGroup in subGroups)
            {
              tocGroups.Add(tocGroup);
            }
          }
          break;
      }

      return tocGroups;
    }
  }
}
