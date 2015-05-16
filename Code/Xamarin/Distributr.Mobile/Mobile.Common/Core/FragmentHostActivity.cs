using System;

using Android.OS;
using Android.Views;
using Android.Support.V4.Widget;
using Android.App;
using ActionBarDrawerToggle = Android.Support.V7.App.ActionBarDrawerToggle;
using Android.Widget;
using FragmentManager = Android.Support.V4.App.FragmentManager;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace Mobile.Common.Core
{

    public class FragmentHostActivity<U> : BaseActivity<U>, FragmentManager.IOnBackStackChangedListener
    {
        private BaseFragment<U> currentFragement;
        private DrawerLayout drawer;
        private ActionBarDrawerToggle drawerToggle;

        public override void Created(Bundle bundle)
        {
            
        }

        private void SetupNavigationDrawer()
        {
            drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            drawerToggle = new ActionBarDrawerToggle(this, drawer, toolbar, Resource.String.open, Resource.String.close);
            drawer.SetDrawerListener(drawerToggle);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);
        }

        protected void SetNavigationView(View naivgationView)
        {
            //Enable navigation drawer
            SetupNavigationDrawer();
            ListView navigationList = FindViewById<ListView>(Resource.Id.left_drawer);
            navigationList.AddHeaderView(naivgationView);
        }

        public override void Show(Type fragmentType)
        {
            var fragment = (BaseFragment<U>)fragmentType
                .GetConstructor(new Type[] { })
                .Invoke(new object[] { });

            SupportFragmentManager
                .BeginTransaction()
                .Replace(Resource.Id.fragment_container, fragment)
                .AddToBackStack(fragment.GetTag())
                .Commit();

            currentFragement = fragment;
        }

        protected override void OnPostCreate(Bundle savedInstanceState)
        {
            base.OnPostCreate(savedInstanceState);
            if (drawerToggle != null)
            {
                drawerToggle.SyncState();
            }
            SupportFragmentManager.AddOnBackStackChangedListener(this);
        }

        public override void OnConfigurationChanged(Android.Content.Res.Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig);
            if (drawerToggle != null)
            {
                drawerToggle.OnConfigurationChanged(newConfig);
            }
        }

        public void OnBackStackChanged()
        {
            currentFragement = SupportFragmentManager
                .FindFragmentById(Resource.Id.fragment_container) as BaseFragment<U>;
        }

        public override void OnBackPressed()
        {
            if (currentFragement.OnBackPressed())
            {
                //Has been handled by fragment
                return;
            }

            if (SupportFragmentManager.BackStackEntryCount == 1)
            {
                ShowLogoutPrompt();
            }
            else
            {
                SupportFragmentManager.PopBackStackImmediate();
            }
        }

        private void ShowLogoutPrompt()
        {
            new AlertDialog.Builder(this)
                .SetPositiveButton("Yes", (sender, args) =>
                {
                    App.Logout();
                    Finish();
                })
                .SetNegativeButton("No", (sender, args) => { })
                .SetMessage("Do you want to logout?")
                .SetTitle("Logout")
                .Show();
        }
    }
}
