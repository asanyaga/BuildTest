using System;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Views;
using DK.Ostebaronen.FloatingActionButton;
using Android.Widget;
using Mobile.Common.Core.Views;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace Mobile.Common.Core
{
    public abstract class BaseActivity<U> : ActionBarActivity
    {
        private BasicTabHost TabHost { get; set; }
        public EditText SearchWidget { get; private set; }
        public ScreenHeader Header { get; private set; }
        public Fab Fab { get; private set; }
        public Toolbar Toolbar { get; private set; }
        public BaseApplication<U> App { get; private set; }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            App = (BaseApplication<U>)Application;

            SetupLayout();
            SetupToolBar();
            SetupTabHost();
            SetupHeader();
            SetupSearch();
            SetupFab();
            // After settings up the UI, call into the Created method of the subclass
            Created(bundle);
        }

        public abstract void Created(Bundle bundle);

        protected override sealed void OnPause()
        {
            base.OnPause();
            App.Unregister(this);
            Paused();
        }

        protected virtual void Paused()
        {
        }

        protected override sealed void OnStop()
        {
            base.OnStop();
            Stopped();
        }

        protected virtual void Stopped()
        {
        }

        protected override sealed void OnResume()
        {
            base.OnResume();
            App.Register(this);
            Resumed();
        }

        protected virtual void Resumed()
        {
        }

        protected override sealed void OnStart()
        {
            base.OnStart();
            Started();
        }

        protected virtual void Started()
        {
        }

        public override bool OnPrepareOptionsMenu(IMenu menu)
        {
            var menuId = App.ResolveMenu(GetType());
            if (menuId != -1)
            {
                MenuInflater.Inflate(menuId, menu);
                return true;
            }
            return false; 
        }

        private void SetupLayout()
        {
            var layoutId = App.ResolveLayout(GetType());
            if (layoutId != -1)
            {
                SetContentView(layoutId);
            }
        }

        private void SetupHeader()
        {
            Header = FindViewById<ScreenHeader>(Resource.Id.screen_header);
        }

        private void SetupTabHost()
        {
            TabHost = FindViewById<BasicTabHost>(Resource.Id.tab_host);
        }

        public void AddTab(TabModel model)
        {
            TabHost.AddTab(model);
        }

        public void RemoveTabs()
        {
            TabHost.Reset();
        }

        // Android's Floating Action Button. This is is invisible by default but can be enabled 
        // by Fragments that require it. 
        private void SetupFab()
        {
            Fab = FindViewById<Fab>(Resource.Id.fabbutton);
            if (Fab != null)
            {
                Fab.FabDrawable = Resources.GetDrawable(Resource.Drawable.ic_action_left_arrow);
                Fab.FabColor = Resources.GetColor(Resource.Color.color_accent);
            }
        }

        // Called by Fragments so that they can clean-up when they are no longer visible. 
        public void ResetState(bool resetState)
        {
            if (resetState) { 
                SupportActionBar.Title = string.Empty;
            }
            FindViewById<ViewGroup>(Resource.Id.screen_header_content).RemoveAllViews();
            FindViewById<ViewGroup>(Resource.Id.popup_view).RemoveAllViews();
            SearchWidget.Visibility = ViewStates.Gone;
            Fab.Visibility = ViewStates.Invisible;
        }

        private void SetupToolBar()
        {
            Toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            if (Toolbar != null)
            {
                SetSupportActionBar(Toolbar);
                SupportActionBar.Title = string.Empty;
            }
        }

        private void SetupSearch()
        {
            SearchWidget = FindViewById<EditText>(Resource.Id.search_widget);
        }

        // Resolve a dependency from the container
        protected T Resolve<T>() where T : class
        {
            return App.Resolve<T>();
        }

        // Publish a message on the Event Bus
        protected void Publish(object message)
        {
            App.Publish(message);
        }

        public virtual void Show(Type fragment)
        {
            // Show the Fragment. This is overriden by FragmentHostActivity which displays 
            // fragments on request
        }

        //Clear the backstack and show the specified fragment as the new root
        public virtual void ShowRootFragment(Type fragment)
        {

        }

        public virtual void GoBackTo(Type fragment)
        {
            
        }

        public void CloseNavigationDrawer()
        {
            var drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            if (drawer != null)
            {
                drawer.CloseDrawers();
            }
        }
    }
}