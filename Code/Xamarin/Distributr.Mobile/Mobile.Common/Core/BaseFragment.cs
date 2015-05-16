using System;
using Android.App;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Views;
using Android.Widget;
using Mobile.Common.Core;
using Mobile.Common.Core.Views;
using Fragment = Android.Support.V4.App.Fragment;

namespace Mobile.Common
{
    public abstract class BaseFragment<U> : Fragment
    {
        public BaseApplication<U> App { get; private set; }
        public BaseActivity<U> Activity { get; private set; }

        private readonly bool resetTabs;

        protected BaseFragment(bool resetTabs = true)
        {
            this.resetTabs = resetTabs;
        }

        public string GetTag()
        {
            return GetType().Name;
        }

        protected T Resolve<T>() where T : class
        {
            return App.Resolve<T>();
        }

        protected void Publish(object message)
        {
            App.Publish(message);
        }

        public override sealed void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            HasOptionsMenu = true;
            Created(bundle);
        }

        protected void ShowFab(Color color, Drawable drawable)
        {
            Activity.Fab.FabColor = color; 
            Activity.Fab.FabDrawable = drawable;
            Activity.Fab.Visibility = ViewStates.Visible;            
        }

        protected void ShowFragment(int viewId, Fragment fragment)
        {
            ChildFragmentManager
                .BeginTransaction()
                .Replace(viewId, fragment)
                .Commit();
        }

        public void EnableAutoHideToolbarOnScroll(ListView list)
        {
            var listener = new HideHeaderOnScrollListener {ActionBar = Activity.SupportActionBar};
            list.SetOnScrollListener(listener);
        }

        public virtual void Created(Bundle bundle)
        {
            
        }

        public override void OnAttach(Activity activity)
        {
            base.OnAttach(activity);
            Activity = (BaseActivity<U>) activity;
            App = Activity.App;
        }

        public void ChangePrimaryColor(int color)
        {
            Activity.ChangePrimaryColor(color);
        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater menuInflater)
        {
            var name = GetType().Name.ToLower();
            var menuId = App.ResolveMenu(name);

            if (menuId != -1)
            {
                menuInflater.Inflate(menuId, menu);
            }
            base.OnCreateOptionsMenu(menu, menuInflater);
        }

        public override sealed View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle bundle)
        {
            Reset();
            var layout = SetupLayout(inflater);
            CreateChildViews(layout, bundle);
            return layout;
        }

        private void Reset()
        {
            if (resetTabs)
            {
                Activity.RemoveTabs();
            }
            Activity.ResetState();
        }

        public abstract void CreateChildViews(View parent, Bundle bundle);

        private View SetupLayout(LayoutInflater inflater)
        {
            var name = GetType().Name.ToLower();
            var layoutId = App.ResolveLayout(name);
            return  inflater.Inflate(layoutId, null);
        }

        public void SetTitle(int stringResourceId)
        {
            SetTitle(Resources.GetString(stringResourceId));
        }

        public void SetTitle(string title)
        {
            Activity.SupportActionBar.Title = title;
        }

        public View Inflate(int viewId)
        {
            return Activity.LayoutInflater.Inflate(viewId, null);
        }

        public override sealed void OnPause()
        {
            base.OnPause();
            App.Unregister(this);
            Paused();
        }

        protected virtual void Paused()
        {
        }

        public override sealed void OnStop()
        {
            base.OnStop();
            Stopped();
        }

        protected virtual void Stopped()
        {
        }

        public override sealed void OnResume()
        {
            base.OnResume();
            App.Register(this);
            Resumed();
        }

        protected virtual void Resumed()
        {
        }

        public override sealed void OnStart()
        {
            base.OnStart();
            Started();
        }

        protected virtual void Started()
        {
        }

        public virtual bool OnBackPressed()
        {
            return false;
        }

    }
}