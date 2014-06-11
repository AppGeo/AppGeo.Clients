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
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace AppGeo.Clients
{
  [Serializable]
  public abstract class CommonHost
  {
    private string _serverUrl = null;
    private string _serverVersion = null;

    private string _domain = null;
    private string _user = null;
    private string _password = null;

    public abstract List<String> AllServiceNames { get; }

    internal NetworkCredential Credentials
    {
      get
      {
        if (!String.IsNullOrEmpty(_user) && !String.IsNullOrEmpty(_password))
        {
          if (String.IsNullOrEmpty(_domain))
          {
            return new NetworkCredential(_user, _password);
          }
          else
          {
            return new NetworkCredential(_user, _password, _domain);
          }
        }

        return null;
      }
    }

    public string Domain
    {
      get
      {
        return _domain;
      }
      protected set
      {
        _domain = value;
      }
    }

    public abstract List<String> GeocodeServiceNames { get; }

    public abstract List<String> MapServiceNames { get; }

    public string Password
    {
      get
      {
        return _password;
      }
      protected set
      {
        _password = value;
      }
    }

    public string ServerUrl
    {
      get
      {
        return _serverUrl;
      }
      protected set
      {
        _serverUrl = value;
      }
    }

    public string ServerVersion
    {
      get
      {
        return _serverVersion;
      }
      protected set
      {
        _serverVersion = value;
      }
    }

    public string User
    {
      get
      {
        return _user;
      }
      protected set
      {
        if (String.IsNullOrEmpty(value))
        {
          _domain = null;
          _user = null;
        }
        else
        {
          string[] tokens = value.Split(new char[] { '\\' });

          _domain = tokens.Length == 1 ? null : tokens[0];
          _user = tokens.Length == 1 ? tokens[0] : tokens[1];
        }
      }
    }

    protected void DefaultAllowAllCertificates()
    {
      if (ServicePointManager.ServerCertificateValidationCallback == null)
      {
        ServicePointManager.ServerCertificateValidationCallback += delegate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
      }
    }

    public abstract CommonMapService GetMapService(string serviceName);

    public abstract CommonGeocodeService GetGeocodeService(string serviceName);
  }
}
