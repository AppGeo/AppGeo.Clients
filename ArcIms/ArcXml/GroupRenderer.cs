// © 2007-2012, Applied Geographics, Inc.  All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace AppGeo.Clients.ArcIms.ArcXml
{
  [Serializable]
	public class GroupRenderer : Renderer, IList<Renderer>
	{
		public const string XmlName = "GROUPRENDERER";

		public static GroupRenderer ReadFrom(ArcXmlReader reader)
		{
			try
			{
				GroupRenderer groupRenderer = new GroupRenderer();

				if (!reader.IsEmptyElement)
				{
					reader.Read();

					while (!(reader.NodeType == XmlNodeType.EndElement && reader.Name == XmlName))
					{
						if (reader.NodeType == XmlNodeType.Element)
						{
							switch (reader.Name)
							{
								case GroupRenderer.XmlName: groupRenderer.Add(GroupRenderer.ReadFrom(reader)); break;
								case ScaleDependentRenderer.XmlName: groupRenderer.Add(ScaleDependentRenderer.ReadFrom(reader)); break;
								case SimpleLabelRenderer.XmlName: groupRenderer.Add(SimpleLabelRenderer.ReadFrom(reader)); break;
								case SimpleRenderer.XmlName: groupRenderer.Add(SimpleRenderer.ReadFrom(reader)); break;
								case ValueMapLabelRenderer.XmlName: groupRenderer.Add(ValueMapLabelRenderer.ReadFrom(reader)); break;
								case ValueMapRenderer.XmlName: groupRenderer.Add(ValueMapRenderer.ReadFrom(reader)); break;
							}
						}

						reader.Read();
					}
				}

				return groupRenderer;
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

		private List<Renderer> _renderers = new List<Renderer>();

		public GroupRenderer() { }

    public Renderer this[int index]
    {
      get
      {
        return _renderers[index];
      }
      set
      {
        _renderers[index] = value;
      }
    }

    public int Count
    {
      get
      {
        return _renderers.Count;
      }
    }

    public bool IsReadOnly
    {
      get
      {
        return false;
      }
    }

    public void Add(Renderer item)
    {
      _renderers.Add(item);
    }

    public void Clear()
    {
      _renderers.Clear();
    }

		public override object Clone()
		{
			GroupRenderer clone = (GroupRenderer)this.MemberwiseClone();

			clone._renderers = new List<Renderer>();

			foreach (Renderer renderer in _renderers)
			{
				clone.Add((Renderer)renderer.Clone());
			}

			return clone;
		}

    public bool Contains(Renderer item)
    {
      return _renderers.Contains(item);
    }

    public void CopyTo(Renderer[] array, int arrayIndex)
    {
      _renderers.CopyTo(array, arrayIndex);
    }

    public IEnumerator<Renderer> GetEnumerator()
    {
      return _renderers.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return _renderers.GetEnumerator();
    }

    public int IndexOf(Renderer item)
    {
      return _renderers.IndexOf(item);
    }

    public void Insert(int index, Renderer item)
    {
      _renderers.Insert(index, item);
    }

    public void RemoveAt(int index)
    {
      _renderers.RemoveAt(index);
    }

    public bool Remove(Renderer item)
    {
      return _renderers.Remove(item);
    }

		public override void WriteTo(ArcXmlWriter writer)
		{
			try
			{
				writer.WriteStartElement(XmlName);

				foreach (Renderer renderer in _renderers)
				{
					renderer.WriteTo(writer);
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
