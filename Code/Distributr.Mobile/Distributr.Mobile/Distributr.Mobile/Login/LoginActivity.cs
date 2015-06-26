using System;
using Android.App;
using Android.Content.PM;
using Android.Content.Res;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Views;
using Android.Widget;
using Distributr.Mobile.Core.Data;
using Distributr.Mobile.Core.Sync;
using Distributr.Mobile.Core.Util;
using Distributr.Mobile.Login.Settings;
using Distributr.Mobile.Sync;
using Java.Interop;
using Mobile.Common.Core;
using Orientation = Android.Content.Res.Orientation;
using ConfigChanges = Android.Content.PM.ConfigChanges;

namespace Distributr.Mobile.Login
{
    [Activity(Label = "Distributr", MainLauncher = true, Icon = "@drawable/icon",
        Theme = "@style/Theme.AppCompat.Light.NoActionBar.FullScreen", 
        LaunchMode = LaunchMode.SingleInstance,
        ConfigurationChanges=ConfigChanges.Orientation | ConfigChanges.ScreenSize)
    ]
    public class LoginActivity : BaseActivity<User>
    {
        private const int PROGRESS_LIMIT = 10000;
        private Button loginButton;
        private string loginButtonText;
        private LoginWorkflow loginWorkFlow;
        private ClipDrawable progress;

        public override void Created(Bundle bundle)
        {
            loginWorkFlow = Resolve<LoginWorkflow>();
            SetupLoginButton();
        }

        private void SetupLoginButton()
        {
            loginButton = FindViewById<Button>(Resource.Id.login);
            loginButtonText = loginButton.Text;

            var layer = (LayerDrawable) loginButton.Background;
            progress = (ClipDrawable) layer.GetDrawable(1);
        }

        [Export("Settings")]
        public void Settings(View button)
        {
            StartActivity(typeof (LoginSettingsActivity));
        }

        [Export("Login")]
        public async void Login(View button)
        {
            loginButton.Enabled = false;

            var username = FindViewById<EditText>(Resource.Id.username).Text;
            var password = FindViewById<EditText>(Resource.Id.password).Text;

            loginButton.Text = "Logging In..";

            var result = await loginWorkFlow.Login(username, password);

            loginButton.Enabled = true;

            ProcessLoginResult(result);
        }

        private void ProcessLoginResult(Result<User> result)
        {
            if (result.WasSuccessful())
            {
                var user = result.Value;

                if (user.IsNewUser)
                {
                    // No local user data so we need to hold the user at the login screen until
                    // we download it - the app is pretty useless without data so no 
                    // point in progressing just yet. 
                    App.InitialiseFor(user);
                }
                else
                {
                    // We've already got a user in the local DB which would also include
                    // master data so we can open the app
                    App.Unregister(this);
                    App.InitialiseFor(user);
                    StartActivity(typeof (MainActivity));
                    ResetLoginButton();
                }
            }
            else
            {
                ShowErrorDialog(result.Message, result.Exception);
            }
        }

        public override void OnConfigurationChanged(Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig);
            ShowOrHideMapImage(newConfig);
        }

        // Hide the Map illustration when in Landscape mode, reshow
        // if switching back to portrait. 
        private void ShowOrHideMapImage(Configuration newConfig)
        {
            var orientation = newConfig.Orientation;
            var view = FindViewById<ImageView>(Resource.Id.map);
            switch (orientation)
            {
                case Orientation.Landscape:
                    view.Visibility = ViewStates.Gone;
                    break;
                case Orientation.Portrait:
                    view.Visibility = ViewStates.Visible;
                    break;
            }
        }

        protected override void Resumed()
        {
            if (App.Initialised())
            {
                StartActivity(typeof (MainActivity));
            }
        }

        public void OnEvent(SyncUpdateEvent<MasterDataUpdate> update)
        { 
            RunOnUiThread(() =>
            {
                var level = (update.PercentDone * PROGRESS_LIMIT);
                progress.SetLevel((Convert.ToInt32(level)));
                loginButton.Text = update.Message;
            });
        }

        public void OnEvent(SyncPausedEvent<MasterDataUpdate> paused)
        {
            RunOnUiThread(() =>
            {
                loginButton.Text = paused.Message;
            });
        }

        public void OnEvent(SyncFailedEvent<MasterDataUpdate> failure)
        {
            RunOnUiThread(() =>
            {
                ResetLoginButton();
                ShowErrorDialog(failure.Message, failure.Exception);
            });
        }

        public void OnEvent(SyncCompletedEvent<MasterDataUpdate> completed)
        {
            RunOnUiThread(() =>
            {
                progress.SetLevel(PROGRESS_LIMIT);
                loginButton.Text = "Sync Complete";
                StartActivity(typeof (MainActivity));
                ResetLoginButton();
            });
        }

        private void ShowErrorDialog(string message, Exception exception)
        {
            ResetLoginButton();
            Dialog alert = new ErrorAlertBuilder(this).Build(message, exception, 
                Resource.Style.AppTheme_AlertDialog);
            alert.Show();
        }

        private void ResetLoginButton()
        {
            loginButton.Text = loginButtonText;
            progress.SetLevel(0);
        }
    }
}