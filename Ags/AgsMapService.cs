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

namespace AppGeo.Clients.Ags
{
  [Serializable]
  public class AgsMapService : CommonMapService
  {
    public static AgsMapService LoadFrom(string fileName, AgsHost host)
    {
      BinaryFormatter formatter = new BinaryFormatter();
      FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
      AgsMapService agsMapService = formatter.Deserialize(fileStream) as AgsMapService;
      agsMapService.Host = host;
      fileStream.Dispose();
      return agsMapService;
    }

    private ServiceDescription _serviceDescription = null;
    private MapServerDefinition _mapServerDefinition = null;

    public AgsMapService(string serverUrl, string service) 
      : this(serverUrl, service, null, null) { }

    public AgsMapService(string serverUrl, string service, string user, string password) 
      : this(new AgsHost(serverUrl, user, password), service) { }

    public AgsMapService(AgsHost host, string service)
    {
      _serviceDescription = host.ServiceDescriptions.FirstOrDefault(sd => sd.Type == "MapServer" && String.Compare(sd.Name, service, true) == 0);

      if (_serviceDescription == null)
      {
        throw new AgsException(String.Format("The map service \"{0}\" does not exist on the ArcGIS Server", service));
      }

      Host = host;
      Name = _serviceDescription.Name;
      
      Reload();
		}

    public override bool IsAvailable
    {
      get 
      {
        try
        {
          MapServer.GetMapCount();
          return true;
        }
        catch
        {
          return false;
        }
      }
    }

    public MapServer MapServer
    {
      get
      {
        AgsHost host = (AgsHost)Host;
        MapServer mapServer = _serviceDescription.GetService() as MapServer;
        host.AddCredentials(mapServer);
        return mapServer;
      }
    }

    public MapServerDefinition MapServerDefinition
    {
      get
      {
        return _mapServerDefinition;
      }
    }

    public override void Reload()
    {
      DataFrames.Clear();
      _mapServerDefinition = null;

      try
      {
        _mapServerDefinition = MapServer.GetMapServerDefinition();
        string defaultName = MapServer.GetDefaultMapName();

        foreach (MapServerInfo mapServerInfo in _mapServerDefinition.MapServerInfos)
        {
          bool isDefault = mapServerInfo.Name == defaultName;
          AgsDataFrame dataFrame = new AgsDataFrame(this, mapServerInfo, isDefault);

          DataFrames.Add(dataFrame);

          if (isDefault)
          {
            DefaultDataFrame = dataFrame;
          }
        }
      }
      catch (Exception ex)
      {
        throw new AgsException("Unable to communicate with the ArcGIS Server", ex);
      }
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
