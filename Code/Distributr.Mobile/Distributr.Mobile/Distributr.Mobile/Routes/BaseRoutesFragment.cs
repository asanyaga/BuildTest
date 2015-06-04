using System.Collections.Generic;
using System.Linq;
using Android.OS;
using Android.Views;
using Android.Widget;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Mobile.Core.Outlets;
using Distributr.Mobile.Login;
using Distributr.Mobile.Outlets;
using Mobile.Common.Core;

namespace Distributr.Mobile.Routes
{
    public class BaseRoutesFragment : NestedFragment<User>
    {
        protected RoutetListAdapter RoutetListAdapter;
        protected ListView OutletList;
        protected Spinner RouteTypeDropdown;
        private IOutletRepository outletRepository;

        public override void CreateChildViews(View parent, Bundle bundle)
        {
            outletRepository = Resolve<IOutletRepository>();
            SetTitle(Resource.String.routes_screen_title);
            SetupViews(parent);
        }

        private void SetupViews(View parent)
        {
            OutletList = parent.FindViewById<ListView>(Resource.Id.outlet_list);
            RouteTypeDropdown = parent.FindViewById<Spinner>(Resource.Id.routes_dropdown);
        }

        protected void UpdateOutletList(Outlet[] outlets)
        {
            foreach (var outlet in outlets)
            {
                outletRepository.LoadContactsForOutlet(outlet);
            }
            var orderedOutlets = OrderByPriority(outlets);
            RoutetListAdapter.Clear();
            RoutetListAdapter.AddAll(orderedOutlets);
        }

        private List<Outlet> OrderByPriority(Outlet[] outlets)
        {
            if (outlets.Length == 0) return new List<Outlet>();

            var priorities = outletRepository.FindPrioritiesForRoute(outlets[0].RouteMasterId);
            
            return outlets.OrderBy(o => priorities.ContainsKey(o.Id) 
                ? priorities[o.Id].Priority 
                : int.MaxValue)
                .ToList();
        }

        protected void ShowOutlet(Outlet outlet)
        {
            App.Put(outlet);
            Activity.Show(typeof(OutletFragment));            
        }
    }
}