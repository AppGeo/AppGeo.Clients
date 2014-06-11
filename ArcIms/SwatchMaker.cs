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
using GeoAPI.Geometries;
using NetTopologySuite.Geometries;
using AppGeo.Clients.ArcIms.ArcXml;

namespace AppGeo.Clients.ArcIms
{
	public class SwatchMaker
	{
		public static string GetSwatchName(ArcImsService service, string layerID)
		{
      string host = service.Host.ServerUrl.Replace("http://", "").Replace("https://", "").Replace(":", "_");
      return host + "_" + service.Name + "_" + layerID.Replace(" ", "_");
		}

		public static string GetSwatchName(ArcImsService service, string layerID, int valueIndex)
		{
			return GetSwatchName(service, layerID) + "_" + valueIndex.ToString();
		}

		public Color BackgroundColor = Color.White;

		private ArcImsService _service;
		private ArcImsMap _map;
		private int _width;
		private int _height;
		private int _margin = 2;

		public SwatchMaker(ArcImsService service, int swatchWidth, int swatchHeight)
		{
			_service = service;

      _map = new ArcImsMap(_service, swatchWidth, swatchHeight, new Envelope(new Coordinate(0, 0), new Coordinate(swatchWidth, swatchHeight)));
      _map.ImageType = CommonImageType.Png;
      
      _width = swatchWidth;
			_height = swatchHeight;
		}

		public Bitmap GetSwatch(Symbol symbol)
		{
      MemoryStream memoryStream = new MemoryStream(GetSwatchBytes(symbol));
      Bitmap bitmap = new Bitmap(memoryStream);
			return bitmap;
		}

		public byte[] GetSwatchBytes(Symbol symbol)
		{
			PrepareMap(symbol);
			return _map.GetImageBytes();
		}

		public Bitmap[] GetSwatches(ValueMapRenderer renderer)
		{
      List<Bitmap> swatches = new List<Bitmap>();

			foreach (Classification classification in renderer.Classifications)
			{
        swatches.Add(GetSwatch(classification.Symbol));
			}

			return swatches.ToArray();
		}

		private void PrepareMap(Symbol symbol)
		{
			_map.Clear();

			Layer layer = new Layer("__swatch", LayerType.Acetate);

			switch (symbol.GetType().Name)
			{
				case "SimplePolygonSymbol":
				case "GradientFillSymbol":
				case "RasterFillSymbol":
          double minx = _margin + 0.4999;
          double miny = _margin + 0.4999;
          double maxx = _width - _margin - 0.4999;
          double maxy = _height - _margin - 0.4999;

          IPolygon polygon = new Polygon(new LinearRing(new Coordinate[] {
            new Coordinate(minx, miny),
            new Coordinate(minx, maxy),
            new Coordinate(maxx, maxy),
            new Coordinate(maxx, miny),
            new Coordinate(minx, miny)
          }));

					layer.Add(polygon, symbol);
					break;

				case "SimpleLineSymbol":
				case "HashLineSymbol":
          ILineString lineString = new LineString(new Coordinate[] {
            new Coordinate(_margin, _height / 2),
            new Coordinate(_width - _margin - 1, _height / 2)
          });
          layer.Add(lineString, symbol);
					break;

				case "SimpleMarkerSymbol":
				case "TrueTypeMarkerSymbol":
				case "RasterMarkerSymbol":
          IPoint p = new NetTopologySuite.Geometries.Point(_width / 2, _height / 2);
					layer.Add(p, symbol);
					break;
			}

			_map.BackgroundColor = BackgroundColor;

			if (layer.Objects != null)
			{
				_map.AddLayer(layer);
			}
		}

		public void WriteSwatch(Symbol symbol, string directory, string layerID)
		{
			string outFileName = directory + "\\" + GetSwatchName(_service, layerID) + ".png";
			WriteSwatch(symbol, outFileName);
		}

		public void WriteSwatch(Symbol symbol, string outFileName)
		{
			if (File.Exists(outFileName))
			{
				File.Delete(outFileName);
			}

			FileStream fs = new FileStream(outFileName, FileMode.CreateNew, FileAccess.Write);
			WriteSwatch(symbol, fs);
			fs.Close();
		}

		public void WriteSwatch(Symbol symbol, Stream stream)
		{
			Bitmap swatch = GetSwatch(symbol);
			swatch.Save(stream, swatch.RawFormat);
		}

		public void WriteSwatches(ValueMapRenderer renderer, string directory, string layerID)
		{
			for (int i = 0; i < renderer.Classifications.Count; ++i)
			{
				string outFileName = directory + "\\" + GetSwatchName(_service, layerID, i) + ".png";
        WriteSwatch(renderer.Classifications[i].Symbol, outFileName);
			}
		}
	}
}
