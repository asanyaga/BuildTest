using Android.Content;
using Android.Net;
using Android.OS;
using Android.Views;
using Android.Widget;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Mobile.Core.Outlets;
using Distributr.Mobile.Login;
using Mobile.Common;

namespace Distributr.Mobile.Outlets
{
    public class OutletFragment : BaseFragment<User>, AdapterView.IOnItemClickListener
    {
        private Outlet outlet;
        private OutletActionListAdapter outletActionListAdapter;

        public override void CreateChildViews(View parent, Bundle bundle)
        {
            outlet = App.Get<Outlet>();
            Resolve<OutletRepository>().LoadContactsForOutlet(outlet);
            SetTitle(outlet.Name);
            SetupOutletActionList(parent);
        }

        public void OnItemClick(AdapterView parent, View view, int position, long id)
        {
            var action = outletActionListAdapter.GetItem(position);
            App.Put(outlet);
            Activity.Show(action.Fragment);
        }

        private void SetupOutletActionList(View parent)
        {
            var actionList = parent.FindViewById<ListView>(Resource.Id.outlet_action_list);
            actionList.Adapter = outletActionListAdapter = new OutletActionListAdapter(Activity);
            actionList.OnItemClickListener = this;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.map: LaunchMapActivity();
                    return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        private void LaunchMapActivity()
        {
            var uri = string.Format("http://maps.google.com/maps?daddr={0},{1} ({2})", outlet.Latitude, outlet.Longitude, outlet.Name);
            var intent = new Intent(Intent.ActionView, Uri.Parse(uri));
            intent.SetClassName("com.google.android.apps.maps", "com.google.android.maps.MapsActivity");
            try
            {
                Activity.StartActivity(intent);
            }
            catch (ActivityNotFoundException e)
            {
                try
                {
                    var unrestrictedIntent = new Intent(Intent.ActionView, Uri.Parse(uri));
                    Activity.StartActivity(unrestrictedIntent);
                }
                catch (ActivityNotFoundException x)
                {
                    Toast.MakeText(Activity, "You need a map application installed to use this feature",
                        ToastLength.Long);
                }                
            }
            
        }
    }
}