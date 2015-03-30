using System;

using A = Android;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace Xamarin.Android.MergeAdapter.Sample
{
    [Activity (Label = "MergeAdapter Sample", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        /// <summary>
        /// Creates the activity event.
        /// </summary>
        /// <param name="bundle">Bundle object.</param>
        protected override void OnCreate (Bundle bundle)
        {
            base.OnCreate (bundle);
            SetContentView (Resource.Layout.Main);

            // Create the example merge adapter
            this.mergeAdapter = new MergeAdapter ();

            // Header view
            View header = CurrentInflator.Inflate(Resource.Layout.Header, null);
            var title  = header.FindViewById<TextView> (Resource.Id.header_text);
            title.Text = "Breaking Bad (Main Characters)";
            this.mergeAdapter.AddView (header);
            header.Dispose ();

            // Sample list items
            IListAdapter adapter = new ArrayAdapter<String>(this, A.Resource.Layout.SimpleListItem1, this.Names);
            this.mergeAdapter.AddAdapter (adapter);

            // Enable both 
            this.mergeAdapter.SetActive (adapter, true);
            this.mergeAdapter.SetActive (header, true);

            // No async download in this demo so hide loader
            var loading = FindViewById<ProgressBar> (Resource.Id.sample_loadingBar);
            loading.Visibility = ViewStates.Gone;
            loading.Dispose();

            // Bind the Merge Adapter to the listview in the layout
            ListView list = FindViewById<ListView> (Resource.Id.sample_listView);
            list.Visibility = ViewStates.Visible;
            list.Adapter = this.mergeAdapter;
            list.ItemClick += (object sender, AdapterView.ItemClickEventArgs e) => 
            {
                var t = this.Names[e.Position-1]; // header view in pos 0
                A.Widget.Toast.MakeText(this, t, A.Widget.ToastLength.Short).Show();
            };
            list.Dispose ();

            // The merge adapter allows combinations of views & lists to be added to a list view (in a  
            // smart way). This is particularly useful when creating list based UI with conditional items
            // e.g. dynamic display of generic content from REST API
        }
 
        /// <summary>
        /// Helper method: Gets the current layout inflator.
        /// </summary>
        /// <value>The Android layout inflator.</value>
        protected LayoutInflater CurrentInflator 
        {
            get { 
                return (LayoutInflater)this.ApplicationContext.GetSystemService(Context.LayoutInflaterService);
            }
        }

        /// <summary>
        /// Sample data for list display
        /// </summary>
        protected string[] Names
        {
            get {
                if (namesArray == null)
                    namesArray = new string[] {
                    "Walter White",
                    "Jesse Pinkman",
                    "Skyler White",
                    "Hank Schrader",
                    "Marie Schrader",
                    "Walter White, Jr.",
                    "Saul Goodman",
                    "Mike Ehrmantraut",
                    "Gustavo Fring",
                    "Todd Alquist",
                    "Lydia Rodarte-Quayle"
                };

                return namesArray;
            }
        }

        protected MergeAdapter mergeAdapter = null;
        protected string[] namesArray = null;
    }
}


