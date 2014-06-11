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
	public class Toc : List<TocGroup>
	{
		public const string XmlName = "TOC";

		public static Toc ReadFrom(ArcXmlReader reader)
		{
			try
			{
				Toc toc = new Toc();

				if (!reader.IsEmptyElement)
				{
					reader.Read();

					while (!(reader.NodeType == XmlNodeType.EndElement && reader.Name == XmlName))
					{
						if (reader.NodeType == XmlNodeType.Element)
						{
							switch (reader.Name)
							{
								case TocGroup.XmlName: toc.Add(TocGroup.ReadFrom(reader)); break;
							}
						}

						reader.Read();
					}
				}

				return toc;
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

		public Toc() { }

    public Toc(IEnumerable<TocGroup> tocGroups) : base(tocGroups) { }

		public object Clone()
		{
			Toc clone = new Toc();

			foreach (TocGroup tocGroup in this)
			{
				clone.Add((TocGroup)tocGroup.Clone());
			}

			return clone;
		}
	}
}
