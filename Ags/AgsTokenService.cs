//  Copyright 2014 Applied Geographics, Inc.
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
using System.Net;

namespace AppGeo.Clients.Ags
{
  [Serializable]
  public class AgsTokenService
  {
    private static object GetTokenLock = new object();

    private AgsAuthenticationToken _token = null;
    private string _requestUrl = null;
    private bool _isPortal = false;

    public AgsTokenService(string tokenUrl, string serverUrl, string user, string password)
    {
      TokenUrl = tokenUrl;
      ServerUrl = serverUrl.Replace("/services", "");
      User = user;
      Password = password;

      _requestUrl = TokenUrl;
      _isPortal = _requestUrl.EndsWith("/sharing/rest/generateToken");

      if (!_isPortal)
      {
        _requestUrl += "generateToken";
      }
    }

    public string TokenUrl { get; private set; }
    public string ServerUrl { get; private set; }
    public string User { get; private set; }
    public string Password { get; private set; }

    public AgsAuthenticationToken GetToken()
    {
      lock (GetTokenLock)
      {
        if (_token != null && _token.Expires.AddSeconds(-5) < DateTime.Now)
        {
          _token = null;
        }

        if (_token == null)
        {
          if (!_isPortal)
          {
            string json = RequestToken(String.Format("f=json&username={0}&password={1}&client=requestip", User, Password));
            _token = AgsAuthenticationToken.Deserialize(json);
          }
          else
          {
            string json = RequestToken(String.Format("f=json&username={0}&password={1}&referer=x", User, Password));
            _token = AgsAuthenticationToken.Deserialize(json);

            json = RequestToken(String.Format("f=json&username={0}&password={1}&referer=x&token={2}&serverURL={3}", User, Password, _token.Value, ServerUrl));
            _token = AgsAuthenticationToken.Deserialize(json);
          }
        }
      }

      return _token;
    }

    private string RequestToken(string data)
    {
      string json;

      try
      {
        WebRequest request = WebRequest.Create(_requestUrl);
        request.Method = "POST";
        request.ContentType = "application/x-www-form-urlencoded";

        using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
        {
          writer.Write(data);
        }

        WebResponse response = request.GetResponse();

        using (StreamReader reader = new StreamReader(response.GetResponseStream()))
        {
          json = reader.ReadToEnd();
        }
      }
      catch (Exception ex)
      {
        throw new AgsException("Error communicating with the ArcGIS Server token service", ex);
      }

      return json;
    }
  }
}
