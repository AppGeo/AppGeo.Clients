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
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using GeoAPI.Geometries;
using AppGeo.Clients;
using AppGeo.Clients.ArcIms.ArcXml;

namespace AppGeo.Clients.ArcIms
{
	public class ArcImsMap : CommonMap
	{
    private ArcImsService _service;
		private string _coordinateSystem = null;

    private LayerList _layerList = new LayerList();
    private Layers _layers = new Layers();
    private Layer _acetateLayer = new Layer("__acetate", LayerType.Acetate);
    private bool _accessImageFromHost = true;

    public Color BackgroundColor = Color.Empty;
    public bool Transparent = false;

    public ArcImsMap(ArcImsService service, int width, int height, Envelope extent) : this((ArcImsDataFrame)service.DefaultDataFrame, width, height, extent) { }

    public ArcImsMap(ArcImsDataFrame dataFrame, int width, int height, Envelope extent)
		{
			_service = dataFrame.Service as ArcImsService;
      
      DataFrame = dataFrame;
      Width = width;
      Height = height;
      Extent = extent;
		}

    public bool AccessImageFromHost
    {
      get
      {
        return _accessImageFromHost;
      }
      set
      {
        _accessImageFromHost = value;
      }
    }

		public string CoordinateSystem
		{
			get
			{
				return _coordinateSystem;
			}
			set
			{
				if (value == "")
				{
					_coordinateSystem = null;
				}
				else
				{
					_coordinateSystem = value;
				}
			}
		}

		public ArcImsService Service
		{
			get
			{
				return _service;
			}
		}

    public void AddGraphic(IGeometry shape, Symbol symbol)
    {
      AddGraphic(shape, symbol, false);
    }

    public void AddGraphic(IGeometry shape, Symbol symbol, bool inPixels)
    {
      ArcXml.Object o = new ArcXml.Object(shape, symbol);
      o.Units = inPixels ? ObjectUnits.Pixel : ObjectUnits.Database;
      _acetateLayer.Add(o);
    }

    public void AddGraphic(Text text, TextMarkerSymbol symbol)
    {
      AddGraphic(text, symbol, false);
    }

    public void AddGraphic(Text text, TextMarkerSymbol symbol, bool inPixels)
    {
      ArcXml.Object o = new ArcXml.Object(text, symbol);
      o.Units = inPixels ? ObjectUnits.Pixel : ObjectUnits.Database;
      _acetateLayer.Add(o);
    }

    public void AddGraphic(NorthArrow northArrow)
    {
      _acetateLayer.Add(northArrow);
    }

    public void AddGraphic(ScaleBar scaleBar)
    {
      _acetateLayer.Add(scaleBar);
    }

    public override void AddLayer(string layerId)
    {
      AddLayer(layerId, "");
    }

    public override void AddLayer(string layerId, string definitionQuery)
    {
      ArcImsLayer layer = DataFrame.Layers.FirstOrDefault(lyr => lyr.ID == layerId) as ArcImsLayer;

      if (layer == null)
      {
        throw new ArcImsException(String.Format("No layer with an ID of \"{0}\" exists in the dataFrame of this ArcImsMap.", layerId));
      }

      AddLayer(layer, definitionQuery);
    }

    public override void AddLayer(CommonLayer layer)
    {
      AddLayer(layer, "");
    }

    public override void AddLayer(CommonLayer layer, string definitionQuery)
    {
      ArcImsLayer arcImsLayer = layer as ArcImsLayer;

      if (arcImsLayer == null)
      {
        throw new ArcImsException(String.Format("A {0} cannot be added to an ArcImsMap.", layer.GetType().Name));
      }

      AddLayer(arcImsLayer, definitionQuery);
    }

    public void AddLayer(ArcImsLayer layer)
    {
      AddLayer(layer, null, LayerQueryMode.None);
    }

    public void AddLayer(ArcImsLayer layer, string definitionQuery)
    {
      if (String.IsNullOrEmpty(definitionQuery))
      {
        AddLayer(layer, null, LayerQueryMode.None);
      }
      else
      {
        AddLayer(layer, definitionQuery, LayerQueryMode.Definition);
      }
    }

    public void AddLayer(ArcImsLayer layer, string query, LayerQueryMode queryMode)
    {
      AddLayer(layer, query, queryMode, null);
    }

    public void AddLayer(ArcImsLayer layer, Renderer renderer)
    {
      AddLayer(layer, null, LayerQueryMode.None, renderer);
    }

    public void AddLayer(ArcImsLayer layer, string query, LayerQueryMode queryMode, Renderer renderer)
    {
      int nextLayerNo = 0;

      // layers from other services and data frames are not allowed

      if (layer.DataFrame != DataFrame)
      {
        throw new ArcImsException("The specified ArcImsLayer is not in the same service and dataframe as the ArcImsMap.");
      }

      // queries and renderers are only allow on feature layers

      SpatialQuery spatialQuery = null;

      if (!String.IsNullOrEmpty(query) && queryMode != LayerQueryMode.None)
      {
        if (layer.Type != CommonLayerType.Feature)
        {
          throw new ArcImsException("Definition and selection queries are only allowed on feature layers");
        }

        spatialQuery = new SpatialQuery(query);
      }

      if (renderer != null && layer.Type != CommonLayerType.Feature)
      {
        throw new ArcImsException("Custom renderers are only allowed on feature layers");
      }

      // add all group layers that contain this layer

      ArcImsLayer parent = layer.Parent as ArcImsLayer;

      while (parent != null)
      {
        AddLayer(new LayerDef(parent.ID));
        parent = parent.Parent as ArcImsLayer;
      }
      
      // handle queries and renderers

      LayerDef layerDef = new LayerDef(layer.ID);

      if (spatialQuery != null)
      {
        if (queryMode == LayerQueryMode.Definition)
        {
          layerDef.Query = spatialQuery;
          layerDef.Renderer = renderer;
        }

        if (queryMode == LayerQueryMode.Selection)
        {
          Layer axlLayer = new Layer(String.Format("__{0}", nextLayerNo++));
          axlLayer.Query = spatialQuery;
          axlLayer.Dataset = new Dataset(layer.ID);

          if (_service.IsArcMap)
          {
            layerDef.Renderer = renderer;
          }
          else
          {
            axlLayer.Renderer = renderer;
          }

          AddLayer(axlLayer);
        }
      }
      else
      {
        layerDef.Renderer = renderer;
      }

      AddLayer(layerDef);
    }

    public void AddLayer(LayerDef layerDef)
    {
      if (!_layerList.Contains(layerDef.ID))
      {
        _layerList.Add(layerDef);
      }
    }

    public void AddLayer(Layer layer)
    {
      _layers.Add(layer);
    }

    public void AddLayer(LayerDef layerDef, Layer layer)
    {
      _layerList.Add(layerDef);
      _layers.Add(layer);
    }

    public void AddLayers(ICollection<ArcImsLayer> layers)
    {
      AddLayers(layers.Cast<CommonLayer>().ToList());
    }

    public override void AddLayerAndChildren(string layerId)
    {
      ArcImsLayer layer = DataFrame.Layers.FirstOrDefault(lyr => lyr.ID == layerId) as ArcImsLayer;

      if (layer == null)
      {
        throw new ArcImsException(String.Format("No layer with an ID of \"{0}\" exists in the dataFrame of this ArcImsMap.", layerId));
      }

      AddLayerAndChildren(layer);
    }

    public override void AddLayerAndChildren(CommonLayer layer)
    {
      ArcImsLayer arcImsLayer = layer as ArcImsLayer;

      if (arcImsLayer == null)
      {
        throw new ArcImsException(String.Format("A {0} cannot be added to an ArcImsMap.", layer.GetType().Name));
      }

      AddLayer(arcImsLayer);

      if (arcImsLayer.Children != null && arcImsLayer.Children.Count > 0)
      {
        foreach (CommonLayer child in arcImsLayer.Children)
        {
          AddLayerAndChildren(child);
        }
      }
    }

    public override void Clear()
    {
      _layerList.Clear();
      _layers.Clear();
      _acetateLayer.Objects.Clear();
    }

    public override string GetImageUrl()
		{
			GetImage getImage = PrepareGetImage();
      ArcXml.Image image = (ArcXml.Image)_service.Send(getImage);

      string url = image.Output.Url;

      if (_accessImageFromHost)
      {
        UriBuilder hostBuilder = new UriBuilder(_service.Host.ServerUrl);
        UriBuilder urlBuilder = new UriBuilder(url);
        urlBuilder.Host = hostBuilder.Host;
        url = urlBuilder.ToString();
      }

      return url;
		}

    public override byte[] GetImageBytes()
		{
			string imageUrl = GetImageUrl();

      WebClient webClient = new WebClient();
      webClient.Credentials = _service.Host.Credentials;

			try
			{
        byte[] image = webClient.DownloadData(imageUrl);
        return image;
			}
			catch (Exception ex)
			{
				throw new ArcImsException("Unable to retrieve map image", ex);
			}
		}

		private GetImage PrepareGetImage()
		{
			// create a new GetImage request, set the extent and image size

			GetImage getImage = new GetImage();
      getImage.Properties.ImageSize.Width = Width;
      getImage.Properties.ImageSize.Height = Height;
      getImage.Properties.Envelope = VisibleExtent;

			if (Resolution != 1)
			{
				if (_service.IsArcMap)
				{
          getImage.Properties.ImageSize.Width = Convert.ToInt32(Width * Resolution);
          getImage.Properties.ImageSize.Height = Convert.ToInt32(Height * Resolution);
          getImage.Properties.ImageSize.Dpi = Convert.ToInt32(DataFrame.Dpi * Resolution);
				}
				else
				{
          getImage.Properties.ImageSize.PrintWidth = Convert.ToInt32(Width * Resolution);
          getImage.Properties.ImageSize.PrintHeight = Convert.ToInt32(Height * Resolution);
          getImage.Properties.ImageSize.ScaleSymbols = true;
				}
			}

			getImage.DataFrame = DataFrame.Name;

			// set the projection if necessary

			if (_coordinateSystem != null)
			{
				FeatureCoordSys featSys = new FeatureCoordSys();
				FilterCoordSys filtSys = new FilterCoordSys();

				string csType = _coordinateSystem.Substring(0, 6);

				if (csType == "PROJCS" || csType == "GEOGCS")
				{
					featSys.String = _coordinateSystem;
					filtSys.String = _coordinateSystem;
				}
				else
				{
					featSys.ID = _coordinateSystem;
					filtSys.ID = _coordinateSystem;
				}

				getImage.Properties.FeatureCoordSys = featSys;
				getImage.Properties.FilterCoordSys = filtSys;
			}

			// set the background color if one was specified

			if (BackgroundColor != Color.Empty)
			{
				getImage.Properties.Background = new Background(BackgroundColor);

				if (Transparent)
				{
					getImage.Properties.Background.TransparentColor = BackgroundColor;
				}
			}

			// set the image format if one was specified

			if (ImageType != CommonImageType.Default)
			{
				getImage.Properties.Output = new Output();

        switch (ImageType)
        {
          case CommonImageType.Jpg: getImage.Properties.Output.Type = ArcXml.ImageType.Jpg; break;
          case CommonImageType.Png: getImage.Properties.Output.Type = ArcXml.ImageType.Png24; break;
        }
			}

			// add layers to the request that were specified in code

			getImage.Properties.LayerList = (LayerList)_layerList.Clone();

			if (_layers.Count > 0)
			{
			  getImage.Layers = (Layers)_layers.Clone();
			}

      if (_acetateLayer.Objects != null && _acetateLayer.Objects.Count > 0)
      {
        if (getImage.Layers == null)
        {
          getImage.Layers = new Layers();
        }

        getImage.Layers.Add((Layer)_acetateLayer.Clone());
        getImage.Properties.LayerList.Add(new LayerDef(_acetateLayer.ID));
      }

			return getImage;
		}
	}
}
