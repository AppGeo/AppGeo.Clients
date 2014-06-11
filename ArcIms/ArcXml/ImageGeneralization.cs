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
  public class ImageGeneralization : ICloneable
  {
    public const string XmlName = "IMAGEGENERALIZATION";

    public static ImageGeneralization ReadFrom(ArcXmlReader reader)
    {
      try
      {
        ImageGeneralization imageGeneralization = new ImageGeneralization();

        if (reader.HasAttributes)
        {
          while (reader.MoveToNextAttribute())
          {
            string value = reader.ReadContentAsString();

            if (value.Length > 0)
            {
              switch (reader.Name)
              {
                case "mode": imageGeneralization.Mode = value; break;
              }
            }
          }

          reader.MoveToElement();
        }

        return imageGeneralization;
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

    public string Mode = null;

    public ImageGeneralization() { }

    public object Clone()
    {
      ImageGeneralization clone = (ImageGeneralization)this.MemberwiseClone();
      return clone;
    }
  }
}
