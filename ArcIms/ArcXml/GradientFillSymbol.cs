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
	public class GradientFillSymbol : Symbol
	{
    public const string XmlName = "GRADIENTFILLSYMBOL";

    public static GradientFillSymbol ReadFrom(ArcXmlReader reader)
    {
      try
      {
        GradientFillSymbol gradientFillSymbol = new GradientFillSymbol();

        if (reader.HasAttributes)
        {
          while (reader.MoveToNextAttribute())
          {
            string value = reader.ReadContentAsString();

            if (value.Length > 0)
            {
              switch (reader.Name)
              {
                case "antialiasing": gradientFillSymbol.Antialiasing = Convert.ToBoolean(value); break;
                case "finishcolor": gradientFillSymbol.FinishColor = ColorConverter.ToColor(value); break;
                case "overlap": gradientFillSymbol.Overlap = Convert.ToBoolean(value); break;
                case "startcolor": gradientFillSymbol.StartColor = ColorConverter.ToColor(value); break;
                case "transparency": gradientFillSymbol.Transparency = Convert.ToDouble(value); break;
                case "type": gradientFillSymbol.Type = (GradientFillType)ArcXmlEnumConverter.ToEnum(typeof(GradientFillType), value); break;
              }
            }
          }

          reader.MoveToElement();
        }

        return gradientFillSymbol;
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
    public Color FinishColor = Color.Green;
    public bool Overlap = true;
    public Color StartColor = Color.Red;
    public double Transparency = 1;
    public GradientFillType Type = GradientFillType.BDiagonal;

		public override object Clone()
		{
      GradientFillSymbol clone = (GradientFillSymbol)this.MemberwiseClone();
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

        if (!FinishColor.IsEmpty)
        {
          writer.WriteAttributeString("finishcolor", ColorConverter.ToArcXml(FinishColor));
        }

        if (!Overlap)
        {
          writer.WriteAttributeString("overlap", "false");
        }

        if (!StartColor.IsEmpty)
        {
          writer.WriteAttributeString("startcolor", ColorConverter.ToArcXml(StartColor));
        }

        if (0 <= Transparency && Transparency < 1)
        {
          writer.WriteAttributeString("transparency", Transparency.ToString("0.000"));
        }

        if (Type != GradientFillType.BDiagonal)
        {
          writer.WriteAttributeString("type", ArcXmlEnumConverter.ToArcXml(typeof(GradientFillType), Type));
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
