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
	public class Query : ICloneable
	{
		public const string XmlName = "QUERY";

    public static Query ReadFrom(ArcXmlReader reader)
    {
      try
      {
        Query query = new Query();

        if (reader.HasAttributes)
        {
          while (reader.MoveToNextAttribute())
          {
            string value = reader.ReadContentAsString();

            if (value.Length > 0)
            {
              switch (reader.Name)
              {
                case "accuracy": query.Accuracy = Convert.ToDouble(value); break;
                case "featurelimit": query.FeatureLimit = Convert.ToInt32(value); break;
                case "joinexpression": query.JoinExpression = value; break;
                case "jointables": query.JoinTables = value; break;
                case "subfields": query.Subfields = value; break;
                case "where": query.Where = value; break;
              }
            }
          }

          reader.MoveToElement();
        }

        return query;
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

		public double Accuracy = 0;
		public int FeatureLimit = 0;
    public string JoinExpression = null;
    public string JoinTables = null;
    public string Subfields = null;
		public string Where = null;

		public Buffer Buffer = null;
		public FeatureCoordSys FeatureCoordSys = null;

    public Query() { }

		public Query(string where)
		{
			Where = where;
		}

		public virtual object Clone()
		{
			Query clone = (Query)this.MemberwiseClone();

			if (Buffer != null)
			{
				clone.Buffer = (Buffer)Buffer.Clone();
			}

			if (FeatureCoordSys != null)
			{
				clone.FeatureCoordSys = (FeatureCoordSys)FeatureCoordSys.Clone();
			}

			return clone;
		}

		public virtual void WriteTo(ArcXmlWriter writer)
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
