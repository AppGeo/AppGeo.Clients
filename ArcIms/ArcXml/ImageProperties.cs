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
  public class ImageProperties : ICloneable
  {
    public const string XmlName = "IMAGEPROPERTIES";

    public Color TransparentColor = Color.Empty;
    public double Transparency = 1;

    public ImageProperties() { }

    public object Clone()
    {
      ImageProperties clone = (ImageProperties)this.MemberwiseClone();
      return clone;
    }

    public void WriteTo(ArcXmlWriter writer)
    {
      try
      {
        writer.WriteStartElement(XmlName);

        if (!TransparentColor.IsEmpty)
        {
          writer.WriteAttributeString("transcolor", ColorConverter.ToArcXml(TransparentColor));
        }

        if (0 <= Transparency && Transparency < 1)
        {
          writer.WriteAttributeString("transparency", Transparency.ToString("0.000"));
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
