using Android.App;
using Android.OS;
using Android.Views;
using Distributr.Mobile.Login;
using Distributr.Mobile.Routes;
using Mobile.Common.Core;
using Android.Content.PM;
using Distributr.Mobile.SidePanel;

namespace Distributr.Mobile
{
    [Activity(Label = "MainActivity", WindowSoftInputMode = SoftInput.AdjustPan,
        Theme = "@style/AppTheme",
        LaunchMode = LaunchMode.SingleInstance,
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
    public class MainActivity : FragmentHostActivity<User>
    {
        public override void Created(Bundle bundle)
        {
            base.Created(bundle);
            SetupNavigationFragment();
            Show(typeof (RoutesFragment));
        }

        private void SetupNavigationFragment()
        {
            SetupNavigationDrawer();
            var fragment = new SidePanelFragment();
            SupportFragmentManager
                .BeginTransaction()
                .Replace(Resource.Id.side_panel_fragment, fragment)
                .Commit();
        }
    }
}