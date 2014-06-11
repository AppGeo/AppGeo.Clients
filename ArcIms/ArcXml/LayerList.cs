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

namespace AppGeo.Clients.ArcIms.ArcXml
{
  [Serializable]
	public class LayerList : List<LayerDef>, ICloneable
	{
		public const string XmlName = "LAYERLIST";

		public bool DynamicFirst = false;
		public bool NoDefault = false;
		public bool Order = true;

		public LayerList() { }

    public bool Contains(string id)
    {
      foreach (LayerDef layerDef in this)
      {
        if (String.Compare(layerDef.ID, id, false) == 0)
        {
          return true;
        }
      }

      return false;
    }

		public object Clone()
		{
      LayerList clone = new LayerList();

      clone.DynamicFirst = DynamicFirst;
      clone.NoDefault = NoDefault;
      clone.Order = Order;

			foreach (LayerDef layerDef in this)
			{
				clone.Add((LayerDef)layerDef.Clone());
			}

			return clone;
		}

		public void WriteTo(ArcXmlWriter writer)
		{
			try
			{
				writer.WriteStartElement(XmlName);

				if (DynamicFirst)
				{
					writer.WriteAttributeString("dynamicfirst", "true");
				}

				if (NoDefault)
				{
					writer.WriteAttributeString("nodefault", "true");
				}

				if (Order)
				{
					writer.WriteAttributeString("order", "true");
				}

				foreach (LayerDef layerDef in this)
				{
					layerDef.WriteTo(writer);
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
