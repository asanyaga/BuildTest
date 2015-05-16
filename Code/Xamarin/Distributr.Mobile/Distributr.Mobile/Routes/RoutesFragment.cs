using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.CostCentre;
using Distributr.Mobile.Login;
using Distributr.Mobile.Outlets;
using Mobile.Common;
using ActionBar = Android.Support.V7.App.ActionBar;

namespace Distributr.Mobile.Routes
{
    public class RoutesFragment : BaseFragment<User>, ActionBar.IOnNavigationListener, AdapterView.IOnItemClickListener
    {
        private OutletListAdapter outletListAdapter;
        private Dictionary<string, Route> routes;

        public void OnItemClick(AdapterView parent, View view, int position, long id)
        {
            var outlet = outletListAdapter.GetItem(position);
            App.Put(outlet);
            Activity.Show(typeof (OutletFragment));
        }

        public bool OnNavigationItemSelected(int itemPosition, long itemId)
        {
            var key = routes.Keys.ElementAt(itemPosition);
            UpdateOutletList(routes[key]);
            return true;
        }

        public override void CreateChildViews(View parent, Bundle bundle)
        {
            SetupViews(parent);
        }

        private void SetupViews(View parent)
        {
            routes = Resolve<IRoutesRepository>().GetAllRoutes();

            SetupRouteNameDropDown(routes.Keys);
            SetupOutletList(parent);

            if (routes.Count > 0)
            {
                UpdateOutletList(routes.First().Value);
            }
        }

        private void SetupOutletList(View parent)
        {
            var outletList = parent.FindViewById<ListView>(Resource.Id.outlet_list);
            outletList.Adapter = outletListAdapter = new OutletListAdapter(Activity);
            outletList.OnItemClickListener = this;
        }

        private void UpdateOutletList(Route route)
        {
            outletListAdapter.Clear();
            outletListAdapter.AddAll(route.Outlets);
        }

        private void SetupRouteNameDropDown(ICollection routeNames)
        {
            var themedContext = Activity.SupportActionBar.ThemedContext;
            var adapter = new ArrayAdapter(themedContext, Android.Resource.Layout.SimpleSpinnerItem);
            adapter.AddAll(routeNames);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            Activity.SupportActionBar.NavigationMode = (int) ActionBarNavigationMode.List;
            Activity.SupportActionBar.SetListNavigationCallbacks(adapter, this);
        }
    }
}