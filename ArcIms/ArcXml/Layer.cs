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
using GeoAPI.Geometries;

namespace AppGeo.Clients.ArcIms.ArcXml
{
  [Serializable]
	public class Layer : ICloneable
	{
		public const string XmlName = "LAYER";

		public static Layer ReadFrom(ArcXmlReader reader)
		{
			try
			{
				Layer layer = new Layer();

				if (reader.HasAttributes)
				{
					while (reader.MoveToNextAttribute())
					{
						string value = reader.ReadContentAsString();

						if (value.Length > 0)
						{
							switch (reader.Name)
							{
								case "featurecount": layer.FeatureCount = Convert.ToInt32(value); break;
								case "id": layer.ID = value; break;
								case "name": layer.Name = value; break;
							}
						}
					}

					reader.MoveToElement();
				}

				return layer;
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

    public string ID = null;
    public string Name = null;

		public int FeatureCount = 0;
		public string MaxScale = null;
		public string MinScale = null;
		public LayerType Type = LayerType.Acetate;
		public bool Visible = true;

		public Dataset Dataset = null;
    public ImageProperties ImageProperties = null;
		public Query Query = null;
		public CoordSys CoordSys = null;
		public Renderer Renderer = null;

		public List<Object> Objects = null;

		public Layer() { }

		public Layer(string id)
		{
			ID = id;
		}

		public Layer(string id, LayerType type)
		{
      ID = id;
			Type = type;
		}

		public LayerDef ToLayerDef()
		{
			return new LayerDef(ID);
		}

		public void Add(Object o)
		{
			if (Type != LayerType.Acetate)
			{
				throw new ArcXmlException("Layer is not of type Acetate, cannot add graphical objects");
			}

			if (Objects == null)
			{
				Objects = new List<Object>();
			}

			Objects.Add(o);
		}

    public void Add(Envelope envelope, Symbol symbol)
    {
      Add(new Object(envelope, symbol));
    }

    public void Add(IGeometry shape, Symbol symbol)
		{
      Add(new Object(shape, symbol));
		}

		public void Add(NorthArrow northArrow)
		{
			Add(new Object(northArrow));
		}

		public void Add(ScaleBar scaleBar)
		{
			Add(new Object(scaleBar));
		}

    public void Add(Text text)
    {
      Add(new Object(text));
    }

    public void Add(Text text, TextMarkerSymbol symbol)
    {
      Add(new Object(text, symbol));
    }

		public object Clone()
		{
			Layer clone = (Layer)this.MemberwiseClone();

			if (CoordSys != null)
			{
				clone.CoordSys = (CoordSys)CoordSys.Clone();
			}

			if (Dataset != null)
			{
				clone.Dataset = (Dataset)Dataset.Clone();
			}

			if (Objects != null)
			{
			  clone.Objects = new List<Object>();

        foreach (Object obj in Objects)
				{
          clone.Objects.Add((Object)obj.Clone());
				}
			}

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

				if (Type != LayerType.None)
				{
					writer.WriteAttributeString("type", ArcXmlEnumConverter.ToArcXml(typeof(LayerType), Type));
				}

				if (!String.IsNullOrEmpty(MaxScale))
				{
					writer.WriteAttributeString("maxscale", MaxScale);
				}

				if (!String.IsNullOrEmpty(MinScale))
				{
					writer.WriteAttributeString("mincale", MinScale);
				}

        if (!String.IsNullOrEmpty(Name))
				{
					writer.WriteAttributeString("name", Name);
				}

				if (!Visible)
				{
					writer.WriteAttributeString("visible", "false");
				}

				if (CoordSys != null)
				{
					CoordSys.WriteTo(writer);
				}

				if (Dataset != null)
				{
					Dataset.WriteTo(writer);
				}

        if (ImageProperties != null)
        {
          ImageProperties.WriteTo(writer);
        }

				if (Objects != null)
				{
					foreach (Object obj in Objects)
					{
            obj.WriteTo(writer);
					}
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
