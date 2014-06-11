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
	public class LayerDef : ICloneable
	{
		public const string XmlName = "LAYERDEF";

    public string ID = null;
    public string Name = null;
    
		public bool Visible = true;
		public Query Query = null;
		public Renderer Renderer = null;

		public LayerDef() { }

		public LayerDef(string id)
		{
			ID = id;
		}

		public object Clone()
		{
			LayerDef clone = (LayerDef)this.MemberwiseClone();

			if (Query != null)
			{
				clone.Query = (Query)Query.Clone();
			}

			if (Renderer != null)
			{
				clone.Renderer = (Renderer)Renderer.Clone();
			}

			return clone;
		}

		public void WriteTo(ArcXmlWriter writer)
		{
			try
			{
				writer.WriteStartElement(XmlName);

        if (!String.IsNullOrEmpty(ID))
        {
          writer.WriteAttributeString("id", ID);
        }

        if (!String.IsNullOrEmpty(Name))
        {
          writer.WriteAttributeString("name", Name);
        }

				if (Visible)
				{
					writer.WriteAttributeString("visible", "true");
				}

				if (Query != null)
				{
					Query.WriteTo(writer);
				}

				if (Renderer != null)
				{
					Renderer.WriteTo(writer);
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
