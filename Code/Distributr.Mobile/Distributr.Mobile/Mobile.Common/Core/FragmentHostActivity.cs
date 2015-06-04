using System;
using Android.OS;
using Android.Support.V4.Widget;
using Android.App;
using ActionBarDrawerToggle = Android.Support.V7.App.ActionBarDrawerToggle;
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

        protected void SetupNavigationDrawer()
        {
            drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            drawerToggle = new ActionBarDrawerToggle(this, drawer, toolbar, Resource.String.open, Resource.String.close);
            drawer.SetDrawerListener(drawerToggle);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);
        }

        protected void SetNavigationDrawerView(NestedFragment<U> fragment)
        {
            //Enable navigation drawer
            SetupNavigationDrawer();
        }

        public override void Show(Type fragmentType)
        {
            if (currentFragement != null && currentFragement.GetType() == fragmentType) return;

            var fragment = (BaseFragment<U>)Activator.CreateInstance(fragmentType);
                       
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

        public override void ShowRootFragment(Type fragment)
        {
            SupportFragmentManager.PopBackStackImmediate(null,
                Android.Support.V4.App.FragmentManager.PopBackStackInclusive);
            Show(fragment);
        }

        public override void GoBackTo(Type fragment)
        {
            var name = fragment.Name;
            SupportFragmentManager.PopBackStackImmediate(name, 
                Android.Support.V4.App.FragmentManager.PopBackStackInclusive);
            Show(fragment);
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
