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

namespace AppGeo.Clients.ArcIms.ArcXml
{
  [Serializable]
  public class Capabilities : ICloneable
  {
    public const string XmlName = "CAPABILITIES";

    public static Capabilities ReadFrom(ArcXmlReader reader)
    {
      try
      {
        Capabilities capabilities = new Capabilities();

        if (reader.HasAttributes)
        {
          while (reader.MoveToNextAttribute())
          {
            string value = reader.ReadContentAsString();

            if (value.Length > 0)
            {
              switch (reader.Name)
              {
                case "disabledtypes": capabilities.DisabledTypes = value; break;
                case "forbidden": capabilities.Forbidden = value; break;
                case "returngeometry": capabilities.ReturnGeometry = (ReturnGeometry)ArcXmlEnumConverter.ToEnum(typeof(ReturnGeometry), value); break;
                case "servertype": capabilities.ServerType = value; break;
              }
            }
          }

          reader.MoveToElement();
        }

        return capabilities;
      }
      catch (Exception ex)
      {
        if (ex is ArcXmlException)
        {
          throw ex;
        }
        else
        {
          throw new ArcXmlException(String.Format("Could not read {0} element.", XmlName), ex);
        }
      }
    }

    public Capabilities() { }

    public string DisabledTypes = null;
    public string Forbidden = null;
    public ReturnGeometry ReturnGeometry = ReturnGeometry.XmlMode;
    public string ServerType = null;

    public object Clone()
    {
      Capabilities clone = (Capabilities)this.MemberwiseClone();
      return clone;
    }
  }
}
