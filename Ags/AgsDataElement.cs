// © 2007-2010, Applied Geographics, Inc.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;

using AppGeo.Clients.Ags.Proxy;

namespace AppGeo.Clients.Ags
{
  public enum DataElementType
  {
    Unsupported,
    FeatureClass,
    FeatureDataSet
  }

  [DataContract]
  public class AgsDataElement
  {
    private AgsGeoDataService _service = null;
    private DataElement _dataElement = null;

    // Should be in common
    [DataMember(Name = "name")]
    private string _name = "";
    private DataElementType _type = DataElementType.Unsupported;
    private string _metadata = "";
    private List<AgsDataElement> _children = new List<AgsDataElement>();
    private AgsDataElement _parent = null;

    [DataMember(Name = "pubDate")]
    private string _pubDate = "";
    [DataMember(Name = "timePeriodDate")]
    private string _timePeriodDate = "";
    [DataMember(Name = "origin")]
    private string _origin = "";
    [DataMember(Name = "title")]
    private string _title = "";
    [DataMember(Name = "abstract")]
    private string _abstract = "";
    [DataMember(Name = "purpose")]
    private string _purpose = "";
    [DataMember(Name = "keywords")]
    private string _keywords = "";
    [DataMember(Name = "places")]
    private string _places = "";
    [DataMember(Name = "envelope")]
    private AppGeo.Geo.Envelope _envelope = null;
    [DataMember(Name = "contactOrganization")]
    private string _contactOrganization = "";
    [DataMember(Name = "sourceScale")]
    private string _sourceScale = "";

    /// <summary>
    /// Create an AgsDataElement and extract some useful information from the metadata
    /// </summary>
    public AgsDataElement(AgsGeoDataService service, DataElement dataElement)
    {
      _service = service;
      _dataElement = dataElement;
      _name = dataElement.Name;
      _metadata = dataElement.Metadata.XmlDoc;

      if (dataElement is DEDataset)
      {
        switch (((DEDataset)dataElement).DatasetType)
        {
          case esriDatasetType.esriDTFeatureClass:
            _type = DataElementType.FeatureClass;
            break;

          case esriDatasetType.esriDTFeatureDataset:
            _type = DataElementType.FeatureDataSet;
            break;
        }
      }

      XElement metaData = XElement.Parse(_metadata);
      _origin = GetXPathValue(metaData, "./idinfo/citation/citeinfo/origin");
      _title = GetXPathValue(metaData, "./idinfo/citation/citeinfo/title");
      _abstract = GetXPathValue(metaData, "./idinfo/descript/abstract");
      _purpose = GetXPathValue(metaData, "./idinfo/descript/purpose");
      _keywords = String.Join(",", GetXPathValues(metaData, "./idinfo/keywords/theme", "themekey").ToArray());
      _places = String.Join(",", GetXPathValues(metaData, "./idinfo/keywords/place", "placekey").ToArray());

      _pubDate = GetXPathValue(metaData, "./idinfo/citation/citeinfo/pubdate");
      _timePeriodDate = GetXPathValue(metaData, "./idinfo/timeperd/timeinfo/sngdate/caldate");

      string leftValue = GetXPathValue(metaData, "./idinfo/spdom/lboundng/leftbc");
      string bottomValue = GetXPathValue(metaData, "./idinfo/spdom/lboundng/bottombc");
      string rightValue = GetXPathValue(metaData, "./idinfo/spdom/lboundng/rightbc");
      string topValue = GetXPathValue(metaData, "./idinfo/spdom/lboundng/topbc");

      double left, bottom, right, top;
      if (Double.TryParse(leftValue, out left) && Double.TryParse(bottomValue, out bottom) && Double.TryParse(rightValue, out right) && Double.TryParse(topValue, out top))
      {
        _envelope = new AppGeo.Geo.Envelope(left, bottom, right, top);
      }

      _contactOrganization = GetXPathValue(metaData, "./idinfo/ptcontac/cntinfo/cntorgp/cntorg");

      _sourceScale = GetXPathValue(metaData, "./dataqual/lineage/srcinfo/srcscale");
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

    public DataElementType Type
    {
      get
      {
        return _type;
      }
      protected set
      {
        _type = value;
      }
    }

    public string Metadata
    {
      get
      {
        return _metadata;
      }
      protected set
      {
        _metadata = value;
      }
    }

    public string PubDate
    {
      get
      {
        return _pubDate;
      }
    }

    public string TimePeriodDate
    {
      get
      {
        return _timePeriodDate;
      }
    }

    public string Origin
    {
      get
      {
        return _origin;
      }
    }

    public string Title
    {
      get
      {
        return _title;
      }
    }

    public string Abstract
    {
      get
      {
        return _abstract;
      }
    }

    public string Purpose
    {
      get
      {
        return _purpose;
      }
    }

    public string Keywords
    {
      get
      {
        return _keywords;
      }
    }

    public string Places
    {
      get
      {
        return _places;
      }
    }

    public AppGeo.Geo.Envelope Envelope
    {
      get
      {
        return _envelope;
      }
    }

    public string ContactOrganization
    {
      get { return _contactOrganization; }
    }

    public string SourceScale
    {
      get { return _sourceScale; }
    }

    public List<AgsDataElement> Children
    {
      get
      {
        return _children;
      }
      internal set
      {
        _children = value;
      }
    }

    public AgsDataElement Parent
    {
      get
      {
        return _parent;
      }
      internal set
      {
        _parent = value;
      }
    }

    public DataElement DataElement
    {
      get { return _dataElement; }
    }

    private string GetXPathValue(XElement root, string path)
    {
      XElement el = root.XPathSelectElement(path);
      return el != null ? el.Value : "";
    }

    /// <summary>
    /// Assumes there are sub elements and you want the values of all the leaves with the given name
    /// </summary>
    private IEnumerable<string> GetXPathValues(XElement root, string path, string leafName)
    {
      XElement el = root.XPathSelectElement(path);
      if (el != null)
      {
        return el.Elements(leafName).Select(l => l.Value);
      }
      else
      {
        return new List<string>();
      }
    }
  }
}
