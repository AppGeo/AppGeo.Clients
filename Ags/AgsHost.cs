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
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using AppGeo.Clients;
using AppGeo.Clients.Ags.Proxy;
using System.Net;

namespace AppGeo.Clients.Ags
{
  [Serializable]
  public class AgsHost : CommonHost
  {
    public static AgsHost LoadFrom(string fileName)
    {
      BinaryFormatter formatter = new BinaryFormatter();
      FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
      AgsHost agsHost = formatter.Deserialize(fileStream) as AgsHost;
      fileStream.Dispose();
      return agsHost;
    }

    private ServiceDescription[] _serviceDescriptions = null;
    private AgsTokenService _tokenService = null;

    public AgsHost(string serverUrl) : this(serverUrl, null, null) { }

    public AgsHost(string serverUrl, string user, string password)
    {
			ServerUrl = serverUrl;
			User = user;
			Password = password;

      DefaultAllowAllCertificates();

      UriBuilder url = new UriBuilder(serverUrl);

      if (url.Path == "/")
      {
        url.Path = "/ArcGIS/services";
      }

      Catalog catalog = new Catalog(url.ToString());

      if (!String.IsNullOrEmpty(user) && catalog.RequiresTokens())
      {
        string tokenServiceUrl = catalog.GetTokenServiceURL();
        _tokenService = new AgsTokenService(tokenServiceUrl, user, password);
      }

      AddCredentials(catalog);

      ServerVersion = GetAgsVersion(catalog);
      _serviceDescriptions = catalog.GetServiceDescriptions();
    }

    public override List<String> AllServiceNames
    {
      get
      {
        return _serviceDescriptions.Select(s => s.Name).ToList();
      }
    }

    public void AddCredentials(AgsSoapClient client)
    {
      NetworkCredential credentials = Credentials;

      if (credentials != null)
      {
        if (_tokenService != null)
        {
          client.Url = _tokenService.GetToken().SignUrl(client.Url);
        }
        else
        {
          client.Credentials = credentials;
          client.PreAuthenticate = true;
        }
      }
    }

    public override List<String> GeocodeServiceNames
    {
      get
      {
        return _serviceDescriptions.Where(s => s.Type == "GeocodeServer").Select(s => s.Name).ToList();
      }
    }

    public override List<String> MapServiceNames
    {
      get
      {
        return _serviceDescriptions.Where(s => s.Type == "MapServer").Select(s => s.Name).ToList();
      }
    }

    public List<ServiceDescription> ServiceDescriptions
    {
      get
      {
        return new List<ServiceDescription>(_serviceDescriptions);
      }
    }

    private string GetAgsVersion(Catalog catalog)
    {
      try
      {
        switch (catalog.GetMessageVersion())
        {
          case esriArcGISVersion.esriArcGISVersion83: return "8.3";
          case esriArcGISVersion.esriArcGISVersion90: return "9.0";
          case esriArcGISVersion.esriArcGISVersion92: return "9.2";
          case esriArcGISVersion.esriArcGISVersion93: return "9.3";
          default: return "";
        }
      }
      catch (Exception ex)
      {
        string server = String.Format("ArcGIS Server \"{0}\"", ServerUrl);
        string message = "{0} could not be found on the network.";

        if (ex.Message.EndsWith("Unauthorized."))
        {
          if (catalog.Credentials == null)
          {
            message = "{0} requires a user name and password.";
          }
          else
          {
            message = "{0} did not authorize the specified user name and password.";
          }
        }

        throw new AgsException(String.Format(message, server), ex);
      }
    }

    public override CommonMapService GetMapService(string serviceName)
    {
      ServiceDescription serviceDescription = _serviceDescriptions.FirstOrDefault(sd => sd.Type == "MapServer" && String.Compare(sd.Name, serviceName, true) == 0);

      if (serviceDescription == null)
      {
        throw new AgsException(String.Format("The map service \"{0}\" does not exist on the ArcGIS Server", serviceName));
      }

      return new AgsMapService(this, serviceName);
    }

    public override CommonGeocodeService GetGeocodeService(string serviceName)
    {
      ServiceDescription serviceDescription = _serviceDescriptions.FirstOrDefault(sd => sd.Type == "GeocodeServer" && String.Compare(sd.Name, serviceName, true) == 0);

      if (serviceDescription == null)
      {
        throw new AgsException(String.Format("The geocode service \"{0}\" does not exist on the ArcGIS Server", serviceName));
      }

      return new AgsGeocodeService(this, serviceName);
    }

    public void SaveTo(string fileName)
    {
      BinaryFormatter formatter = new BinaryFormatter();
      FileStream fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write);
      formatter.Serialize(fileStream, this);
      fileStream.Dispose();
    }
  }
}
