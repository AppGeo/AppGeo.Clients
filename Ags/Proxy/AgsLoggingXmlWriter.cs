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
using System.IO;
using System.Text;
using System.Xml;

namespace AppGeo.Clients.Ags.Proxy
{
  public class AgsLoggingXmlWriter : XmlTextWriter
  {
    private MemoryStream _memoryStream;
    private XmlTextWriter _logWriter;
    private int _flushCount = 0;

    public AgsLoggingXmlWriter(Stream stream) : base(stream, new UTF8Encoding())
    {
      // create a memory stream and log writer for duplicate writing

      _memoryStream = new MemoryStream();
      _logWriter = new XmlTextWriter(_memoryStream, new UTF8Encoding());
      _logWriter.Formatting = Formatting.Indented;
      _logWriter.Indentation = 2;
      _logWriter.IndentChar = ' ';
    }

    public override void Flush()
    {
      base.Flush();
      _logWriter.Flush();

      // the caller does not call Close(), Dispose() or WriteEndDocument() to signal the end of
      // writing; however it appears to call Flush() just twice, so log on the second flush

      // TODO: check if this applies to longer requests

      if (++_flushCount == 2)
      {
        // read the formatted XML from the buffer into a string and log

        _memoryStream.Position = 0;
        StreamReader streamReader = new StreamReader(_memoryStream, new System.Text.UTF8Encoding());
        AgsLogger.Log(streamReader.ReadToEnd(), "Request");

        // reset the position in the buffer, just in case more data needs to be written

        _memoryStream.Position = _memoryStream.Length - 1;
      }
    }

    // duplicate all writes to the log writer

    public override void WriteAttributes(XmlReader reader, Boolean defattr)
    {
      base.WriteAttributes(reader, defattr);
      _logWriter.WriteAttributes(reader, defattr);
    }

    public new void WriteAttributeString(String prefix, String localName, String ns, String value)
    {
      base.WriteAttributeString(prefix, localName, ns, value);
      _logWriter.WriteAttributeString(prefix, localName, ns, value);
    }

    public new void WriteAttributeString(String localName, String value)
    {
      base.WriteAttributeString(localName, value);
      _logWriter.WriteAttributeString(localName, value);
    }

    public new void WriteAttributeString(String localName, String ns, String value)
    {
      base.WriteAttributeString(localName, ns, value);
      _logWriter.WriteAttributeString(localName, ns, value);
    }

    public override void WriteBase64(Byte[] buffer, Int32 index, Int32 count)
    {
      base.WriteBase64(buffer, index, count);
      _logWriter.WriteBase64(buffer, index, count);
    }

    public override void WriteBinHex(Byte[] buffer, Int32 index, Int32 count)
    {
      base.WriteBinHex(buffer, index, count);
      _logWriter.WriteBinHex(buffer, index, count);
    }

    public override void WriteCData(String text)
    {
      base.WriteCData(text);
      _logWriter.WriteCData(text);
    }

    public override void WriteCharEntity(Char ch)
    {
      base.WriteCharEntity(ch);
      _logWriter.WriteCharEntity(ch);
    }

    public override void WriteChars(Char[] buffer, Int32 index, Int32 count)
    {
      base.WriteChars(buffer, index, count);
      _logWriter.WriteChars(buffer, index, count);
    }

    public override void WriteComment(String text)
    {
      base.WriteComment(text);
      _logWriter.WriteComment(text);
    }

    public override void WriteDocType(String name, String pubid, String sysid, String subset)
    {
      base.WriteDocType(name, pubid, sysid, subset);
      _logWriter.WriteDocType(name, pubid, sysid, subset);
    }

    public new void WriteElementString(String prefix, String localName, String ns, String value)
    {
      base.WriteElementString(prefix, localName, ns, value);
      _logWriter.WriteElementString(prefix, localName, ns, value);
    }

    public new void WriteElementString(String localName, String value)
    {
      base.WriteElementString(localName, value);
      _logWriter.WriteElementString(localName, value);
    }

    public new void WriteElementString(String localName, String ns, String value)
    {
      base.WriteElementString(localName, ns, value);
      _logWriter.WriteElementString(localName, ns, value);
    }

    public override void WriteEndAttribute()
    {
      base.WriteEndAttribute();
      _logWriter.WriteEndAttribute();
    }

    public override void WriteEndDocument()
    {
      base.WriteEndDocument();
      _logWriter.WriteEndDocument();
    }

    public override void WriteEndElement()
    {
      base.WriteEndElement();
      _logWriter.WriteEndElement();
    }

    public override void WriteEntityRef(String name)
    {
      base.WriteEntityRef(name);
      _logWriter.WriteEntityRef(name);
    }

    public override void WriteFullEndElement()
    {
      base.WriteFullEndElement();
      _logWriter.WriteFullEndElement();
    }

    public override void WriteName(String name)
    {
      base.WriteName(name);
      _logWriter.WriteName(name);
    }

    public override void WriteNmToken(String name)
    {
      base.WriteNmToken(name);
      _logWriter.WriteNmToken(name);
    }

    public override void WriteNode(System.Xml.XPath.XPathNavigator navigator, Boolean defattr)
    {
      base.WriteNode(navigator, defattr);
      _logWriter.WriteNode(navigator, defattr);
    }

    public override void WriteNode(XmlReader reader, Boolean defattr)
    {
      base.WriteNode(reader, defattr);
      _logWriter.WriteNode(reader, defattr);
    }

    public override void WriteProcessingInstruction(String name, String text)
    {
      base.WriteProcessingInstruction(name, text);
      _logWriter.WriteProcessingInstruction(name, text);
    }

    public override void WriteQualifiedName(String localName, String ns)
    {
      base.WriteQualifiedName(localName, ns);
      _logWriter.WriteQualifiedName(localName, ns);
    }

    public override void WriteRaw(Char[] buffer, Int32 index, Int32 count)
    {
      base.WriteRaw(buffer, index, count);
      _logWriter.WriteRaw(buffer, index, count);
    }

    public override void WriteRaw(String data)
    {
      base.WriteRaw(data);
      _logWriter.WriteRaw(data);
    }

    public override void WriteStartAttribute(String prefix, String localName, String ns)
    {
      base.WriteStartAttribute(prefix, localName, ns);
      _logWriter.WriteStartAttribute(prefix, localName, ns);
    }

    public new void WriteStartAttribute(String localName)
    {
      base.WriteStartAttribute(localName);
      _logWriter.WriteStartAttribute(localName);
    }

    public new void WriteStartAttribute(String localName, String ns)
    {
      base.WriteStartAttribute(localName, ns);
      _logWriter.WriteStartAttribute(localName, ns);
    }

    public override void WriteStartDocument(Boolean standalone)
    {
      base.WriteStartDocument(standalone);
      _logWriter.WriteStartDocument(standalone);
    }

    public override void WriteStartDocument()
    {
      base.WriteStartDocument();
      _logWriter.WriteStartDocument();
    }

    public override void WriteStartElement(String prefix, String localName, String ns)
    {
      base.WriteStartElement(prefix, localName, ns);
      _logWriter.WriteStartElement(prefix, localName, ns);
    }

    public new void WriteStartElement(String localName)
    {
      base.WriteStartElement(localName);
      _logWriter.WriteStartElement(localName);
    }

    public new void WriteStartElement(String localName, String ns)
    {
      base.WriteStartElement(localName, ns);
      _logWriter.WriteStartElement(localName, ns);
    }

    public override void WriteString(String text)
    {
      base.WriteString(text);
      _logWriter.WriteString(text);
    }

    public override void WriteSurrogateCharEntity(Char lowChar, Char highChar)
    {
      base.WriteSurrogateCharEntity(lowChar, highChar);
      _logWriter.WriteSurrogateCharEntity(lowChar, highChar);
    }

    public override void WriteValue(Double value)
    {
      base.WriteValue(value);
      _logWriter.WriteValue(value);
    }

    public override void WriteValue(Boolean value)
    {
      base.WriteValue(value);
      _logWriter.WriteValue(value);
    }

    public override void WriteValue(DateTime value)
    {
      base.WriteValue(value);
      _logWriter.WriteValue(value);
    }

    public override void WriteValue(Object value)
    {
      base.WriteValue(value);
      _logWriter.WriteValue(value);
    }

    public override void WriteValue(String value)
    {
      base.WriteValue(value);
      _logWriter.WriteValue(value);
    }

    public override void WriteValue(Int32 value)
    {
      base.WriteValue(value);
      _logWriter.WriteValue(value);
    }

    public override void WriteValue(Int64 value)
    {
      base.WriteValue(value);
      _logWriter.WriteValue(value);
    }

    public override void WriteValue(Decimal value)
    {
      base.WriteValue(value);
      _logWriter.WriteValue(value);
    }

    public override void WriteValue(Single value)
    {
      base.WriteValue(value);
      _logWriter.WriteValue(value);
    }

    public override void WriteWhitespace(String ws)
    {
      base.WriteWhitespace(ws);
      _logWriter.WriteWhitespace(ws);
    }
  }
}