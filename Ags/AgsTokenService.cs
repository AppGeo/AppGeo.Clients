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

    public AgsTokenService(string url, string user, string password)
    {
      Url = url;
      User = user;
      Password = password;
    }

    public string Url { get; private set; }
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
          string requestUrl = Url + "generateToken";
          string data = String.Format("f=json&username={0}&password={1}&client=requestip", User, Password);
          string json;

          try
          {
            WebRequest request = WebRequest.Create(requestUrl);
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

          _token = AgsAuthenticationToken.Deserialize(json);
        }
      }

      return _token;
    }
  }
}
