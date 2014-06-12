# AppGeo.Clients

AppGeo.Clients is a .NET library which provides a common, high-level client API for interacting with 
Esri's [ArcGIS for Server](http://www.esri.com/software/arcgis/arcgisserver) and now
unsupported [ArcIMS](http://www.esri.com/software/arcgis/arcims).  It's geometry model is
OGC Simple Features.  Developers can use [GeoAPI](http://geoapi.codeplex.com/) interfaces to 
process the geometry with other compatible libraries.

## Typical Workflow

### Connect to an ArcGIS for Server map service

    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.IO;
    using System.Linq;

    using AppGeo.Clients;
    using AppGeo.Clients.Ags;
    using GeoAPI.Geometries;

    ...

    CommonHost host = new AgsHost("http://services.arcgisonline.com");
    CommonMapService mapService = host.GetMapService("Demographics/USA_1990-2000_Population_Change");

### Generate a map image

    int imageWidth = 500;
    int imageHeight = 500;
    Envelope extent = new Envelope(-8500000, -7500000, 5000000, 6000000);
    CommonLayer layer = mapService.Layers.First(o => o.Name == "Counties");

    CommonMap map = mapService.DefaultDataFrame.GetMap(imageWidth, imageHeight, extent);
    map.AddLayer(layer);
    byte[] imageData = map.GetImageBytes();

### Query a layer

    FeatureData featureData = layer.GetFeatureData("Shape,ID,Name", "ST_ABBREV = 'MA'");
    FeatureRow feature = featureData.Rows.First(o => (string)o.Values[2] == "Middlesex County");

### Draw on map graphics and retrieve the final image

    IGeometry geometry = feature.Values[0] as IGeometry;

    Pen pen = new Pen(Color.Blue, 4);
    pen.LineJoin = LineJoin.Round;

    using (MapGraphics graphics = map.GetMapGraphics())
    {
      graphics.DrawGeometry(pen, geometry);
      imageData = graphics.GetImageBytes();
    }
     
## Features

* Provides complete service and layer metadata.
* Supports map image generation, feature querying and geocoding.
* Map features contain OGC geometry through GeoAPI and NetTopologySuite.
* Automatically vectorizes parametric curves returned from ArcGIS for Server.
* MapGraphics class extends System.Drawing.Graphics to support the drawing of OGC geometry.
* Can access secured map services, supports Windows and token-based authentication in ArcGIS for Server.
* All classes in AppGeo.Clients.Ags are Serializable for caching to any object store.
* Communicates with ArcGIS for Server is via its [SOAP SDK](http://resources.arcgis.com/en/help/soap/10.2/).  
* Dumps SOAP traffic to the debug window or a trace log with the AgsDebug and AgsTrace switches in a &lt;system.diagnostics&gt; configuration.
* .NET 3.5 and above.
