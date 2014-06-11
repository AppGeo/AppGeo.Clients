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
	public class Scale : ICloneable
	{
		public const string XmlName = "SCALE";

    public static Scale ReadFrom(ArcXmlReader reader)
    {
      try
      {
        Scale scale = new Scale();

        if (reader.HasAttributes)
        {
          while (reader.MoveToNextAttribute())
          {
            string value = reader.ReadContentAsString();

            if (value.Length > 0)
            {
              switch (reader.Name)
              {
                case "rf": scale.RepresentativeFraction = Convert.ToDouble(value.StartsWith("1:") ? value.Substring(2) : value); break;
              }
            }
          }

          reader.MoveToElement();
        }

        return scale;
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

    public double RepresentativeFraction = 0;
		public double X = 0;
		public double Y = 0;

		public Scale() { }

		public object Clone()
		{
			Scale clone = (Scale)this.MemberwiseClone();
			return clone;
		}

		public void WriteTo(ArcXmlWriter writer)
		{
			try
			{
				writer.WriteStartElement(XmlName);

        writer.WriteAttributeString("rf", RepresentativeFraction.ToString());
				writer.WriteAttributeString("x", X.ToString());
				writer.WriteAttributeString("y", Y.ToString());

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
