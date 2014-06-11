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
using System.Collections.Generic;
using System.Xml;

namespace AppGeo.Clients.ArcIms.ArcXml
{
  [Serializable]
  public class GcStyle : ICloneable
	{
		public const string XmlName = "GCSTYLE";

		public static GcStyle ReadFrom(ArcXmlReader reader)
		{
			try
			{
				GcStyle gcStyle = new GcStyle();

				if (reader.HasAttributes)
				{
					while (reader.MoveToNextAttribute())
					{
						string value = reader.ReadContentAsString();

						if (value.Length > 0)
						{
							switch (reader.Name)
							{
								case "name": gcStyle.Name = value; break;
								case "endoffset": gcStyle.EndOffset = Convert.ToInt32(value); break;
								case "sideoffset": gcStyle.SideOffset = Convert.ToInt32(value); break;
								case "sideoffsetunits": gcStyle.SideOffsetUnits = (Units)ArcXmlEnumConverter.ToEnum(typeof(Units), value); break;
								case "spellingsensitivity": gcStyle.SpellingSensitivity = Convert.ToInt32(value); break;
							}
						}
					}

					reader.MoveToElement();
				}

        if (!reader.IsEmptyElement)
        {
          reader.Read();

          while (!(reader.NodeType == XmlNodeType.EndElement && reader.Name == XmlName))
          {
            if (reader.NodeType == XmlNodeType.Element)
            {
              switch (reader.Name)
              {
                case GcInput.XmlName: gcStyle.GcInputs.Add(GcInput.ReadFrom(reader)); break;
              }
            }

            reader.Read();
          }
        }

				return gcStyle;
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

		public string Name = "";
		public int EndOffset = 3;
		public int SideOffset = 0;
		public Units SideOffsetUnits = Units.Feet;
		public int SpellingSensitivity = 80;

    private List<GcInput> _gcInputs = new List<GcInput>();

		public GcStyle() { }

    public List<GcInput> GcInputs
    {
      get
      {
        return _gcInputs;
      }
    }

		public object Clone()
		{
			GcStyle clone = (GcStyle)this.MemberwiseClone();
			return clone;
		}
	}
}
