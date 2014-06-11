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
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AppGeo.Clients.Ags
{
  [Serializable]
  public class AgsAuthenticationToken
  {
    private static Regex JsonRegex = new Regex("^{\"token\":\"(\\S+)\",\"expires\":(\\d+)}$");

    public static AgsAuthenticationToken Deserialize(string json)
    {
      Match match = JsonRegex.Match(json);

      if (match.Success)
      {
        double milliseconds = Convert.ToDouble(match.Groups[2].Captures[0].Value);
        DateTime utcExpires = (new DateTime(1970, 1, 1)).AddMilliseconds(milliseconds);
        DateTime localExpires = TimeZone.CurrentTimeZone.ToLocalTime(utcExpires);

        return new AgsAuthenticationToken
        {
          Value = match.Groups[1].Captures[0].Value,
          Expires = localExpires
        };
      }
      else
      {
        throw new AgsException("Unable to deserialize authentication token");
      }
    }

    public string Value { get; private set; }
    public DateTime Expires { get; private set; }

    public string SignUrl(string url)
    {
      string tokenParameter = "token=" + Value;
      UriBuilder uri = new UriBuilder(url);

      if (!String.IsNullOrEmpty(uri.Query))
      {
        uri.Query = uri.Query.Substring(1) + "&" + tokenParameter;
      }
      else
      {
        uri.Query = tokenParameter;
      }

      return uri.ToString();
    }
  }
}
