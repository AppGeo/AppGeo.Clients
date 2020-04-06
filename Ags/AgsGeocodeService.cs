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
using AppGeo.Clients;
using AppGeo.Clients.Ags.Proxy;

namespace AppGeo.Clients.Ags
{
  public class AgsGeocodeService : CommonGeocodeService
  {
    private ServiceDescription _serviceDescription = null;
    private PropertySet _locatorProperties = null;

    public AgsGeocodeService(string serverUrl, string service) 
      : this(serverUrl, service, null, null) { }

    public AgsGeocodeService(string serverUrl, string service, string user, string password) 
      : this(new AgsHost(serverUrl, user, password), service) { }

    public AgsGeocodeService(AgsHost host, string service)
    {
      _serviceDescription = host.ServiceDescriptions.FirstOrDefault(sd => sd.Type == "GeocodeServer" && String.Compare(sd.Name, service, true) == 0);

      if (_serviceDescription == null)
      {
        throw new AgsException(String.Format("The geocode service \"{0}\" does not exist on the ArcGIS Server", service));
      }

      Host = host;
      Name = _serviceDescription.Name;

      Reload();
    }

    public override double EndOffset
    {
      get 
      {
        PropertySetProperty prop = _locatorProperties.PropertyArray.FirstOrDefault(o => o.Key == "EndOffset");
        return prop == null ? 0 : Convert.ToDouble(prop.Value);
      }
    }

    public override double SideOffset
    {
      get
      {
        PropertySetProperty prop = _locatorProperties.PropertyArray.FirstOrDefault(o => o.Key == "SideOffset");
        return prop == null ? 0 : Convert.ToDouble(prop.Value);
      }
    }

    public override string SideOffsetUnits
    {
      get 
      {
        PropertySetProperty prop = _locatorProperties.PropertyArray.FirstOrDefault(o => o.Key == "SideOffsetUnits");
        return prop == null ? "unknown" : prop.Value.ToString();
      }
    }

    public GeocodeServer GeocodeServer
    {
      get
      {
        AgsHost host = (AgsHost)Host;
        GeocodeServer geocodeServer = _serviceDescription.GetService() as GeocodeServer;
        host.AddCredentials(geocodeServer);
        return geocodeServer;
      }
    }

    public override bool IsAvailable
    {
      get
      {
        try
        {
          GeocodeServer.GetAddressFields();
          return true;
        }
        catch
        {
          return false;
        }
      }
    }

    private PropertySet ConvertAddressValuesToProperties(AddressValue[] values)
    {
      return new PropertySet(values.Select(o => new PropertySetProperty(o.Name, o.Value)));
    }

    public override List<MatchedAddress> FindAddressCandidates(params AddressValue[] values)
    {
      ValidateAddressValues(values);
      RecordSet recordSet = GeocodeServer.FindAddressCandidates(ConvertAddressValuesToProperties(values), GetModifiedLocatorProperties());

      List<MatchedAddress> matchedAddresses = new List<MatchedAddress>();

      if (recordSet != null && recordSet.Records != null)
      {
        var select = recordSet.Fields.FieldArray.Select((field, index) => new { field, index });

        var result = select.FirstOrDefault(o => o.field.Name == "Match_addr");
        int addressIndex = result == null ? -1 : result.index;

        result = select.FirstOrDefault(o => o.field.Name == "Score");
        int scoreIndex = result == null ? -1 : result.index;

        result = select.FirstOrDefault(o => o.field.Name == "Shape");
        int shapeIndex = result == null ? -1 : result.index;

        foreach (Record record in recordSet.Records)
        {
          MatchedAddress matchedAddress = new MatchedAddress();

          if (addressIndex > -1)
          {
            matchedAddress.Address = record.Values[addressIndex].ToString();
          }

          if (scoreIndex > -1)
          {
            matchedAddress.Score = Convert.ToInt32(record.Values[scoreIndex]);
          }

          if (shapeIndex > -1)
          {
            matchedAddress.Location = ((AppGeo.Clients.Ags.Proxy.Geometry)record.Values[shapeIndex]).ToCommon().Centroid.Coordinate;
          }

          matchedAddresses.Add(matchedAddress);
        }
      }

      return matchedAddresses;
    }

    public override MatchedAddress GeocodeAddress(params AddressValue[] values)
    {
      ValidateAddressValues(values);
      PropertySet propertySet = GeocodeServer.GeocodeAddress(ConvertAddressValuesToProperties(values), GetModifiedLocatorProperties());

      MatchedAddress matchedAddress = null;

      if (propertySet != null && propertySet.PropertyArray != null || propertySet.PropertyArray.Length > 0)
      {
        PropertySetProperty prop = propertySet.PropertyArray.FirstOrDefault(o => o.Key == "Status");

        if (prop != null && prop.Value.ToString() == "M")
        {
          matchedAddress = new MatchedAddress();

          prop = propertySet.PropertyArray.FirstOrDefault(o => o.Key == "Match_addr");

          if (prop != null)
          {
            matchedAddress.Address = prop.Value.ToString();
          }

          prop = propertySet.PropertyArray.FirstOrDefault(o => o.Key == "Score");

          if (prop != null)
          {
            matchedAddress.Score = Convert.ToInt32(prop.Value);
          }

          prop = propertySet.PropertyArray.FirstOrDefault(o => o.Key == "Shape");

          if (prop != null)
          {
            matchedAddress.Location = ((AppGeo.Clients.Ags.Proxy.Geometry)prop.Value).ToCommon().Centroid.Coordinate;
          }
        }
      }

      return matchedAddress;
    }

    private PropertySet GetModifiedLocatorProperties()
    {
      PropertySet propertySet = _locatorProperties.Copy();

      PropertySetProperty prop = propertySet.PropertyArray.FirstOrDefault(o => o.Key == "MinimumMatchScore");

      if (prop != null)
      {
        prop.Value = MinimumScore;
      }

      prop = propertySet.PropertyArray.FirstOrDefault(o => o.Key == "SpellingSensitivity");

      if (prop != null)
      {
        prop.Value = SpellingSensitivity;
      }

      if (!String.IsNullOrEmpty(CoordinateSystem))
      {
        List<PropertySetProperty> propList = new List<PropertySetProperty>(propertySet.PropertyArray);
        propList.Add(new PropertySetProperty("OutputSpatialReference", SpatialReference.Create(CoordinateSystem)));
        propertySet.PropertyArray = propList.ToArray();
      }

      return propertySet;
    }

    public override void Reload()
    {
      Fields fields = GeocodeServer.GetAddressFields();
      AddressFields = fields.FieldArray.Select(o => new AgsField(o)).Cast<CommonField>().ToList();

      _locatorProperties = GeocodeServer.GetLocatorProperties();

      PropertySetProperty prop = _locatorProperties.PropertyArray.FirstOrDefault(o => o.Key == "MinimumMatchScore");

      if (prop != null)
      {
        MinimumScore = Convert.ToInt32(prop.Value);
      }

      prop = _locatorProperties.PropertyArray.FirstOrDefault(o => o.Key == "SpellingSensitivity");

      if (prop != null)
      {
        SpellingSensitivity = Convert.ToInt32(prop.Value);
      }
    }
  }
}
