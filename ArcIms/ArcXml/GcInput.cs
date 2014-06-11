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
  public class GcInput : ICloneable
  {
    public const string XmlName = "GCINPUT";

    public static GcInput ReadFrom(ArcXmlReader reader)
    {
      try
      {
        GcInput gcInput = new GcInput();

        if (reader.HasAttributes)
        {
          while (reader.MoveToNextAttribute())
          {
            string value = reader.ReadContentAsString();

            if (value.Length > 0)
            {
              switch (reader.Name)
              {
                case "description": gcInput.Description = value; break;
                case "id": gcInput.ID = value; break;
                case "label": gcInput.Label = value; break;
                case "type": gcInput.Type = value; break;
                case "width": gcInput.Width = Convert.ToInt32(value); break;
              }
            }
          }

          reader.MoveToElement();
        }

        return gcInput;
      }
      catch (Exception ex)
      {
        if (ex is ArcXmlException)
        {
          throw ex;
        }
        else
        {
          throw new ArcXmlException("Could not read " + XmlName + " element.", ex);
        }
      }
    }

		public string Description = null;
    public string ID = null;
    public string Label = null;
    public string Type = null;
		public int Width = 0;

		public GcInput() { }

    public object Clone()
    {
      GcInput clone = (GcInput)this.MemberwiseClone();
      return clone;
    }
  }
}
