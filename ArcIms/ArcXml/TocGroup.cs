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
	public class TocGroup : List<TocClass>
	{
		public const string XmlName = "TOCGROUP";

		public static TocGroup ReadFrom(ArcXmlReader reader)
		{
			try
			{
				TocGroup tocGroup = new TocGroup();

				if (reader.HasAttributes)
				{
					while (reader.MoveToNextAttribute())
					{
						string value = reader.ReadContentAsString();

						if (value.Length > 0)
						{
							switch (reader.Name)
							{
								case "heading": tocGroup.Heading = value; break;
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
								case TocClass.XmlName: tocGroup.Add(TocClass.ReadFrom(reader)); break;
							}
						}

						reader.Read();
					}
				}

				return tocGroup;
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

		public string Heading = null;
		public double MinScale = 0;
		public double MaxScale = Double.PositiveInfinity;

		public TocGroup() { }

    public TocGroup(IEnumerable<TocClass> tocClasses) : base(tocClasses) { }

		public object Clone()
		{
      TocGroup clone = new TocGroup();

      clone.Heading = Heading;
      clone.MinScale = MinScale;
      clone.MaxScale = MaxScale;

			foreach (TocClass tocClass in this)
			{
				clone.Add((TocClass)tocClass.Clone());
			}

			return clone;
		}
	}
}
