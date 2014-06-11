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
	public class GetServiceInfo : Request
	{
		public const string XmlName = "GET_SERVICE_INFO";

    public bool ForGeocoding = false;

    public bool AcetateInfo = false;
		public string DataFrame = "#ALL#";
		public int Dpi = 96;
		public bool Envelope = true;
		public bool Extensions = true;
		public bool Fields = true;
    public bool RelativeScale = false;
		public bool Renderer = true;
		public bool Toc = true;
		public ImageType TocType = ImageType.Png8;

		public GetServiceInfo() { }

    public GetServiceInfo(bool forGeocoding) 
    {
      ForGeocoding = forGeocoding;
    }

		public override object Clone()
		{
      GetServiceInfo clone = (GetServiceInfo)this.MemberwiseClone();
      return clone;
		}

		public override void WriteTo(ArcXmlWriter writer)
		{
			try
			{
				writer.WriteStartElement(XmlName);

        if (ForGeocoding)
        {
          writer.WriteAttributeString("extensions", "true");
        }
        else
        {
          if (!String.IsNullOrEmpty(DataFrame))
          {
            writer.WriteAttributeString("dataframe", DataFrame);
          }

          writer.WriteAttributeString("acetateinfo", AcetateInfo ? "true" : "false");
          writer.WriteAttributeString("dpi", Dpi.ToString());
          writer.WriteAttributeString("envelope", Envelope ? "true" : "false");
          writer.WriteAttributeString("extensions", Extensions ? "true" : "false");
          writer.WriteAttributeString("fields", Fields ? "true" : "false");
          writer.WriteAttributeString("relativescale", RelativeScale ? "true" : "false");
          writer.WriteAttributeString("renderer", Renderer ? "true" : "false");
          writer.WriteAttributeString("toc", Toc ? "true" : "false");
          writer.WriteAttributeString("toctype", ArcXmlEnumConverter.ToArcXml(typeof(ImageType), TocType));

          writer.WriteEndElement();
        }
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
