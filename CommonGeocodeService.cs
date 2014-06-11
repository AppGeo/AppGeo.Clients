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
using System.Runtime.Serialization;
using GeoAPI.Geometries;

namespace AppGeo.Clients
{
  public abstract class CommonGeocodeService
  {
    private CommonHost _host = null;
    private string _name = null;
    private List<CommonField> _addressFields = null;

    private int _spellingSensitivity = 80;
    private int _minimumScore = 60;
    private string _coordinateSystem = null;

    public List<CommonField> AddressFields
    {
      get
      {
        return _addressFields;
      }
      protected set
      {
        _addressFields = value;
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
        _coordinateSystem = value;
      }
    }

    public abstract double EndOffset { get; }

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

    public abstract bool IsAvailable { get; }

    public int MinimumScore
    {
      get
      {
        return _minimumScore;
      }
      set
      {
        if (!(0 <= value && value <= 100))
        {
          throw new ArgumentException("MinimumScore must be in the range of 0 to 100");
        }

        _minimumScore = value;
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

    public abstract double SideOffset { get; }

    public abstract string SideOffsetUnits { get; }

    public int SpellingSensitivity
    {
      get
      {
        return _spellingSensitivity;
      }
      set
      {
        if (!(0 <= value && value <= 100))
        {
          throw new ArgumentException("SpellingSensitivity must be in the range of 0 to 100");
        }

        _spellingSensitivity = value;
      }
    }

    public List<MatchedAddress> FindAddressCandidates(params string[] values)
    {
      return FindAddressCandidates(GetAddressValues(values));
    }

    public List<MatchedAddress> FindAddressCandidates(List<AddressValue> values)
    {
      return FindAddressCandidates(values.ToArray());
    }

    public abstract List<MatchedAddress> FindAddressCandidates(params AddressValue[] values);

    private AddressValue[] GetAddressValues(string[] values)
    {
      List<AddressValue> addressValues = new List<AddressValue>();

      for (int i = 0; i < Math.Min(values.Length, AddressFields.Count); ++i)
      {
        if (!String.IsNullOrEmpty(values[i]))
        {
          addressValues.Add(new AddressValue(AddressFields[i].Name, values[i]));
        }
      }

      return addressValues.ToArray();
    }

    public MatchedAddress GeocodeAddress(params string[] values)
    {
      return GeocodeAddress(GetAddressValues(values));
    }

    public MatchedAddress GeocodeAddress(List<AddressValue> values)
    {
      return GeocodeAddress(values.ToArray());
    }

    public abstract MatchedAddress GeocodeAddress(params AddressValue[] values);

    public DataTable GetAddressCandidatesTable(params string[] values)
    {
      return GetAddressCandidatesTable(GetAddressValues(values));
    }

    public DataTable GetAddressCandidatesTable(List<AddressValue> values)
    {
      return GetAddressCandidatesTable(values.ToArray());
    }

    public DataTable GetAddressCandidatesTable(params AddressValue[] values)
    {
      List<MatchedAddress> matchedAddresses = FindAddressCandidates(values);

      DataTable table = new DataTable();
      table.Columns.Add("Address", typeof(string));
      table.Columns.Add("Score", typeof(int));
      table.Columns.Add("X", typeof(double));
      table.Columns.Add("Y", typeof(double));

      foreach (MatchedAddress matchedAddress in matchedAddresses)
      {
        DataRow row = table.NewRow();

        row["Address"] = matchedAddress.Address;
        row["Score"] = matchedAddress.Score;
        row["X"] = matchedAddress.Location == null ? null : (object)matchedAddress.Location.X;
        row["Y"] = matchedAddress.Location == null ? null : (object)matchedAddress.Location.Y;

        table.Rows.Add(row);
      }

      return table;
    }

    protected void ValidateAddressValues(AddressValue[] values)
    {
      if (values.Length == 0)
      {
        throw new ArgumentException("At least one address value must be provided.");
      }

      foreach (AddressValue addressValue in values)
      {
        if (!AddressFields.Any(o => String.Compare(o.Name, addressValue.Name, true) == 0))
        {
          throw new ArgumentException(String.Format("This geocoder does not contain an address field named \"{0}\".", addressValue.Name));
        }
      }

      foreach (CommonField field in AddressFields.Where(o => o.Required))
      {
        if (!values.Any(o => String.Compare(o.Name, field.Name, true) == 0))
        {
          throw new ArgumentException(String.Format("Required address value \"{0}\" is missing.", field.Name));
        }
      }
    }

    public abstract void Reload();
  }

  public struct AddressValue
  {
    public string Name;
    public string Value;

    public AddressValue(string name, string value)
    {
      Name = name;
      Value = value;
    }
  }

  [DataContract]
  public class MatchedAddress : IComparable<MatchedAddress>
  {
    [DataMember]
    public string Address = null;

    [DataMember]
    public int Score = 0;

    [DataMember]
    public Coordinate Location = null;

    public int CompareTo(MatchedAddress other)
    {
      int c = -Score.CompareTo(other.Score);

      if (c == 0)
      {
        c = Address.CompareTo(other.Address);

        if (c == 0)
        {
          c = Location.CompareTo(other.Location);
        }
      }

      return c;
    }

    public override string ToString()
    {
      return String.Format("{0} | {1} | {2}", Score, Address, Location);
    }
  }
}
