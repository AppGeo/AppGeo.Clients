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
  public class ChartSymbol : Symbol
  {
    public const string XmlName = "CHARTSYMBOL";

    public static ChartSymbol ReadFrom(ArcXmlReader reader)
    {
      try
      {
        ChartSymbol chartSymbol = new ChartSymbol();

        if (reader.HasAttributes)
        {
          while (reader.MoveToNextAttribute())
          {
            string value = reader.ReadContentAsString();

            if (value.Length > 0)
            {
              switch (reader.Name)
              {
                case "antialiasing": chartSymbol.Antialiasing = Convert.ToBoolean(value); break;
                case "maxsize": chartSymbol.MaxSize = Convert.ToInt32(value); break;
                case "maxvalue": chartSymbol.MaxValue = Convert.ToInt32(value); break;
                case "minsize": chartSymbol.MinSize = Convert.ToInt32(value); break;
                case "minvalue": chartSymbol.MinValue = Convert.ToInt32(value); break;
                case "mode": chartSymbol.Mode = (ChartSymbolMode)ArcXmlEnumConverter.ToEnum(typeof(ChartSymbolMode), value); break;
                case "outline": chartSymbol.Outline = ColorConverter.ToColor(value); break;
                case "shadow": chartSymbol.Shadow = ColorConverter.ToColor(value); break;
                case "size": chartSymbol.Size = Convert.ToInt32(value); break;
                case "sizefield": chartSymbol.SizeField = value; break;
                case "transparency": chartSymbol.Transparency = Convert.ToDouble(value); break;
                case "width": chartSymbol.Width = Convert.ToInt32(value); break;
              }
            }
          }

          reader.MoveToElement();
        }

        return chartSymbol;
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
    public int MaxSize = 0;
    public int MaxValue = 0;
    public int MinSize = 0;
    public int MinValue = 0;
    public ChartSymbolMode Mode = ChartSymbolMode.Pie;
    public Color Outline = Color.Empty;
    public Color Shadow = Color.Empty;
    public int Size = 0;
    public string SizeField = null;
    public double Transparency = 1;
    public int Width = 0;

    public ChartSymbol() { }

    public override object Clone()
    {
      ChartSymbol clone = (ChartSymbol)this.MemberwiseClone();
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

        if (MaxSize > 0)
        {
          writer.WriteAttributeString("maxsize", MaxSize.ToString());
        }

        if (MaxSize > 0 && MaxValue > 0)
        {
          writer.WriteAttributeString("maxvalue", MaxValue.ToString());
        }

        if (MinSize > 0)
        {
          writer.WriteAttributeString("minsize", MinSize.ToString());
        }

        if (MinSize > 0 && MinValue > 0)
        {
          writer.WriteAttributeString("minvalue", MinValue.ToString());
        }

        if (Mode != ChartSymbolMode.Pie)
        {
          writer.WriteAttributeString("mode", ArcXmlEnumConverter.ToArcXml(typeof(ChartSymbolMode), Mode));
        }

        if (!Outline.IsEmpty)
        {
          writer.WriteAttributeString("outline", ColorConverter.ToArcXml(Outline));
        }

        if (!Shadow.IsEmpty)
        {
          writer.WriteAttributeString("shadow", ColorConverter.ToArcXml(Shadow));
        }

        if (Size > 0)
        {
          writer.WriteAttributeString("size", Size.ToString());
        }

        if (!String.IsNullOrEmpty(SizeField))
        {
          writer.WriteAttributeString("sizefield", SizeField);
        }

        if (0 <= Transparency && Transparency < 1)
        {
          writer.WriteAttributeString("transparency", Transparency.ToString("0.000"));
        }

        if (Width > 0)
        {
          writer.WriteAttributeString("width", Width.ToString());
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
