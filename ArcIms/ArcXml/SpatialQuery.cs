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
	public class SpatialQuery : Query
	{
		public new const string XmlName = "SPATIALQUERY";

    public string OrderBy = null;
		public SearchOrder SearchOrder = SearchOrder.Optimize;

		public FilterCoordSys FilterCoordSys = null;
		public SpatialFilter SpatialFilter = null;

		public SpatialQuery(string where) : this(where, null) { }

		public SpatialQuery(IGeometry shape) : this(null, shape) { }

    public SpatialQuery(string where, IGeometry shape)
		{
			Where = where;

      if (shape != null)
      {
        SpatialFilter = new SpatialFilter(shape);
      }
		}

		public override object Clone()
		{
			SpatialQuery clone = (SpatialQuery)this.MemberwiseClone();

			if (Buffer != null)
			{
				clone.Buffer = (Buffer)Buffer.Clone();
			}

			if (FeatureCoordSys != null)
			{
				clone.FeatureCoordSys = (FeatureCoordSys)FeatureCoordSys.Clone();
			}

			if (FilterCoordSys != null)
			{
				clone.FilterCoordSys = (FilterCoordSys)FilterCoordSys.Clone();
			}

			if (SpatialFilter != null)
			{
				clone.SpatialFilter = (SpatialFilter)SpatialFilter.Clone();
			}

			return clone;
		}

		public override void WriteTo(ArcXmlWriter writer)
		{
			try
			{
				writer.WriteStartElement(XmlName);

				if (Accuracy > 0)
				{
					writer.WriteAttributeString("accuracy", Accuracy.ToString());
				}

				if (FeatureLimit > 0)
				{
					writer.WriteAttributeString("featurelimit", FeatureLimit.ToString());
				}

        if (!String.IsNullOrEmpty(JoinExpression))
        {
          writer.WriteAttributeString("joinexpression", JoinExpression);
        }

        if (!String.IsNullOrEmpty(JoinTables))
        {
          writer.WriteAttributeString("jointables", JoinTables);
        }

        if (!String.IsNullOrEmpty(OrderBy))
        {
          writer.WriteAttributeString("order_by", OrderBy);
        }

        if (!String.IsNullOrEmpty(Subfields))
        {
          writer.WriteAttributeString("subfields", Subfields);
        }

        if (!String.IsNullOrEmpty(Where))
        {
          writer.WriteAttributeString("where", Where);
        }

				if (Buffer != null)
				{
					Buffer.WriteTo(writer);
				}

				if (FeatureCoordSys != null)
				{
					FeatureCoordSys.WriteTo(writer);
				}

				if (FilterCoordSys != null)
				{
					FilterCoordSys.WriteTo(writer);
				}

				if (SpatialFilter != null)
				{
					SpatialFilter.WriteTo(writer);
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
