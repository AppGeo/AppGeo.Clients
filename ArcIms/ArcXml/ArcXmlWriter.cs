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
	public class ArcXmlWriter : XmlWriter
	{
		private XmlWriter _innerWriter;

		public char[] CoordinateSeparator = new char[] { ' ' };
		public char[] TupleSeparator = new char[] { ';' };

		public ArcXmlWriter(XmlWriter innerWriter)
		{
			_innerWriter = innerWriter;
		}

		public XmlWriter InnerWriter
		{
			get
			{
				return _innerWriter;
			}
		}

		public override XmlWriterSettings Settings
		{
			get
			{
				return _innerWriter.Settings;
			}
		}

		public override WriteState WriteState
		{
			get
			{
				return _innerWriter.WriteState;
			}
		}

		public override string XmlLang
		{
			get
			{
				return _innerWriter.XmlLang;
			}
		}

		public override XmlSpace XmlSpace
		{
			get
			{
				return _innerWriter.XmlSpace;
			}
		}

		public override void Close()
		{
			_innerWriter.Close();
		}

		public override void Flush()
		{
			_innerWriter.Flush();
		}

		public override string LookupPrefix(string ns)
		{
			return _innerWriter.LookupPrefix(ns);
		}

		public override string ToString()
		{
			return _innerWriter.ToString();
		}

		public override void WriteAttributes(XmlReader reader, bool defattr)
		{
			_innerWriter.WriteAttributes(reader, defattr);
		}

		public override void WriteBase64(byte[] buffer, int index, int count)
		{
			_innerWriter.WriteBase64(buffer, index, count);
		}

		public override void WriteBinHex(byte[] buffer, int index, int count)
		{
			_innerWriter.WriteBinHex(buffer, index, count);
		}

		public override void WriteCData(string text)
		{
			_innerWriter.WriteCData(text);
		}

		public override void WriteCharEntity(char ch)
		{
			_innerWriter.WriteCharEntity(ch);
		}

		public override void WriteChars(char[] buffer, int index, int count)
		{
			_innerWriter.WriteChars(buffer, index, count);
		}

		public override void WriteComment(string text)
		{
			_innerWriter.WriteComment(text);
		}

		public override void WriteDocType(string name, string pubid, string sysid, string subset)
		{
			_innerWriter.WriteDocType(name, pubid, sysid, subset);
		}

		public void WriteEndArcXmlRequest()
		{
			WriteEndElement();
			WriteEndElement();
		}

		public override void WriteEndAttribute()
		{
			_innerWriter.WriteEndAttribute();
		}

		public override void WriteEndDocument()
		{
			_innerWriter.WriteEndDocument();
		}

		public override void WriteEndElement()
		{
			_innerWriter.WriteEndElement();
		}

		public override void WriteEntityRef(string name)
		{
			_innerWriter.WriteEntityRef(name);
		}

		public override void WriteFullEndElement()
		{
			_innerWriter.WriteFullEndElement();
		}

		public override void WriteName(string name)
		{
			_innerWriter.WriteName(name);
		}

		public override void WriteNmToken(string name)
		{
			_innerWriter.WriteNmToken(name);
		}

		public override void WriteNode(XmlReader reader, bool defattr)
		{
			_innerWriter.WriteNode(reader, defattr);
		}

		public override void WriteNode(System.Xml.XPath.XPathNavigator navigator, bool defattr)
		{
			_innerWriter.WriteNode(navigator, defattr);
		}

		public override void WriteProcessingInstruction(string name, string text)
		{
			_innerWriter.WriteProcessingInstruction(name, text);
		}

		public override void WriteQualifiedName(string localName, string ns)
		{
			_innerWriter.WriteQualifiedName(localName, ns);
		}

		public override void WriteRaw(char[] buffer, int index, int count)
		{
			_innerWriter.WriteRaw(buffer, index, count);
		}

		public override void WriteRaw(string data)
		{
			_innerWriter.WriteRaw(data);
		}

		public void WriteStartArcXmlRequest()
		{
			WriteStartDocument();
			WriteStartElement("ARCXML");
			WriteAttributeString("version", "1.1");
			WriteStartElement("REQUEST");
		}

		public override void WriteStartAttribute(string prefix, string localName, string ns)
		{
			_innerWriter.WriteStartAttribute(prefix, localName, ns);
		}

		public override void WriteStartDocument()
		{
			_innerWriter.WriteStartDocument();
		}

		public override void WriteStartDocument(bool standalone)
		{
			_innerWriter.WriteStartDocument(standalone);
		}

		public override void WriteStartElement(string prefix, string localName, string ns)
		{
			_innerWriter.WriteStartElement(prefix, localName, ns);
		}

		public override void WriteString(string text)
		{
			_innerWriter.WriteString(text);
		}

		public override void WriteSurrogateCharEntity(char lowChar, char highChar)
		{
			_innerWriter.WriteSurrogateCharEntity(lowChar, highChar);
		}

		public override void WriteValue(bool value)
		{
			_innerWriter.WriteValue(value);
		}

		public override void WriteValue(decimal value)
		{
			_innerWriter.WriteValue(value);
		}

		public override void WriteValue(double value)
		{
			_innerWriter.WriteValue(value);
		}

		public override void WriteValue(float value)
		{
			_innerWriter.WriteValue(value);
		}

		public override void WriteValue(int value)
		{
			_innerWriter.WriteValue(value);
		}

		public override void WriteValue(long value)
		{
			_innerWriter.WriteValue(value);
		}

		public override void WriteValue(object value)
		{
			_innerWriter.WriteValue(value);
		}

		public override void WriteValue(string value)
		{
			_innerWriter.WriteValue(value);
		}

		public override void WriteValue(DateTime value)
		{
			_innerWriter.WriteValue(value);
		}

		public override void WriteWhitespace(string ws)
		{
			_innerWriter.WriteWhitespace(ws);
		}
	}
}
