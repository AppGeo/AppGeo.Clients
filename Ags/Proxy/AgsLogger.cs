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
using System.Diagnostics;

namespace AppGeo.Clients.Ags.Proxy
{
  public static class AgsLogger
  {
    private static BooleanSwitch _debugSwitch = new BooleanSwitch("AgsDebug", "Displays ArcGIS Server requests and responses in the Debug window");
    private static BooleanSwitch _traceSwitch = new BooleanSwitch("AgsTrace", "Enables the writing of ArcGIS Server requests and responses to a trace log");

    public static bool IsLogging
    {
      get
      {
        return _debugSwitch.Enabled || _traceSwitch.Enabled;
      }
    }

    public static void Log(string message, string type)
    {
      message = String.Format("{0:yyyy-MM-dd hh:mm:ss}  --  ArcGIS Server {1}\n\n{2}\n\n", DateTime.Now, type, message);

      if (_debugSwitch.Enabled)
      {
        Debugger.Log(0, "AGS", message);
      }

      if (_traceSwitch.Enabled)
      {
        Trace.Write(message);
      }
    }
  }
}
