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
	public class ArcXmlReader : XmlReader
	{
		private XmlReader _innerReader;

		public char[] CoordinateSeparator = new char[] {' '};
		public char[] TupleSeparator = new char[] {';'};

		public ArcXmlReader(XmlReader innerReader)
		{
			_innerReader = innerReader;

			if (_innerReader is XmlTextReader)
			{
				((XmlTextReader)_innerReader).WhitespaceHandling = WhitespaceHandling.None;
			}

			_innerReader.MoveToContent();
		}

		public XmlReader InnerReader
		{
			get
			{
				return _innerReader;
			}
		}

		public override int AttributeCount
		{
			get
			{
				return _innerReader.AttributeCount;
			}
		}

		public override string BaseURI
		{
			get
			{
				return _innerReader.BaseURI;
			}
		}

		public override bool CanReadBinaryContent
		{
			get
			{
				return _innerReader.CanReadBinaryContent;
			}
		}

		public override bool CanReadValueChunk
		{
			get
			{
				return _innerReader.CanReadValueChunk;
			}
		}

		public override bool CanResolveEntity
		{
			get
			{
				return _innerReader.CanResolveEntity;
			}
		}

		public override int Depth
		{
			get
			{
				return _innerReader.Depth;
			}
		}

		public override bool EOF
		{
			get
			{
				return _innerReader.EOF;
			}
		}

		public override bool HasAttributes
		{
			get
			{
				return _innerReader.HasAttributes;
			}
		}

		public override bool HasValue
		{
			get
			{
				return _innerReader.HasValue;
			}
		}

		public override bool IsDefault
		{
			get
			{
				return _innerReader.IsDefault;
			}
		}

		public override bool IsEmptyElement
		{
			get
			{
				return _innerReader.IsEmptyElement;
			}
		}

		public override string LocalName
		{
			get
			{
				return _innerReader.LocalName;
			}
		}

		public override string Name
		{
			get
			{
				return _innerReader.Name;
			}
		}

		public override XmlNameTable NameTable
		{
			get
			{
				return _innerReader.NameTable;
			}
		}

		public override string NamespaceURI
		{
			get
			{
				return _innerReader.NamespaceURI;
			}
		}

		public override XmlNodeType NodeType
		{
			get
			{
				return _innerReader.NodeType;
			}
		}

		public override string Prefix
		{
			get
			{
				return _innerReader.Prefix;
			}
		}

		public override char QuoteChar
		{
			get
			{
				return _innerReader.QuoteChar;
			}
		}

		public override ReadState ReadState
		{
			get
			{
				return _innerReader.ReadState;
			}
		}

		public override System.Xml.Schema.IXmlSchemaInfo SchemaInfo
		{
			get
			{
				return _innerReader.SchemaInfo;
			}
		}

		public override XmlReaderSettings Settings
		{
			get
			{
				return _innerReader.Settings;
			}
		}

		public override string this[int i]
		{
			get
			{
				return _innerReader[i];
			}
		}

		public override string this[string name, string namespaceURI]
		{
			get
			{
				return _innerReader[name, namespaceURI];
			}
		}

		public override string this[string name]
		{
			get
			{
				return _innerReader[name];
			}
		}

		public override string Value
		{
			get
			{
				return _innerReader.Value;
			}
		}

		public override Type ValueType
		{
			get
			{
				return _innerReader.ValueType;
			}
		}

		public override string XmlLang
		{
			get
			{
				return _innerReader.XmlLang;
			}
		}

		public override XmlSpace XmlSpace
		{
			get
			{
				return _innerReader.XmlSpace;
			}
		}

		public override void Close()
		{
			_innerReader.Close();
		}

		public override string GetAttribute(int i)
		{
			return _innerReader.GetAttribute(i);
		}

		public override string GetAttribute(string name, string namespaceURI)
		{
			return _innerReader.GetAttribute(name, namespaceURI);
		}

		public override string GetAttribute(string name)
		{
			return _innerReader.GetAttribute(name);
		}

		public override bool IsStartElement()
		{
			return _innerReader.IsStartElement();
		}

		public override bool IsStartElement(string localname, string ns)
		{
			return _innerReader.IsStartElement(localname, ns);
		}

		public override bool IsStartElement(string name)
		{
			return _innerReader.IsStartElement(name);
		}

		public override string LookupNamespace(string prefix)
		{
			return _innerReader.LookupNamespace(prefix);
		}

		public override void MoveToAttribute(int i)
		{
			_innerReader.MoveToAttribute(i);
		}

		public override bool MoveToAttribute(string name, string ns)
		{
			return _innerReader.MoveToAttribute(name, ns);
		}

		public override bool MoveToAttribute(string name)
		{
			return _innerReader.MoveToAttribute(name);
		}

		public override XmlNodeType MoveToContent()
		{
			return _innerReader.MoveToContent();
		}

		public override bool MoveToElement()
		{
			return _innerReader.MoveToElement();
		}

		public override bool MoveToFirstAttribute()
		{
			return _innerReader.MoveToFirstAttribute();
		}

		public override bool MoveToNextAttribute()
		{
			return _innerReader.MoveToNextAttribute();
		}

		public bool MoveToResponse()
		{
			bool result = false;

			try
			{
				MoveToContent();

				if (ReadToDescendant("RESPONSE"))
				{
					result = Read();
				}
			}
			catch { }

			return result;
		}

		public override bool Read()
		{
			return _innerReader.Read();
		}

		public override bool ReadAttributeValue()
		{
			return _innerReader.ReadAttributeValue();
		}

		public override object ReadContentAs(Type returnType, IXmlNamespaceResolver namespaceResolver)
		{
			return _innerReader.ReadContentAs(returnType, namespaceResolver);
		}

		public override int ReadContentAsBase64(byte[] buffer, int index, int count)
		{
			return _innerReader.ReadContentAsBase64(buffer, index, count);
		}

		public override int ReadContentAsBinHex(byte[] buffer, int index, int count)
		{
			return _innerReader.ReadContentAsBinHex(buffer, index, count);
		}

		public override bool ReadContentAsBoolean()
		{
			return _innerReader.ReadContentAsBoolean();
		}

		public override DateTime ReadContentAsDateTime()
		{
			return _innerReader.ReadContentAsDateTime();
		}

		public override decimal ReadContentAsDecimal()
		{
			return _innerReader.ReadContentAsDecimal();
		}

		public override double ReadContentAsDouble()
		{
			return _innerReader.ReadContentAsDouble();
		}

		public override float ReadContentAsFloat()
		{
			return _innerReader.ReadContentAsFloat();
		}

		public override int ReadContentAsInt()
		{
			return _innerReader.ReadContentAsInt();
		}

		public override long ReadContentAsLong()
		{
			return _innerReader.ReadContentAsLong();
		}

		public override object ReadContentAsObject()
		{
			return _innerReader.ReadContentAsObject();
		}

		public override string ReadContentAsString()
		{
			return _innerReader.ReadContentAsString();
		}

		public override object ReadElementContentAs(Type returnType, IXmlNamespaceResolver namespaceResolver)
		{
			return _innerReader.ReadElementContentAs(returnType, namespaceResolver);
		}

		public override object ReadElementContentAs(Type returnType, IXmlNamespaceResolver namespaceResolver, string localName, string namespaceURI)
		{
			return _innerReader.ReadElementContentAs(returnType, namespaceResolver, localName, namespaceURI);
		}

		public override int ReadElementContentAsBase64(byte[] buffer, int index, int count)
		{
			return _innerReader.ReadElementContentAsBase64(buffer, index, count);
		}

		public override int ReadElementContentAsBinHex(byte[] buffer, int index, int count)
		{
			return _innerReader.ReadElementContentAsBinHex(buffer, index, count);
		}

		public override bool ReadElementContentAsBoolean()
		{
			return _innerReader.ReadElementContentAsBoolean();
		}

		public override bool ReadElementContentAsBoolean(string localName, string namespaceURI)
		{
			return _innerReader.ReadElementContentAsBoolean(localName, namespaceURI);
		}

		public override DateTime ReadElementContentAsDateTime()
		{
			return _innerReader.ReadElementContentAsDateTime();
		}

		public override DateTime ReadElementContentAsDateTime(string localName, string namespaceURI)
		{
			return _innerReader.ReadElementContentAsDateTime(localName, namespaceURI);
		}

		public override decimal ReadElementContentAsDecimal()
		{
			return _innerReader.ReadElementContentAsDecimal();
		}

		public override decimal ReadElementContentAsDecimal(string localName, string namespaceURI)
		{
			return _innerReader.ReadElementContentAsDecimal(localName, namespaceURI);
		}

		public override double ReadElementContentAsDouble()
		{
			return _innerReader.ReadElementContentAsDouble();
		}

		public override double ReadElementContentAsDouble(string localName, string namespaceURI)
		{
			return _innerReader.ReadElementContentAsDouble(localName, namespaceURI);
		}

		public override float ReadElementContentAsFloat()
		{
			return _innerReader.ReadElementContentAsFloat();
		}

		public override float ReadElementContentAsFloat(string localName, string namespaceURI)
		{
			return _innerReader.ReadElementContentAsFloat(localName, namespaceURI);
		}

		public override int ReadElementContentAsInt()
		{
			return _innerReader.ReadElementContentAsInt();
		}

		public override int ReadElementContentAsInt(string localName, string namespaceURI)
		{
			return _innerReader.ReadElementContentAsInt(localName, namespaceURI);
		}

		public override long ReadElementContentAsLong()
		{
			return _innerReader.ReadElementContentAsLong();
		}

		public override long ReadElementContentAsLong(string localName, string namespaceURI)
		{
			return _innerReader.ReadElementContentAsLong(localName, namespaceURI);
		}

		public override object ReadElementContentAsObject()
		{
			return _innerReader.ReadElementContentAsObject();
		}

		public override object ReadElementContentAsObject(string localName, string namespaceURI)
		{
			return _innerReader.ReadElementContentAsObject(localName, namespaceURI);
		}

		public override string ReadElementContentAsString()
		{
			return _innerReader.ReadElementContentAsString();
		}

		public override string ReadElementContentAsString(string localName, string namespaceURI)
		{
			return _innerReader.ReadElementContentAsString(localName, namespaceURI);
		}

		public override string ReadElementString()
		{
			return _innerReader.ReadElementString();
		}

		public override string ReadElementString(string localname, string ns)
		{
			return _innerReader.ReadElementString(localname, ns);
		}

		public override string ReadElementString(string name)
		{
			return _innerReader.ReadElementString(name);
		}

		public override void ReadEndElement()
		{
			_innerReader.ReadEndElement();
		}

		public override string ReadInnerXml()
		{
			return _innerReader.ReadInnerXml();
		}

		public override string ReadOuterXml()
		{
			return _innerReader.ReadOuterXml();
		}

		public override void ReadStartElement()
		{
			_innerReader.ReadStartElement();
		}

		public override void ReadStartElement(string localname, string ns)
		{
			_innerReader.ReadStartElement(localname, ns);
		}

		public override void ReadStartElement(string name)
		{
			_innerReader.ReadStartElement(name);
		}

		public override string ReadString()
		{
			return _innerReader.ReadString();
		}

		public override XmlReader ReadSubtree()
		{
			return _innerReader.ReadSubtree();
		}

		public override bool ReadToDescendant(string localName, string namespaceURI)
		{
			return _innerReader.ReadToDescendant(localName, namespaceURI);
		}

		public override bool ReadToDescendant(string name)
		{
			return _innerReader.ReadToDescendant(name);
		}

		public override bool ReadToFollowing(string localName, string namespaceURI)
		{
			return _innerReader.ReadToFollowing(localName, namespaceURI);
		}

		public override bool ReadToFollowing(string name)
		{
			return _innerReader.ReadToFollowing(name);
		}

		public override bool ReadToNextSibling(string localName, string namespaceURI)
		{
			return _innerReader.ReadToNextSibling(localName, namespaceURI);
		}

		public override bool ReadToNextSibling(string name)
		{
			return _innerReader.ReadToNextSibling(name);
		}

		public override int ReadValueChunk(char[] buffer, int index, int count)
		{
			return _innerReader.ReadValueChunk(buffer, index, count);
		}

		public override void ResolveEntity()
		{
			_innerReader.ResolveEntity();
		}

		public override void Skip()
		{
			_innerReader.Skip();
		}

		public override string ToString()
		{
			return _innerReader.ToString();
		}
	}
}
