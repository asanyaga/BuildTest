using System;
using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using DK.Ostebaronen.FloatingActionButton;
using Android.Graphics;
using Android.Widget;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace Mobile.Common.Core
{
    public abstract class BaseActivity<U> : ActionBarActivity
    {
        private BasicTabHost tabHost;

        public Fab Fab { get; private set; }
        public Toolbar Toolbar { get; private set; }
        public BaseApplication<U> App { get; private set; }

        protected T Resolve<T>() where T : class
        {
            return App.Resolve<T>();
        }

        protected void Publish(object message)
        {
            App.Publish(message);
        }

        public virtual void Show(Type fragment)
        {
            
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            App = (BaseApplication<U>) Application;

            SetupLayout();
            SetupToolBar();
            SetupTabHost();
            SetupFab();
            Created(bundle);
        }

        public void ResetState()
        {
            SupportActionBar.NavigationMode = (int)ActionBarNavigationMode.Standard;
            SupportActionBar.Title = string.Empty;
                        
            Fab.Visibility = ViewStates.Invisible;
        }

        private void SetupFab()
        {
            Fab = FindViewById<Fab>(Resource.Id.fabbutton);
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

        private void SetupLayout()
        {
            var name = GetType().Name.ToLower();
            var layoutId = App.ResolveLayout(name);
            if (layoutId != -1)
            {
                SetContentView(layoutId);
            }
        }

        private void SetupTabHost()
        {
            tabHost = FindViewById<BasicTabHost>(Resource.Id.tab_host);
        }

        public void AddTab(TabModel model)
        {
            tabHost.AddTab(model);
        }

        public void RemoveTabs()
        {
            tabHost.Reset();
        }

        public abstract void Created(Bundle bundle);

        public override bool OnPrepareOptionsMenu(IMenu menu)
        {
            var name = GetType().Name.ToLower();
            var menuId = App.ResolveMenu(name);
            if (menuId != -1)
            {
                MenuInflater.Inflate(menuId, menu);
                return true;
            }

            return false;
        }

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

        protected void ToggleVisibility(int viewId)
        {
            var view = FindViewById<View>(viewId);
            if (view.Visibility == ViewStates.Visible)
            {
                view.Visibility = ViewStates.Invisible;
            }
            else
            {
                view.Visibility = ViewStates.Visible;
            }
        }

        public void ChangePrimaryColor(int color)
        {
            var newColor = Resources.GetColor(color);
            Toolbar.SetBackgroundColor(newColor);
            tabHost.SetBackgroundResource(color);

            var api = (int) Build.VERSION.SdkInt;
            if (api >= 21)
            {
                //Only valid on Lollipop and higher
                Window.SetStatusBarColor(Darken(newColor));
            }
        }

        private static Color Darken(Color original)
        {
            var  hsv = new float[3];
            Color.ColorToHSV(original, hsv);
            hsv[2] *= 0.8f; 
            return Color.HSVToColor(hsv);
        }
    }
}