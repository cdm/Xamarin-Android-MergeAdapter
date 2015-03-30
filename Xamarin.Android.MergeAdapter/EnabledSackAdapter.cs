using System;
using Android.Runtime;
using Android.Views;

/***
  C# Port Copyright (c) 2012 @cdm
  Original Java impl (c) 2008-2009 CommonsWare, LLC & portions 2009 Google, Inc.
  
  Licensed under the Apache License, Version 2.0 (the "License"); you may
  not use this file except in compliance with the License. You may obtain
  a copy of the License at
    http://www.apache.org/licenses/LICENSE-2.0
  Unless required by applicable law or agreed to in writing, software
  distributed under the License is distributed on an "AS IS" BASIS,
  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
  See the License for the specific language governing permissions and
  limitations under the License.
*/

namespace Xamarin.Android.MergeAdapter
{
    /// <summary>
    /// Enabled sack adapter.
    /// </summary>
    public class EnabledSackAdapter : SackOfViewsAdapter
    {
        public EnabledSackAdapter(JavaList<View> views) : base (views)
        { }
        
        public override bool AreAllItemsEnabled() {
            return true;
        }
        
        public override bool IsEnabled(int position) {
            return true;
        }
    }
}

