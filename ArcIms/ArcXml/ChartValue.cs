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
using System.Drawing;

namespace AppGeo.Clients.ArcIms.ArcXml
{
  [Serializable]
  public class ChartValue : ICloneable
  {
    public const string XmlName = "CHARTVALUE";

    public static ChartValue ReadFrom(ArcXmlReader reader)
    {
      try
      {
        ChartValue chartValue = new ChartValue();

        if (reader.HasAttributes)
        {
          while (reader.MoveToNextAttribute())
          {
            string value = reader.ReadContentAsString();

            if (value.Length > 0)
            {
              switch (reader.Name)
              {
                case "color": chartValue.Color = ColorConverter.ToColor(value); break;
                case "lookupfield": chartValue.LookUpField = value; break;
                case "lower": chartValue.Lower = Convert.ToInt32(value); break;
                case "upper": chartValue.Upper = Convert.ToInt32(value); break;
                case "value": chartValue.Value = Convert.ToInt32(value); break;
              }
            }
          }

          reader.MoveToElement();
        }

        return chartValue;
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

    public ChartValue() { }

    public Color Color = Color.Black;
    public string LookUpField = null;
    public int Lower = Int32.MinValue;
    public int Upper = Int32.MinValue;
    public int Value = Int32.MinValue;

    public object Clone()
    {
      ChartValue clone = (ChartValue)this.MemberwiseClone();
      return clone;
    }

    public void WriteTo(ArcXmlWriter writer)
    {
      try
      {
        writer.WriteStartElement(XmlName);

        if (!Color.IsEmpty && Color != Color.Black)
        {
          writer.WriteAttributeString("color", ColorConverter.ToArcXml(Color));
        }

        if (!String.IsNullOrEmpty(LookUpField))
        {
          writer.WriteAttributeString("lookupfield", LookUpField);
        }

        if (Lower > Int32.MinValue)
        {
          writer.WriteAttributeString("lower", Lower.ToString());
        }

        if (Upper > Int32.MinValue)
        {
          writer.WriteAttributeString("upper", Upper.ToString());
        }

        if (Value > Int32.MinValue)
        {
          writer.WriteAttributeString("value", Value.ToString());
        }

        writer.WriteEndElement();
      }
      catch (Exception ex)
      {
        if (ex is ArcXmlException)
        {
          throw ex;
        }
        else
        {
          throw new ArcXmlException(String.Format("Could not write {0} object.", GetType().Name), ex);
        }
      }
    }
  }
}
