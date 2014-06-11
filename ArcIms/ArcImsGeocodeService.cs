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
using System.Linq;
using GeoAPI.Geometries;
using AppGeo.Clients;
using AppGeo.Clients.ArcIms.ArcXml;

namespace AppGeo.Clients.ArcIms
{
  public class ArcImsGeocodeService : CommonGeocodeService
  {
    private ServiceInfo _serviceInfo;
    private LayerInfo _layerInfo;
    private GcStyle _gcStyle;

    private string _layerName = null;

    private int _maximumCandidates = 20;

    public ArcImsGeocodeService(string serverUrl, string service) 
      : this(serverUrl, service, null, null) { }

    public ArcImsGeocodeService(string serverUrl, string service, string user, string password) 
      : this(new ArcImsHost(serverUrl, user, password), service) { }

    public ArcImsGeocodeService(string serverUrl, string servletPath, string service) 
      : this(serverUrl, servletPath, service, null, null) { }

    public ArcImsGeocodeService(string serverUrl, string servletPath, string service, string user, string password) 
      : this(new ArcImsHost(serverUrl, servletPath, user, password), service) { }

    public ArcImsGeocodeService(ArcImsHost host, string service) 
      : this(host, service, null) { }

    public ArcImsGeocodeService(ArcImsHost host, string service, string layerName)
    {
      Host = host;
      Name = service;
      _layerName = layerName;

      Reload();

      SpellingSensitivity = _gcStyle.SpellingSensitivity;
    }

    public override double EndOffset
    {
      get 
      {
        return _gcStyle.EndOffset;
      }
    }

    public GcStyle GcStyle
    {
      get
      {
        return _gcStyle;
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
          getImage.Properties.Envelope = new Envelope(new Coordinate(0, 0),  new Coordinate(1, 1));
          Send(getImage);

          return true;
        }
        catch
        {
          return false;
        }
      }
    }

    public LayerInfo LayerInfo
    {
      get
      {
        return _layerInfo;
      }
    }

    public int MaximumCandidates
    {
      get
      {
        return _maximumCandidates;
      }
      set
      {
        if (value < 1)
        {
          throw new ArcImsException("MaximumCandidates must be greater than or equal to 1");
        }

        _maximumCandidates = value;
      }
    }

    public override double SideOffset
    {
      get
      {
        return _gcStyle.SideOffset;
      }
    }

    public override string SideOffsetUnits
    {
      get 
      {
        switch (_gcStyle.SideOffsetUnits)
        {
          case Units.Feet: return "Foot";
          case Units.Meters: return "Meter";
          case Units.Degrees: return "Degree";
        }

        return "unknown";
      }
    }

		public ServiceInfo ServiceInfo
		{
			get
			{
				return _serviceInfo;
			}
		}

    private List<MatchedAddress> ConvertToMatchedAddresses(Geocode geocode)
    {
      List<MatchedAddress> matchedAddresses = new List<MatchedAddress>();

      if (geocode.GcCount != null && geocode.GcCount.Count > 0)
      {
        foreach (Feature feature in geocode.Features)
        {
          MatchedAddress matchedAddress = new MatchedAddress();

          foreach (Field field in feature.Fields)
          {
            switch (field.Name)
            {
              case "ADDRESSFOUND":
                matchedAddress.Address = field.FieldValue.ValueString;
                break;

              case "SCORE":
                matchedAddress.Score = Convert.ToInt32(field.FieldValue.ValueString);
                break;

              case "SHAPEFIELD":
                matchedAddress.Location = field.FieldValue.Point.Coordinate;
                break;
            }

            matchedAddresses.Add(matchedAddress);
          }
        }
      }

      return matchedAddresses;
    }

    public override List<MatchedAddress> FindAddressCandidates(params AddressValue[] values)
    {
      ValidateAddressValues(values);
      return ConvertToMatchedAddresses(Geocode(values, _maximumCandidates));
    }

    private Geocode Geocode(AddressValue[] values, int maxCandidates)
    {
      GetGeocode getGeocode = new GetGeocode();
      getGeocode.Layer.ID = _layerInfo.ID;
      getGeocode.MaxCandidates = maxCandidates;
      getGeocode.MinScore = MinimumScore;
      getGeocode.SpellingSensitivity = SpellingSensitivity;

      if (!String.IsNullOrEmpty(CoordinateSystem))
      {
        getGeocode.FeatureCoordSys = new FeatureCoordSys(CoordinateSystem);
      }

      foreach (AddressValue addressValue in values)
      {
        getGeocode.Address.GcTags.Add(new GcTag(addressValue.Name, addressValue.Value));
      }

      return (Geocode)Send(getGeocode);
    }

    public override MatchedAddress GeocodeAddress(params AddressValue[] values)
    {
      ValidateAddressValues(values);
      List<MatchedAddress> matchedAddresses = ConvertToMatchedAddresses(Geocode(values, 1));
      return matchedAddresses.Count == 0 ? null : matchedAddresses[0];
    }

    public override void Reload()
    {
      _serviceInfo = (ServiceInfo)Send(new GetServiceInfo(true));

      if (_serviceInfo.LayerInfos == null || _serviceInfo.LayerInfos.Count == 0)
      {
        throw new ArcImsException(String.Format("The service \"{0}\" is not configured for geocoding on the ArcIMS server", Name));
      }

      if (String.IsNullOrEmpty(_layerName))
      {
        _layerInfo = _serviceInfo.LayerInfos[0];
      }
      else
      {
        _layerInfo = _serviceInfo.LayerInfos.FirstOrDefault(o => o.Name == _layerName);

        if (_layerInfo == null)
        {
          throw new ArcImsException(String.Format("The layer \"{0}\" is not configured for geocoding in the service \"{1}\" on the ArcIMS server", _layerName, Name));
        }
      }

      _gcStyle = _layerInfo.Extensions.First(o => o.GcStyle != null).GcStyle;
      AddressFields = _gcStyle.GcInputs.Select(o => new ArcImsField(o)).Cast<CommonField>().ToList();
    }

    public Response Send(Request request)
    {
      ArcImsHost host = Host as ArcImsHost;
      return host.Send(this, request);
    }
  }
}
