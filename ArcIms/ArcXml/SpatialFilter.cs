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
using GeoAPI.Geometries;

namespace AppGeo.Clients.ArcIms.ArcXml
{
  [Serializable]
	public class SpatialFilter : ICloneable
	{
		public const string XmlName = "SPATIALFILTER";

		public SpatialRelation Relation = SpatialRelation.AreaIntersection;

    public IGeometry Shape = null;
		public Buffer Buffer = null;

		public SpatialFilter() { }

    public SpatialFilter(IGeometry shape)
		{
			Shape = shape;
		}

		public object Clone()
		{
			SpatialFilter clone = (SpatialFilter)this.MemberwiseClone();

			if (Shape != null)
			{
        clone.Shape = (IGeometry)Shape.Clone();
			}

			if (Buffer != null)
			{
				clone.Buffer = (Buffer)Buffer.Clone();
			}
			
			return clone;
		}

		public void WriteTo(ArcXmlWriter writer)
		{
			try
			{
				writer.WriteStartElement(XmlName);

				writer.WriteAttributeString("relation", ArcXmlEnumConverter.ToArcXml(typeof(SpatialRelation), Relation));

				if (Buffer != null)
				{
					Buffer.WriteTo(writer);
				}

				if (Shape != null)
				{
          if (Shape.OgcGeometryType == OgcGeometryType.Point)
          {
            GeometrySerializer.WriteAsMultiPointTo(writer, (IPoint)Shape);
          }
          else
          {
            GeometrySerializer.WriteTo(writer, Shape);
          }
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
