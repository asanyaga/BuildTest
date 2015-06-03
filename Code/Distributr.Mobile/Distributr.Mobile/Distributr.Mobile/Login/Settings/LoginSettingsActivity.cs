using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Java.Interop;
using Mobile.Common.Core;

namespace Distributr.Mobile.Login.Settings
{
    [Activity(Label = "Login Settings")]
    public class LoginSettingsActivity : BaseActivity<User>
    {
        private LoginSettingsRepository repository;
        private LoginSettings settings;

        public override void Created(Bundle bundle)
        {
            OverridePendingTransition(Resource.Animation.slide_in, Resource.Animation.slide_out);

            repository = Resolve<LoginSettingsRepository>();
            settings = repository.GetSettings();

            SetupServerUrl();
        }

        private void SetupServerUrl()
        {
            FindViewById<EditText>(Resource.Id.server_url).Text = settings.ServerUrl;
        }

        [Export("Save")]
        public void Save(View button)
        {
            var serverUrl = FindViewById<EditText>(Resource.Id.server_url).Text;
            settings.ServerUrl = serverUrl;
            repository.Save(settings);
            OnBackPressed();
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
            OverridePendingTransition(Resource.Animation.slide_in_back, Resource.Animation.slide_out_back);
        }
    }
}