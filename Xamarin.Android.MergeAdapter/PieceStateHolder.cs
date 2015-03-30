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
    /// <summary>
    /// Wrapper for Pieces / State
    /// </summary>
    public class PieceStateHolder 
    {
        JavaList<PieceState> pieces = new JavaList<PieceState>();
        JavaList<IListAdapter> active = null;

        public void Add(IListAdapter adapter) {
            pieces.Add(new PieceState(adapter, false));
        }

        public void SetActive (IListAdapter adapter, bool isActive)
        {
            foreach (PieceState state in pieces) 
            {
                if (state.Adapter == adapter)
                {
                    state.IsActive = isActive;
                    active = null;
                    break;
                }
            }
        }

        public void SetActive(View v, bool isActive) 
        {
            foreach (PieceState state in pieces) 
            {
                if (state.Adapter is SackOfViewsAdapter &&
                    ((SackOfViewsAdapter) state.Adapter).HasView(v)) 
                {
                    state.IsActive = isActive;
                    active = null;
                    break;
                }
            }
        }

        public JavaList<PieceState> GetRawPieces() 
        {
            return this.pieces;
        }

        public JavaList<IListAdapter> GetPieces ()
        {
            if (this.active == null) {
                this.active = new JavaList<IListAdapter>();
                
                foreach (PieceState state in pieces) {
                    if (state.IsActive) {
                        active.Add(state.Adapter);
                    }
                }
            }
            
            return(active);
        }
    }
}

