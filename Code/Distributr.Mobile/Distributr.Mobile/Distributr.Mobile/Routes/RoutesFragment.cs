using Android.OS;
using Android.Views;
using Distributr.Mobile.Login;
using Mobile.Common.Core;

namespace Distributr.Mobile.Routes
{
    public class RoutesFragment : TabbedFragment<User>
    {
        private RoutesByNameFragment routesByNameFragment;
        private RoutesByVisitDayFragment routesByVisitDayFragment;

        public override void CreateChildViews(View parent, Bundle bundle)
        {
            routesByNameFragment = new RoutesByNameFragment();
            routesByVisitDayFragment = new RoutesByVisitDayFragment();
            SetupTabs();
            ShowFragment(routesByNameFragment);
        }

        private void ShowFragment(BaseRoutesFragment fragment)
        {
            ShowNestedFragment(Resource.Id.routes_fragment_container, fragment);
        }

        private void SetupTabs()
        {
            AddTab(
            new TabModel(Resources.GetString(Resource.String.routes_screen_by_name_title))
            {
                OnTabSelected = () => { ShowFragment(routesByNameFragment); }
            });

            AddTab(
            new TabModel(Resources.GetString(Resource.String.routes_screen_by_visit_day_title))
            {
                OnTabSelected = () => { ShowFragment(routesByVisitDayFragment); }
            });
        }
    }
}