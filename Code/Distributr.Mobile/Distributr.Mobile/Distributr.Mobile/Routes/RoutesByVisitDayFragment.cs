using System;
using System.Collections.Generic;
using System.Linq;
using Android.OS;
using Android.Views;
using Android.Widget;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Mobile.Data;

namespace Distributr.Mobile.Routes
{
    public class RoutesByVisitDayFragment : BaseRoutesFragment
    {

        public override void CreateChildViews(View parent, Bundle bundle)
        {
            base.CreateChildViews(parent, bundle);
            SetupOutletListAdapter();
            SetupRoutesDropdownByDay();
        }

        private void SetupRoutesDropdownByDay()
        {
            var days = GetDaysOfWeekRelativeToToday();
            var adapter = new ArrayAdapter<DayOfWeek>(Activity, Resource.Layout.route_type_dropdown_item, days);
            RouteTypeDropdown.Adapter = adapter;
            RouteTypeDropdown.ItemSelected += delegate
            {
                var day = adapter.GetItem(RouteTypeDropdown.SelectedItemPosition);
                UpdateRouteListForDay(day);
            };
            UpdateRouteListForDay(days.First());
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

        private void UpdateRouteListForDay(DayOfWeek day)
        {
            var outlets = Resolve<Database>().GetAll<OutletVisitDay>().Where(v => v.Day == day).Select(o => o.OutletRef);
            UpdateOutletList(outlets.ToArray());
        }

        private List<DayOfWeek> GetDaysOfWeekRelativeToToday()
        {
            var days = Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>();
            var today = DateTime.Now.DayOfWeek;
            return days.OrderBy(d => d < today ? 1 : 0).ToList();
        }
    }
}