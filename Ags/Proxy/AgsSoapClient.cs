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

namespace AppGeo.Clients.Ags.Proxy
{
  public class AgsSoapClient : System.Web.Services.Protocols.SoapHttpClientProtocol
  {
    protected override XmlReader GetReaderForMessage(System.Web.Services.Protocols.SoapClientMessage message, int bufferSize)
    {
      if (!AgsLogger.IsLogging)
      {
        return base.GetReaderForMessage(message, bufferSize);
      }
      else
      {
        return new AgsLoggingXmlReader(message.Stream, bufferSize);
      }
    }

    protected override XmlWriter GetWriterForMessage(System.Web.Services.Protocols.SoapClientMessage message, int bufferSize)
    {
      if (!AgsLogger.IsLogging)
      {
        return base.GetWriterForMessage(message, bufferSize);
      }
      else
      {
        return new AgsLoggingXmlWriter(message.Stream);
      }
    }
  }
}
