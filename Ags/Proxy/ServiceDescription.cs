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
using System.Web.Services.Protocols;

namespace AppGeo.Clients.Ags.Proxy
{
  public partial class ServiceDescription
  {
    public SoapHttpClientProtocol GetService()
    {
      switch (Type)
      {
        case "MapServer":
          return new MapServer(Url);
        case "FeatureServer":
          return new FeatureServer(Url);
        case "GeocodeServer":
          return new GeocodeServer(Url);
        //case "GeometryServer":
        //  return new GeometryServer(Url);
        case "GPServer":
          return new GPServer(Url);
        case "ImageServer":
          return new ImageServer(Url);
        case "NAServer":
          return new NAServer(Url);
        default:
          throw new NotSupportedException(String.Format("Cannot return a service of type {0}, not supported.", Type));
      }
    }
  }
}
