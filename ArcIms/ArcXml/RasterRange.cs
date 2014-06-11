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
  public class RasterRange : RasterClassification
  {
    public const string XmlName = "RASTER_RANGE";

    public static RasterRange ReadFrom(ArcXmlReader reader)
    {
      try
      {
        RasterRange rasterRange = new RasterRange();

        if (reader.HasAttributes)
        {
          while (reader.MoveToNextAttribute())
          {
            string value = reader.ReadContentAsString();

            if (value.Length > 0)
            {
              switch (reader.Name)
              {
                case "color": rasterRange.Color = ColorConverter.ToColor(value); break;
                case "equality": rasterRange.Equality = (RangeEquality)ArcXmlEnumConverter.ToEnum(typeof(RangeEquality), value); break;
                case "label": rasterRange.Label = value; break;
                case "lower": rasterRange.Lower = Convert.ToDouble(value); break;
                case "transparency": rasterRange.Transparency = Convert.ToDouble(value); break;
                case "upper": rasterRange.Upper = Convert.ToDouble(value); break;
              }
            }
          }

          reader.MoveToElement();
        }

        return rasterRange;
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

    public RangeEquality Equality = RangeEquality.Lower;
    public double Lower = 0;
    public double Upper = 0;

    public RasterRange() { }

    public override object Clone()
    {
      RasterRange clone = (RasterRange)this.MemberwiseClone();
      return clone;
    }
  }
}
