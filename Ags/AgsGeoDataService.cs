// © 2007-2010, Applied Geographics, Inc.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AppGeo.Clients.Ags.Proxy;

namespace AppGeo.Clients.Ags
{
  public class AgsGeoDataService
  {
    private CommonHost _host = null;
    private string _name = null;

    private GeoDataServer _geoDataServer = null;
    private List<AgsDataElement> _dataElements = new List<AgsDataElement>();

    public AgsGeoDataService(string serverUrl, string service)
      : this(serverUrl, service, null, null) { }

    public AgsGeoDataService(string serverUrl, string service, string user, string password)
      : this(new AgsHost(serverUrl, user, password), service) { }

    public AgsGeoDataService(AgsHost host, string service)
    {
      ServiceDescription serviceDescription = host.ServiceDescriptions.FirstOrDefault(sd => sd.Type == "GeoDataServer" && String.Compare(sd.Name, service, true) == 0);

      if (serviceDescription == null)
      {
        throw new AgsException(String.Format("The geodata service \"{0}\" does not exist on the ArcGIS Server", service));
      }

      Host = host;
      Name = serviceDescription.Name;

      _geoDataServer = serviceDescription.GetService() as GeoDataServer;
      _geoDataServer.Credentials = host.Credentials;
      _geoDataServer.PreAuthenticate = _geoDataServer.Credentials != null;

      Reload();
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

    public List<AgsDataElement> DataElements
    {
      get
      {
        return _dataElements;
      }
    }

    public GeoDataServer GeoDataServer
    {
      get
      {
        return _geoDataServer;
      }
    }

    public void Reload()
    {
      DEBrowseOptions options = new DEBrowseOptions()
      {
        ExpandType = esriDEExpandType.esriDEExpandDescendants,
        RetrieveFullProperties = true,
        RetrieveMetadata = true
      };

      DataElement[] proxyDataElements = _geoDataServer.GetDataElements(options);

      foreach (DataElement proxyDataElement in proxyDataElements)
      {
        AgsDataElement dataElement = new AgsDataElement(this, proxyDataElement);
        _dataElements.Add(dataElement);
      }
    }
  }
}
