using System;
using Android.Runtime;
using Android.Database;
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
    /// <summary>
    /// Merge adapter, allows varied views and list adapters to be easily bound to an Android list view.
    /// </summary>
    public class MergeAdapter : BaseAdapter, ISectionIndexer, IDisposable
    {
        protected PieceStateHolder pieces = new PieceStateHolder();

        /// <summary>
        /// Initializes a new instance of the <see cref="Xamarin.Android.MergeAdapter.MergeAdapter"/> class.
        /// </summary>
        public MergeAdapter () : base()
        { }

        /// <summary>
        /// Adds a new adapter to the roster of things to appear in the aggregate list.
        /// </summary>
        /// <param name='adapter'>
        /// Source for row views for this section
        /// </param>
        public void AddAdapter (IListAdapter adapter)
        {
            pieces.Add(adapter);
            var cdso = new CascadeDataSetObserver();
            cdso.OnDataSetChanged += delegate {
                this.NotifyDataSetChanged();
            };
            cdso.OnDataSetInvalidated += delegate {
                this.NotifyDataSetInvalidated();
            };
            adapter.RegisterDataSetObserver(cdso);
        }

        /// <summary>
        /// Adds a new View to the roster of things to appear in the aggregate list.
        /// </summary>
        /// <param name='view'>
        /// Single view to add.
        /// </param>
        public void AddView(View view) {
            AddView(view, false);
        }

        /// <summary>
        /// Adds a new View to the roster of things to appear in the aggregate list.
        /// </summary>
        /// <param name='view'>
        ///  Single view to add
        /// </param>
        /// <param name='enabled'>
        /// false if views are disabled, true if enabled
        /// </param>
        public void AddView(View view, bool enabled) {
            JavaList<View> list = new JavaList<View>() { view };
            AddViews(list, enabled);
        }

        /// <summary>
        /// Adds a list of views to the roster of things to appear in the aggregate list.
        /// </summary>
        /// <param name='views'>
        /// List of views to add
        /// </param>
        public void AddViews(JavaList<View> views) {
            AddViews(views, false);
        }

        /// <summary>
        /// Adds a list of views to the roster of things to appear in the aggregate list.
        /// </summary>
        /// <param name='views'>
        /// List of views to add
        /// </param>
        /// <param name='enabled'>
        /// false if views are disabled, true if enabled
        /// </param>
        public void AddViews(JavaList<View> views, bool enabled) {
            if (enabled) {
                AddAdapter(new EnabledSackAdapter(views));
            }
            else {
                AddAdapter(new SackOfViewsAdapter(views));
            }
        }

        /// <summary>
        /// Get the data item associated with the specified  position in the data set.
        /// </summary>
        /// <returns>
        /// The item.
        /// </returns>
        /// <param name='position'>
        /// Get the data item associated with the specified position in the data set.
        /// </param>
        public override Java.Lang.Object GetItem (int position)
        {
            foreach (IListAdapter piece in GetPieces()) 
            {
                int size = piece.Count;
                if (position < size) return piece.GetItem(position);
                position -= size;
            }
            
            return null;
        }

        /// <summary>
        /// Get the adapter associated with the specified position in the data set.
        /// </summary>
        /// <returns>
        /// The adapter.
        /// </returns>
        /// <param name='position'>
        /// Position of the item whose adapter we want
        /// </param>
        public IListAdapter GetAdapter (int position)
        {
            foreach (IListAdapter piece in GetPieces()) 
            {
                int size = piece.Count;
                if (position < size) return piece;
                position -= size;
            }
            
            return null;
        }

        /// <summary>
        ///  How many items are in the data set represented by this Adapter.
        /// </summary>
        /// <value>
        /// The count of items.
        /// </value>
        public override int Count {
            get 
            {
                int total = 0;
                foreach (IListAdapter piece in GetPieces()) {
                    total += piece.Count;
                }
                return total;
            }
        }

        /// <summary>
        /// Returns the number of types of Views that will be created by GetView().
        /// </summary>
        /// <value>
        /// The view type count.
        /// </value>
        public override int ViewTypeCount {
            get 
            {
                int total = 0;
                foreach (PieceState piece in pieces.GetRawPieces()) {
                    total += piece.Adapter.ViewTypeCount;
                }
                return Math.Max(total, 1); 
            }
        }

        /// <summary>
        /// Get the type of View that will be created by getView() for the specified item.
        /// </summary>
        /// <returns>
        /// The item.
        /// </returns>
        /// <param name='position'>
        ///  Position of the item whose data we want
        /// </param>
        public override int GetItemViewType (int position)
        {
            int typeOffset = 0;
            int result = -1;

            foreach (PieceState piece in pieces.GetRawPieces()) {
                if (piece.IsActive) {
                    int size = piece.Adapter.Count;
                    if (position < size) {
                        result = typeOffset + piece.Adapter.GetItemViewType(position);
                        break;
                    }
                    
                    position -= size;
                }

                typeOffset += piece.Adapter.ViewTypeCount;
            }

            return result;
        }

        /// <Docs>Indicates whether all the items in this adapter are enabled.</Docs>
        /// <remarks>Indicates whether all the items in this adapter are enabled. If the
        ///  value returned by this method changes over time, there is no guarantee
        ///  it will take effect. If true, it means all items are selectable and
        ///  clickable (there is no separator.)</remarks>
        /// <format type="text/html">[Android Documentation]</format>
        /// <since version="Added in API level 1"></since>
        /// <summary>
        /// Ares all items enabled.
        /// </summary>
        /// <returns><c>true</c>, if all items enabled was ared, <c>false</c> otherwise.</returns>
        public override bool AreAllItemsEnabled ()
        {
            return false;
        }

        /// <Docs>Index of the item</Docs>
        /// <returns>To be added.</returns>
        /// <summary>
        /// Determines whether this instance is enabled the specified position.
        /// </summary>
        /// <param name="position">Position.</param>
        public override bool IsEnabled (int position)
        {
            foreach (IListAdapter piece in GetPieces()) {
                int size = piece.Count;
                if (position < size) {
                    return piece.IsEnabled(position);
                }
                position-=size;
            }
            return false;
        }

        /// <summary>
        /// Get a View that displays the data at the specified position in the data set.
        /// </summary>
        /// <returns>
        /// The view.
        /// </returns>
        /// <param name='position'>
        ///  Position of the item whose data we want
        /// </param>
        /// <param name='convertView'>
        /// View to recycle, if not null
        /// </param>
        /// <param name='parent'>
        ///  ViewGroup containing the returned View
        /// </param>
        public override View GetView (int position, View convertView, ViewGroup parent)
        {
            foreach (IListAdapter piece in GetPieces()) {
                int size = piece.Count;
                if (position < size) {
                    return piece.GetView(position, convertView, parent);
                }
                position -= size;
            }            
            return null;
        }

        /// <summary>
        /// Get the row id associated with the specified position in the list.
        /// </summary>
        /// <returns>
        /// The item identifier.
        /// </returns>
        /// <param name='position'>
        /// Position of the item whose data we want
        /// </param>
        public override long GetItemId (int position)
        {
            foreach (IListAdapter piece in GetPieces()) {
                int size = piece.Count;
                if (position < size)
                {
                    return piece.GetItemId(position);
                }
                position -= size;
            }
            return -1;
        }

        /// <Docs>the index of the section to jump to.</Docs>
        /// <returns>To be added.</returns>
        /// <para tool="javadoc-to-mdoc">Provides the starting index in the list for a given section.</para>
        /// <format type="text/html">[Android Documentation]</format>
        /// <since version="Added in API level 3"></since>
        /// <summary>
        /// Gets the position for section.
        /// </summary>
        /// <param name="section">Section.</param>
        public int GetPositionForSection (int section)
        {
            int position = 0;
            
            foreach (IListAdapter piece in GetPieces()) {
                if (piece is ISectionIndexer) 
                {
                    Object[] sections=((ISectionIndexer)piece).GetSections();
                    int numSections=0;
                    if (sections != null) {
                        numSections=sections.Length;
                    }
                    if (section < numSections) {
                        return position + ((ISectionIndexer)piece).GetPositionForSection(section);
                    }
                    else if (sections != null) {
                        section -= numSections;
                    }
                }
                position+=piece.Count;
            }
            
            return 0;
        }

        /// <Docs>the position for which to return the section</Docs>
        /// <returns>To be added.</returns>
        /// <para tool="javadoc-to-mdoc">This is a reverse mapping to fetch the section index for a given position
        ///  in the list.</para>
        /// <format type="text/html">[Android Documentation]</format>
        /// <since version="Added in API level 3"></since>
        /// <summary>
        /// Gets the section for position.
        /// </summary>
        /// <param name="position">Position.</param>
        public int GetSectionForPosition (int position)
        {
            int section = 0;
            
            foreach (IListAdapter piece in GetPieces()) {
                int size=piece.Count;
                
                if (position < size) 
                {
                    if (piece is ISectionIndexer) {
                        return(section + ((ISectionIndexer)piece).GetSectionForPosition(position));
                    }
                    return 0;
                }
                else 
                {
                    if (piece is ISectionIndexer) 
                    {
                        Object[] sections=((ISectionIndexer)piece).GetSections();
                        if (sections != null) {
                            section += sections.Length;
                        }
                    }
                }
                position-=size;
            }
            
            return 0;
        }

        /// <Docs>This provides the list view with an array of section objects.</Docs>
        /// <remarks>This provides the list view with an array of section objects. In the simplest
        ///  case these are Strings, each containing one letter of the alphabet.
        ///  They could be more complex objects that indicate the grouping for the adapter's
        ///  consumption. The list view will call toString() on the objects to get the
        ///  preview letter to display while scrolling.</remarks>
        /// <format type="text/html">[Android Documentation]</format>
        /// <since version="Added in API level 3"></since>
        /// <summary>
        /// Gets the sections.
        /// </summary>
        /// <returns>The sections.</returns>
        public Java.Lang.Object[] GetSections ()
        {
            JavaList<Java.Lang.Object> sections = new JavaList<Java.Lang.Object>();

            foreach (IListAdapter piece in GetPieces()) {
                if (piece is ISectionIndexer) {
                    Java.Lang.Object[] curSections = ((ISectionIndexer) piece).GetSections();
                    if (curSections != null) {
                        foreach (Java.Lang.Object section in curSections) {
                            sections.Add(section);
                        }
                    }
                }
            }

            if (sections.Count == 0) {
                return new Java.Lang.Object[0];
            }
            
            return sections.ToArray<Java.Lang.Object>();
        }

        public void SetActive(IListAdapter adapter, bool isActive) {
            pieces.SetActive(adapter, isActive);
            this.NotifyDataSetChanged();
        }

        public void SetActive(View v, bool isActive) {
            pieces.SetActive(v, isActive);
            this.NotifyDataSetChanged();
        }

        protected JavaList<IListAdapter> GetPieces() {
            return pieces.GetPieces();
        }

        /// <summary>
        /// Dispose the MergeAdapter, clean up
        /// </summary>
        protected override void Dispose (bool disposing)
        {
            this.pieces = null;
            base.Dispose (disposing);
        }

        // In C# we need to add actions for the changed/invalidated events
        // We now handle them as delegates in the Merge class (above).
        protected class CascadeDataSetObserver : DataSetObserver 
        {
            public Action OnDataSetChanged;
            public Action OnDataSetInvalidated;

            public override void OnChanged() 
            {
                if (OnDataSetChanged != null)
                    OnDataSetChanged();
            }
            
            public override void OnInvalidated() 
            {
                if (OnDataSetInvalidated != null)
                    OnDataSetInvalidated();
            }
        }
    }
}

