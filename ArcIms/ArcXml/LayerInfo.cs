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
using GeoAPI.Geometries;

namespace AppGeo.Clients.ArcIms.ArcXml
{
  [Serializable]
	public class LayerInfo : ICloneable
	{
		public const string XmlName = "LAYERINFO";

		public static LayerInfo ReadFrom(ArcXmlReader reader)
		{
			try
			{
				LayerInfo layerInfo = new LayerInfo();

				if (reader.HasAttributes)
				{
					while (reader.MoveToNextAttribute())
					{
						string value = reader.ReadContentAsString();

						if (value.Length > 0)
						{
							switch (reader.Name)
							{
                case "arcmaptype": layerInfo.ArcMapType = (ArcMapType)ArcXmlEnumConverter.ToEnum(typeof(ArcMapType), value); break;
                case "id": layerInfo.ID = value; break;
								case "type": layerInfo.Type = (LayerType)ArcXmlEnumConverter.ToEnum(typeof(LayerType), value); break;
                case "maxscale": layerInfo.MaxScale = value; break;
								case "minscale": layerInfo.MinScale = value; break;
								case "name": layerInfo.Name = value; break;
                case "parentlayerid": layerInfo.ParentLayerID = value; break;
                case "visible": layerInfo.Visible = Convert.ToBoolean(value); break;
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
                case "ENVELOPE": layerInfo.Envelope = EnvelopeSerializer.ReadFrom(reader); break;
                case FClass.XmlName: layerInfo.FClass = FClass.ReadFrom(reader); break;
								case GroupRenderer.XmlName: layerInfo.Renderer = GroupRenderer.ReadFrom(reader); break;
                case RasterRenderer.XmlName: layerInfo.Renderer = RasterRenderer.ReadFrom(reader); break;
                case ScaleDependentRenderer.XmlName: layerInfo.Renderer = ScaleDependentRenderer.ReadFrom(reader); break;
								case SimpleLabelRenderer.XmlName: layerInfo.Renderer = SimpleLabelRenderer.ReadFrom(reader); break;
								case SimpleRenderer.XmlName: layerInfo.Renderer = SimpleRenderer.ReadFrom(reader); break;
                case Toc.XmlName: layerInfo.Toc = Toc.ReadFrom(reader); break;
                case ValueMapLabelRenderer.XmlName: layerInfo.Renderer = ValueMapLabelRenderer.ReadFrom(reader); break;
								case ValueMapRenderer.XmlName: layerInfo.Renderer = ValueMapRenderer.ReadFrom(reader); break;
                
                case Extension.XmlName:
                  if (layerInfo.Extensions == null)
                  {
                    layerInfo.Extensions = new List<Extension>();
                  }

                  layerInfo.Extensions.Add(Extension.ReadFrom(reader)); break;
              }
						}

						reader.Read();
					}
				}

				return layerInfo;
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
    
    public ArcMapType ArcMapType = ArcMapType.None;
    public string ParentLayerID = null;
    public LayerType Type = LayerType.FeatureClass;
		public string MaxScale = null;
		public string MinScale = null;
		public bool Visible = true;

		public FClass FClass = null;
		public Renderer Renderer = null;
    public Envelope Envelope = new Envelope();
		public List<Extension> Extensions = null;
		public Toc Toc = null;

		public LayerInfo() { }

		public LayerDef ToLayerDef()
		{
			return ToLayerDef(false);
		}

    public bool IsGeocodable
    {
      get
      {
        if (Extensions != null)
        {
          foreach (Extension extension in Extensions)
          {
            if (extension.Type == ExtensionType.Geocode)
            {
              return true;
            }
          }
        }

        return false;
      }
    }

		public LayerDef ToLayerDef(bool includeRenderer)
		{
			LayerDef layerDef = new LayerDef(ID);

			if (includeRenderer && Renderer != null)
			{
				layerDef.Renderer = (Renderer)Renderer.Clone();
			}

			return layerDef;
		}

		public Layer ToLayer(string newID)
		{
			return ToLayer(newID, false);
		}

		public Layer ToLayer(string newID, bool includeRenderer)
		{
			Layer layer = new Layer(newID);
			layer.Type = Type;
			layer.Dataset = new Dataset(ID);
			layer.MinScale = MinScale;
			layer.MaxScale = MaxScale;

			if (includeRenderer && Renderer != null)
			{
				layer.Renderer = (Renderer)Renderer.Clone();
			}

			return layer;
		}

		public object Clone()
		{
			LayerInfo clone = (LayerInfo)this.MemberwiseClone();

			if (FClass != null)
			{
				clone.FClass = (FClass)FClass.Clone();
			}

			if (Renderer != null)
			{
				clone.Renderer = (Renderer)Renderer.Clone();
			}

			if (Extensions != null)
			{
        clone.Extensions = new List<Extension>();

        foreach (Extension extension in Extensions)
        {
          clone.Extensions.Add((Extension)extension.Clone());
        }
			}

			if (Toc != null)
			{
				clone.Toc = (Toc)Toc.Clone();
			}

			return clone;
		}
	}
}
