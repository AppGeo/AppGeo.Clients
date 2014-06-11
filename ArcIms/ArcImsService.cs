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
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using GeoAPI.Geometries;
using AppGeo.Clients;
using AppGeo.Clients.ArcIms.ArcXml;

namespace AppGeo.Clients.ArcIms
{
	public class ArcImsService : CommonMapService
	{
    private ServiceInfo _serviceInfo;

    public ArcImsService(string serverUrl, string service) 
      : this(serverUrl, service, null, null) { }

    public ArcImsService(string serverUrl, string service, string user, string password) 
      : this(new ArcImsHost(serverUrl, user, password), service) { }

    public ArcImsService(string serverUrl, string servletPath, string service) 
      : this(serverUrl, servletPath, service, null, null) { }

    public ArcImsService(string serverUrl, string servletPath, string service, string user, string password) 
      : this(new ArcImsHost(serverUrl, servletPath, user, password), service) { }

    public ArcImsService(ArcImsHost host, string service)
    {
      Host = host;
      Name = service;

      Reload();
		}

		public ServiceInfo ServiceInfo
		{
			get
			{
				return _serviceInfo;
			}
		}

    public override bool IsAvailable
		{
			get
			{
				try
				{
          GetImage getImage = new GetImage();
          getImage.Properties.ImageSize.Width = 1;
          getImage.Properties.ImageSize.Height = 1;
          getImage.Properties.Envelope = new Envelope(new Coordinate(0, 0), new Coordinate(1, 1));
          Send(getImage);

          return true;
        }
				catch
				{
					return false;
				}
			}
		}

		public bool IsArcMap
		{
			get
			{
				return _serviceInfo.LayoutInfo != null;
			}
		}

		public void LoadToc(bool includeImages)
		{
			if (IsArcMap)
			{
				throw new ArcImsException("Attempted to run the LoadToc method on an ArcMap service.  This method is only valid for Image services.");
			}

      TocLoader tocLoader = new TocLoader(this);
      tocLoader.LoadServiceTocs(includeImages);
		}

    public override void Reload()
    {
      DataFrames.Clear();
      DefaultDataFrame = null;

      _serviceInfo = (ServiceInfo)Send(new GetServiceInfo());

      if (IsArcMap)
      {
        foreach (DataFrameInfo dataFrameInfo in _serviceInfo.DataFrameInfos)
        {
          ArcImsDataFrame dataFrame = new ArcImsDataFrame(this, dataFrameInfo);
          DataFrames.Add(dataFrame);

          if (dataFrame.IsDefault)
          {
            DefaultDataFrame = dataFrame;
          }
        }

        if (DefaultDataFrame == null && DataFrames.Count > 0)
        {
          DefaultDataFrame = DataFrames[0];
        }
      }
      else
      {
        ArcImsDataFrame dataFrame = new ArcImsDataFrame(this);
        DataFrames.Add(dataFrame);
        DefaultDataFrame = dataFrame;
      }
    }

    public Response Send(Request request)
    {
      ArcImsHost host = Host as ArcImsHost;
      return host.Send(this, request);
    }
  }
}
