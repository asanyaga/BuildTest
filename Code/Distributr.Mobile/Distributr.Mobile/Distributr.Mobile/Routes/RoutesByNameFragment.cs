using System.Collections.Generic;
using System.Linq;
using Android.OS;
using Android.Views;
using Android.Widget;
using Distributr.Core.Domain.Master.CostCentreEntities;

namespace Distributr.Mobile.Routes
{
    public class RoutesByNameFragment : BaseRoutesFragment
    {
        private Dictionary<string, Route> routes;

        public override void CreateChildViews(View parent, Bundle bundle)
        {
            base.CreateChildViews(parent, bundle);
            routes = Resolve<IRoutesRepository>().GetAllRoutes();
            SetupOutletListAdapter();
            SetupRoutesDropdownByName();
        }

        private void SetupRoutesDropdownByName()
        {
            var adapter = new ArrayAdapter<string>(Activity, Resource.Layout.route_type_dropdown_item, routes.Keys.ToArray());
            RouteTypeDropdown.Adapter = adapter;
            RouteTypeDropdown.ItemSelected += delegate
            {
                var item = adapter.GetItem(RouteTypeDropdown.SelectedItemPosition);
                UpdateOutletList(routes[item].Outlets);
            };

            if (routes.Count > 0)
            {
                UpdateOutletList(routes.First().Value.Outlets);
            }
        }

        private void SetupOutletListAdapter()
        {
            OutletList.Adapter = RoutetListAdapter = new RoutetListAdapter(Activity);
            OutletList.ItemClick += delegate(object sender, AdapterView.ItemClickEventArgs args)
            {
                var outlet = RoutetListAdapter.GetItem(args.Position);
                ShowOutlet(outlet);
            };
        }
    }
}