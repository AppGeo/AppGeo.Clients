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
using GeoAPI.Geometries;

namespace AppGeo.Clients.ArcIms.ArcXml
{
	public static class EnvelopeSerializer
	{
	  public static Envelope ReadFrom(ArcXmlReader reader)
	  {
			try
			{
        double minx = Double.NaN;
				double miny = Double.NaN;
				double maxx = Double.NaN;
				double maxy = Double.NaN;

				if (reader.HasAttributes)
				{
					while (reader.MoveToNextAttribute())
					{
						string value = reader.ReadContentAsString();

						if (value.Length > 0)
						{
							switch (reader.Name)
							{
								case "minx": minx = Convert.ToDouble(value); break;
								case "miny": miny = Convert.ToDouble(value); break;
								case "maxx": maxx = Convert.ToDouble(value); break;
								case "maxy": maxy = Convert.ToDouble(value); break;
							}
						}
					}

					reader.MoveToElement();
				}

				if (Double.IsNaN(minx) || Double.IsNaN(minx) || Double.IsNaN(minx) || Double.IsNaN(minx))
				{
					return new Envelope();
				}
				else
				{
					return new Envelope(new Coordinate(minx, miny), new Coordinate(maxx, maxy));
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
					throw new ArcXmlException("Could not read ENVELOPE element.", ex);
				}
			}
		}

		public static void WriteTo(ArcXmlWriter writer, Envelope envelope)
	  {
			if (!envelope.IsNull)
			{
				try
				{
					writer.WriteStartElement("ENVELOPE");

					writer.WriteAttributeString("minx", envelope.MinX.ToString());
					writer.WriteAttributeString("miny", envelope.MinY.ToString());
					writer.WriteAttributeString("maxx", envelope.MaxX.ToString());
					writer.WriteAttributeString("maxy", envelope.MaxY.ToString());

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
						throw new ArcXmlException("Could not write Envelope object.", ex);
					}
				}
			}
		}
	}
}
