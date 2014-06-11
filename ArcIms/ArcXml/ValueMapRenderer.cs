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
	public class ValueMapRenderer : Renderer
	{
		public const string XmlName = "VALUEMAPRENDERER";

		public static ValueMapRenderer ReadFrom(ArcXmlReader reader)
		{
			try
			{
				ValueMapRenderer valueMapRenderer = new ValueMapRenderer();

				if (reader.HasAttributes)
				{
					while (reader.MoveToNextAttribute())
					{
						string value = reader.ReadContentAsString();

						if (value.Length > 0)
						{
							switch (reader.Name)
							{
								case "lookupfield": valueMapRenderer.LookupField = value; break;
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
								case Exact.XmlName: valueMapRenderer.Classifications.Add(Exact.ReadFrom(reader)); break;
								case Other.XmlName: valueMapRenderer.Classifications.Add(Other.ReadFrom(reader)); break;
								case Range.XmlName: valueMapRenderer.Classifications.Add(Range.ReadFrom(reader)); break;
							}
						}

						reader.Read();
					}
				}

				return valueMapRenderer;
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

		public string LookupField = null;

		private List<Classification> _classifications = new List<Classification>();

		public ValueMapRenderer() { }

		public ValueMapRenderer(string lookupField)
		{
			LookupField = lookupField;
		}

		public List<Classification> Classifications
		{
			get
			{
				return _classifications;
			}
		}

		public void AddValue(string value, Symbol symbol)
		{
			_classifications.Add(new Exact(value, symbol));
		}

		public override object Clone()
		{
			ValueMapRenderer clone = (ValueMapRenderer)this.MemberwiseClone();

			clone._classifications = new List<Classification>();

			foreach (Classification classification in _classifications)
			{
				clone.Classifications.Add((Classification)classification.Clone());
			}

			return clone;
		}

		public override void WriteTo(ArcXmlWriter writer)
		{
			try
			{
				writer.WriteStartElement(XmlName);

				if (!String.IsNullOrEmpty(LookupField))
				{
					writer.WriteAttributeString("lookupfield", LookupField);
				}

				foreach (Classification classification in _classifications)
				{
					classification.WriteTo(writer);
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
