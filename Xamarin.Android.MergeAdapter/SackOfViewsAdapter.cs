using System;
using Android.Runtime;
using Android.Views;
using Android.Widget;

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
    public class SackOfViewsAdapter : BaseAdapter, IDisposable
    {
        private JavaList<View> views = null;

        /// <summary>
        /// Constructor creating an empty list of views, but with a specified count. Subclasses must override newView().
        /// </summary>
        /// <param name='count'>
        /// Count.
        /// </param>
        public SackOfViewsAdapter(int count) : base()
        {   
            views = new JavaList<View>();
            
            for (int i=0; i < count; i++) {
                views.Add(null);
            }
        }

        /// <summary>
        /// Constructor wrapping a supplied list of views. Subclasses must override newView() if any of the elements in the list are null.
        /// </summary>
        /// <param name='views'>
        /// Views.
        /// </param>
        public SackOfViewsAdapter (JavaList<View> views) : base()
        {
            this.views = views;
        }

        public override Java.Lang.Object GetItem (int position)
        {
            return views[position];
        }

        public override int Count 
        {
            get { return views.Count; }
        }

        public override int GetItemViewType (int position)
        {
            return position;
        }

        public override bool AreAllItemsEnabled ()
        {
            return false;
        }

        public override bool IsEnabled (int position)
        {
            return false;
        }

        /// <summary>
        /// Get the row id associated with the specified position in the list.
        /// </summary>
        /// <returns>
        /// The item.
        /// </returns>
        /// <param name='position'>
        /// Position.
        /// </param>
        public override long GetItemId (int position)
        {
            return position;
        }

        /// <summary>
        /// Determines whether this instance has view in the collection
        /// </summary>
        /// <returns>
        /// <c>true</c> if this instance has view; otherwise, <c>false</c>.
        /// </returns>
        public bool HasView(View v) 
        {
            return views.Contains(v);
        }

        public override View GetView (int position, View convertView, ViewGroup parent)
        {
            View result = views[position];
            if (result == null) 
            {
                result = NewView(position, parent);
                views.Insert(position, result);
            }
            
            return result;
        }

        protected override void Dispose (bool disposing)
        {
            views = null;
            base.Dispose (disposing);
        }

        /// <summary>
        ///  Create a new View to go into the list at the specified position.
        ///  Note: You must override this method in your implementation
        /// </summary>
        protected virtual View NewView(int position, ViewGroup parent) 
        { 
            throw new Exception("You must override this method");
        }
    }
}