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
	public class Service : ICloneable
	{
		public const string XmlName = "SERVICE";

		public static Service ReadFrom(ArcXmlReader reader)
		{
			try
			{
				Service service = new Service();

				if (reader.HasAttributes)
				{
					while (reader.MoveToNextAttribute())
					{
						string value = reader.ReadContentAsString();

						if (value.Length > 0)
						{
							switch (reader.Name.ToLower())
							{
								case "access": service.Access = (ServiceAccess)ArcXmlEnumConverter.ToEnum(typeof(ServiceAccess), value); break;
								case "name": service.Name = value; break;
								case "servicegroup": service.ServiceGroup = value; break;
								case "status": service.Status = (ServiceStatus)ArcXmlEnumConverter.ToEnum(typeof(ServiceStatus), value); break;
								case "type": service.Type = value; break;
								case "version": service.Version = value; break;
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
								case Cleanup.XmlName: service.Cleanup = Cleanup.ReadFrom(reader); break;
								case Environment.XmlName: service.Environment = Environment.ReadFrom(reader); break;
							}
						}

						reader.Read();
					}
				}

				return service;
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

		public ServiceAccess Access = ServiceAccess.Private;
    public string Name = null;
		public string ServiceGroup = null;
		public ServiceStatus Status = ServiceStatus.Disabled;
    public string Type = null;
    public string Version = null;

		public Cleanup Cleanup = null;
		public Environment Environment = null;

		public Service() { }

		public object Clone()
		{
			Service clone = (Service)this.MemberwiseClone();

			if (Cleanup != null)
			{
				clone.Cleanup = (Cleanup)Cleanup.Clone();
			}
			
			if (Environment != null)
			{
				clone.Environment = (Environment)Environment.Clone();
			}
			
			return clone;
		}
	}
}
