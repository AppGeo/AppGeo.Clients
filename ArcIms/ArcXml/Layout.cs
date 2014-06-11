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
using System.Xml;
using GeoAPI.Geometries;

namespace AppGeo.Clients.ArcIms.ArcXml
{
  [Serializable]
	public class Layout : Response
	{
		public const string XmlName = "LAYOUT";

		public static Layout ReadFrom(ArcXmlReader reader)
		{
			try
			{
				Layout layout = new Layout();

				if (!reader.IsEmptyElement)
				{
					reader.Read();

					while (!(reader.NodeType == XmlNodeType.EndElement && reader.Name == XmlName))
					{
						if (reader.NodeType == XmlNodeType.Element)
						{
							switch (reader.Name)
							{
                case "ENVELOPE": layout.Envelope = EnvelopeSerializer.ReadFrom(reader); break;
                case Output.XmlName: layout.Output = Output.ReadFrom(reader); break;
							}
						}

						reader.Read();
					}
				}

				return layout;
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

    public Envelope Envelope = new Envelope();
		public Output Output = null;

		public Layout() { }

		public override object Clone()
		{
			Layout clone = (Layout)this.MemberwiseClone();

			if (Output != null)
			{
				clone.Output = (Output)Output.Clone();
			}

			return clone;
		}
	}
}
