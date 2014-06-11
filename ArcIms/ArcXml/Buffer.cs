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
  public class Buffer : ICloneable
	{
		public const string XmlName = "BUFFER";

		public double Distance = 0;
		public BufferUnits BufferUnits = BufferUnits.Default;
		public bool Project = true;

		public SpatialQuery SpatialQuery = null;
		public TargetLayer TargetLayer = null;

		public Buffer() { }

		public object Clone()
		{
			Buffer clone = (Buffer)this.MemberwiseClone();

			if (SpatialQuery != null)
			{
				clone.SpatialQuery = (SpatialQuery)SpatialQuery.Clone();
			}

			if (TargetLayer != null)
			{
				clone.TargetLayer = (TargetLayer)TargetLayer.Clone();
			}

			return clone;
		}

		public void WriteTo(ArcXmlWriter writer)
		{
			try
			{
				writer.WriteStartElement(XmlName);

				if (Distance > 0)
				{
					writer.WriteAttributeString("distance", Distance.ToString());
				}

				if (BufferUnits != BufferUnits.Default)
				{
					writer.WriteAttributeString("bufferunits", ArcXmlEnumConverter.ToArcXml(typeof(BufferUnits), BufferUnits));
				}

				if (!Project)
				{
					writer.WriteAttributeString("project", "false");
				}

				if (SpatialQuery != null)
				{
				  SpatialQuery.WriteTo(writer);
				}

				if (TargetLayer != null)
				{
					TargetLayer.WriteTo(writer);
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
