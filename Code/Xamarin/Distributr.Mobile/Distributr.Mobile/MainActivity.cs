using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Distributr.Mobile.Login;
using Distributr.Mobile.Routes;
using Mobile.Common.Core;
using Android.Content.PM;

namespace Distributr.Mobile
{
    [Activity(Label = "MainActivity", WindowSoftInputMode = SoftInput.AdjustPan,
        Theme = "@style/AppTheme",
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
    public class MainActivity : FragmentHostActivity<User>
    {

        public override void Created(Bundle bundle)
        {
            base.Created(bundle);
            Show(typeof (RoutesFragment));
            SetupNavigationView();
        }

        private void SetupNavigationView()
        {
            TextView text = new TextView(this) {Text = "Hello, World!"};
            SetNavigationView(text);
        }
    }
}