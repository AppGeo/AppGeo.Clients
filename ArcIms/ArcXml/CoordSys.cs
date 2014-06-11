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
	public class CoordSys : ICloneable
	{
		public const string XmlName = "COORDSYS";

    public static CoordSys ReadFrom(ArcXmlReader reader)
    {
      return ReadFrom(reader, new CoordSys(), XmlName);
    }

    protected static CoordSys ReadFrom(ArcXmlReader reader, CoordSys coordSys, string xmlName)
    {
      try
      {
        if (reader.HasAttributes)
        {
          while (reader.MoveToNextAttribute())
          {
            string value = reader.ReadContentAsString();

            if (value.Length > 0)
            {
              switch (reader.Name)
              {
                case "id": coordSys.ID = value; break;
                case "string": coordSys.String = value; break;
                case "datumtransformid": coordSys.DatumTransformID = value; break;
                case "datumtransformstring": coordSys.DatumTransformString = value; break;
              }
            }
          }

          reader.MoveToElement();
        }

        return coordSys;
      }
      catch (Exception ex)
      {
        if (ex is ArcXmlException)
        {
          throw ex;
        }
        else
        {
          throw new ArcXmlException(String.Format("Could not read {0} element.", xmlName), ex);
        }
      }
    }

    private string _xmlName= null;

		public string ID = null;
    public string String = null;
    public string DatumTransformID = null;
    public string DatumTransformString = null;

		public CoordSys() : this(XmlName, null) { }

    public CoordSys(string value) : this(XmlName, value) { }

    protected CoordSys(string xmlName, string value)
    {
      _xmlName = xmlName;

      if (!String.IsNullOrEmpty(value))
      {
        int wkid;

        if (Int32.TryParse(value, out wkid))
        {
          ID = value;
        }
        else
        {
          String = value;
        }
      }
    }

		public object Clone()
		{
			CoordSys clone = (CoordSys)this.MemberwiseClone();
			return clone;
		}

		public void WriteTo(ArcXmlWriter writer)
		{
			try
			{
        writer.WriteStartElement(_xmlName);

				if (!String.IsNullOrEmpty(ID))
				{
					writer.WriteAttributeString("id", ID);
				}

				if (!String.IsNullOrEmpty(String))
				{
					writer.WriteAttributeString("string", String);
				}

				if (!String.IsNullOrEmpty(DatumTransformID))
				{
					writer.WriteAttributeString("datumtransformid", DatumTransformID);
				}

				if (!String.IsNullOrEmpty(DatumTransformString))
				{
					writer.WriteAttributeString("datumtransformstring", DatumTransformString);
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
