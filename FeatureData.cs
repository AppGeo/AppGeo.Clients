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

using System.Runtime.Serialization;

namespace AppGeo.Clients
{
  [DataContract]
  public class FeatureData
  {
    [DataMember(Name = "layerName")]
    public string LayerName = null;

    [DataMember(Name = "fieldNames")]
    public string[] FieldNames = null;

    [DataMember(Name = "rows")]
    public FeatureRow[] Rows = null;
  }
}
