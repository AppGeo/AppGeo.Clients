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

namespace AppGeo.Clients.ArcIms.ArcXml
{
  [Serializable]
	public class Error : Response
	{
		public const string XmlName = "ERROR";

		public static Error ReadFrom(ArcXmlReader reader)
		{
			try
			{
				Error error = new Error();

				if (reader.HasAttributes)
				{
					while (reader.MoveToNextAttribute())
					{
						string value = reader.ReadContentAsString();

						if (value.Length > 0)
						{
							switch (reader.Name)
							{
								case "machine": error.Machine = value; break;
								case "processid": error.ProcessID = Convert.ToInt32(value); break;
								case "threadid": error.ThreadID = Convert.ToInt32(value); break;
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
						if (reader.NodeType == XmlNodeType.Text)
						{
							error.Text = reader.Value;
						}

						reader.Read();
					}
				}

				return error;
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

		public string Machine = "";
		public int ProcessID = -1;
		public int ThreadID = -1;
		public string Text = "";

		public Error() { }

		public override object Clone()
		{
			return this.MemberwiseClone();
		}
	}
}
