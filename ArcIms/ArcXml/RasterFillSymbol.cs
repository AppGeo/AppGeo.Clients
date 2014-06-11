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
	public class RasterFillSymbol : Symbol
	{
    public const string XmlName = "RASTERFILLSYMBOL";

    public static RasterFillSymbol ReadFrom(ArcXmlReader reader)
    {
      try
      {
        RasterFillSymbol rasterFillSymbol = new RasterFillSymbol();

        if (reader.HasAttributes)
        {
          while (reader.MoveToNextAttribute())
          {
            string value = reader.ReadContentAsString();

            if (value.Length > 0)
            {
              switch (reader.Name)
              {
                case "antialiasing": rasterFillSymbol.Antialiasing = Convert.ToBoolean(value); break;
                case "image": rasterFillSymbol.Image = value; break;
                case "overlap": rasterFillSymbol.Overlap = Convert.ToBoolean(value); break;
                case "transparency": rasterFillSymbol.Transparency = Convert.ToDouble(value); break;
                case "url": rasterFillSymbol.Url = value; break;
              }
            }
          }

          reader.MoveToElement();
        }

        return rasterFillSymbol;
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

    public bool Antialiasing = false;
    public string Image = null;
    public bool Overlap = true;
    public double Transparency = 1;
    public string Url = null;

    public RasterFillSymbol() { }

    public override object Clone()
    {
      RasterFillSymbol clone = (RasterFillSymbol)this.MemberwiseClone();
      return clone;
    }

    public override void WriteTo(ArcXmlWriter writer)
    {
      try
      {
        writer.WriteStartElement(XmlName);

        if (Antialiasing)
        {
          writer.WriteAttributeString("antialiasing", "true");
        }

        if (!String.IsNullOrEmpty(Image))
        {
          writer.WriteAttributeString("image", Image);
        }

        if (!Overlap)
        {
          writer.WriteAttributeString("overlap", "false");
        }

        if (0 <= Transparency && Transparency < 1)
        {
          writer.WriteAttributeString("transparency", Transparency.ToString("0.000"));
        }

        if (!String.IsNullOrEmpty(Url))
        {
          writer.WriteAttributeString("url", Url);
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
