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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using AppGeo.Clients;
using AppGeo.Clients.ArcIms.ArcXml;

namespace AppGeo.Clients.ArcIms
{
	public class ArcImsHost : CommonHost
	{
    public const string DefaultServletPath = "/servlet";

    private static BooleanSwitch _debugSwitch = new BooleanSwitch("ArcXmlDebug", "Displays ArcXML requests and responses in the Debug window");
    private static BooleanSwitch _traceSwitch = new BooleanSwitch("ArcXmlTrace", "Enables the writing of ArcXML requests and responses to a trace log");

    private string _servletPath;
		private Services _services;

    public ArcImsHost(string serverUrl) : this(serverUrl, DefaultServletPath, null, null) { }

    public ArcImsHost(string serverUrl, string servletPath) : this(serverUrl, servletPath, null, null) { }

    public ArcImsHost(string serverUrl, string user, string password) : this(serverUrl, DefaultServletPath, user, password) { }

    public ArcImsHost(string serverUrl, string servletPath, string user, string password)
		{
      _servletPath = servletPath;
      
      ServerUrl = serverUrl;
			User = user;
			Password = password;

      DefaultAllowAllCertificates();

      ServerVersion = GetArcImsVersion();
      _services = (Services)Send(new GetClientServices());
		}

    private bool LoggingEnabled
    {
      get
      {
        return _debugSwitch.Enabled || _traceSwitch.Enabled;
      }
    }

    public string ServletPath
		{
			get
			{
        return _servletPath;
			}
		}

    public override List<String> AllServiceNames
    {
      get
      {
        return _services.Select(s => s.Name).ToList();
      }
    }

    public override List<String> GeocodeServiceNames
    {
      get
      {
        return _services.Where(s => s.Version != "ArcMap").Select(s => s.Name).ToList();
      }
    }

    public override List<String> MapServiceNames
    {
      get
      {
        return _services.Where(s => s.Type == "ImageServer").Select(s => s.Name).ToList();
      }
    }

		public Services Services
		{
			get
			{
				return _services;
			}
		}

    private string GetArcImsVersion()
    {
      WebRequest webRequest = null;

      string requestUrl = ServerUrl + _servletPath + "/com.esri.esrimap.Esrimap?Cmd=getVersion";
      webRequest = WebRequest.Create(requestUrl);

      webRequest.Credentials = Credentials;
      webRequest.PreAuthenticate = webRequest.Credentials != null;
      
      try
      {
        WebResponse webResponse = webRequest.GetResponse();
        Stream respStream = webResponse.GetResponseStream();
        string[] serverInfo = new StreamReader(respStream).ReadToEnd().Split(new char[] { '\n' });
        string[] version = serverInfo[0].Split(new char[] { '=' });

        return version[1];
      }
      catch (Exception ex)
      {
        string server = String.Format("ArcIMS server \"{0}\"", ServerUrl);
        string message = "{0} could not be found on the network.";

        if (ex.Message.EndsWith("Unauthorized."))
        {
          if (webRequest.Credentials == null)
          {
            message = "{0} requires a user name and password.";
          }
          else
          {
            message = "{0} did not authorize the specified user name and password.";
          }
        }

        throw new ArcImsException(String.Format(message, server), ex);
      }
    }

    public override CommonGeocodeService GetGeocodeService(string serviceName)
    {
      return new ArcImsGeocodeService(this, ValidateServiceName(serviceName));
    }

    public override CommonMapService GetMapService(string serviceName)
    {
      return new ArcImsService(this, ValidateServiceName(serviceName));
    }

    private void Log(string s)
    {
      if (_debugSwitch.Enabled)
      {
        Debugger.Log(0, "ArcXML", s);
      }

      if (_traceSwitch.Enabled)
      {
        Trace.Write(s);
      }
    }

		public void Reload()
		{
			_services = Send(new GetClientServices());
		}

    private Response Receive(WebResponse webResponse)
    {
      Response response = null;
      Error error = null;

      try
      {
        // open the response stream; if debugging or tracing, read the stream
        // into a string for display then open an XmlReader on the string,
        // otherwise open an XmlReader directly on the stream

        Stream stream = webResponse.GetResponseStream();
        XmlTextReader xmlTextReader;

        if (LoggingEnabled)
        {
          StreamReader streamReader = new StreamReader(stream, new UTF8Encoding(false));
          string responseString = streamReader.ReadToEnd();

          Log(String.Format("{0}  --  ArcXML Response\n\n", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")));
          Log(responseString + "\n\n");

          xmlTextReader = new XmlTextReader(new StringReader(responseString));
        }
        else
        {
          xmlTextReader = new XmlTextReader(stream);
        }

        ArcXmlReader reader = new ArcXmlReader(xmlTextReader);

        // handle an error response from the Application Server

        if (reader.Name == Error.XmlName)
        {
          error = Error.ReadFrom(reader);
          throw new ArcImsException("ArcIMS Application Server - " + error.Text);
        }

        // move to the specific response element and deserialize it

        reader.MoveToResponse();

        switch (reader.Name)
        {
          case ServiceInfo.XmlName: response = ServiceInfo.ReadFrom(reader); break;
          case Image.XmlName: response = Image.ReadFrom(reader); break;
          case Layout.XmlName: response = Layout.ReadFrom(reader); break;
          case Geocode.XmlName: response = Geocode.ReadFrom(reader); break;
          case Features.XmlName: response = Features.ReadFrom(reader); break;
          case Services.XmlName: response = Services.ReadFrom(reader); break;

          case Error.XmlName:
            error = Error.ReadFrom(reader);
            throw new ArcImsException("ArcIMS Spatial Server - " + error.Text);

          default:
            throw new ArcImsException("Unsupported ArcXML response received from ArcIMS server: " + reader.Name);
        }
      }
      catch (Exception ex)
      {
        if (ex is ArcImsException)
        {
          throw ex;
        }
        else
        {
          throw new ArcImsException("Could not receive ArcXML response", ex);
        }
      }
      finally
      {
        webResponse.Close();
      }

      return response;
    }

    public Services Send(GetClientServices request)
    {
      WebResponse webResponse = Send("catalog", request);
      return (Services)Receive(webResponse);
    }

    public Response Send(ArcImsService service, Request request)
    {
      WebResponse webResponse = Send(service.Name, request);
      return Receive(webResponse);
    }

    public Response Send(ArcImsGeocodeService service, Request request)
    {
      WebResponse webResponse = Send(service.Name, request);
      return Receive(webResponse);
    }

    private WebResponse Send(string serviceName, Request request)
    {
      WebResponse webResponse = null;

      // build the request URL

      string requestUrl = ServerUrl + _servletPath + "/com.esri.esrimap.Esrimap?ServiceName=" + serviceName;

      if (request is GetFeatures)
      {
        requestUrl += "&CustomService=Query";
      }

      if (request is GetGeocode || (request is GetServiceInfo && ((GetServiceInfo)request).ForGeocoding))
      {
        requestUrl += "&CustomService=Geocode";
      }

      // send the ArcXML request to the ArcIMS server

      try
      {
        // prepare the web request: set credentials if necessary and
        // accept all certificates

        WebRequest webRequest = WebRequest.Create(requestUrl);
        webRequest.Method = "POST";
        webRequest.ContentType = "text/xml";

        webRequest.Credentials = Credentials;
        webRequest.PreAuthenticate = webRequest.Credentials != null;

        // if debugging or tracing, capture the ArcXML into a memory stream, otherwise
        // send it directly to the server

        Stream stream;

        if (LoggingEnabled)
        {
          stream = new MemoryStream();
        }
        else
        {
          stream = webRequest.GetRequestStream();
        }

        // write the ArcXML request

        XmlTextWriter xmlTextWriter = new XmlTextWriter(stream, new UTF8Encoding(false));
        ArcXmlWriter writer = new ArcXmlWriter(xmlTextWriter);

        if (LoggingEnabled)
        {
          xmlTextWriter.Formatting = Formatting.Indented;
        }

        if (request is GetClientServices)
        {
          writer.WriteStartDocument();
          request.WriteTo(writer);
        }
        else
        {
          writer.WriteStartArcXmlRequest();
          request.WriteTo(writer);
          writer.WriteEndArcXmlRequest();
        }

        writer.Close();

        // if debugging or tracing, display the ArcXML that was generated for the
        // request and then send it to the server

        if (LoggingEnabled)
        {
          byte[] requestBytes = ((MemoryStream)stream).ToArray();

          Log("\n==========================================\n\n");
          Log(String.Format("{0}  --  ArcXML Request\n\n", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")));
          Log((new UTF8Encoding()).GetString(requestBytes));
          Log("\n\n");

          stream = webRequest.GetRequestStream();
          BinaryWriter binaryWriter = new BinaryWriter(stream);
          binaryWriter.Write(requestBytes);
          binaryWriter.Close();
        }

        webResponse = webRequest.GetResponse();
      }
      catch (Exception ex)
      {
        if (ex is ArcXmlException)
        {
          throw ex;
        }
        else
        {
          throw new ArcImsException("Unable to communicate with the ArcIMS server", ex);
        }
      }

      return webResponse;
    }

    private string ValidateServiceName(string serviceName)
    {
      Service service = _services.FirstOrDefault(o => String.Compare(serviceName, o.Name, true) == 0);

      if (service == null)
      {
        throw new ArcImsException(String.Format("The service \"{0}\" does not exist on the ArcIMS server", serviceName));
      }

      if (service.Status == ServiceStatus.Disabled)
      {
        throw new ArcImsException(String.Format("The service \"{0}\" is currently disabled on the ArcIMS server"));
      }

      return service.Name;
    }
  }
}
