using System;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Views.Animations;
using Android.Views.InputMethods;
using Android.Widget;
using Mobile.Common.Core;
using Mobile.Common.Core.Views;
using Fragment = Android.Support.V4.App.Fragment;
using Math = Java.Lang.Math;

namespace Mobile.Common
{
    public abstract class BaseFragment<U> : Fragment, View.IOnClickListener, TextView.IOnEditorActionListener
    {
        public BaseApplication<U> App { get; private set; }
        public new BaseActivity<U> Activity { get; private set; }

        protected IMenu Menu;

        public U User
        {
            get { return App.User; }
        }

        private bool ResetState { get; set; }

        protected BaseFragment(bool resetState = true)
        {
            ResetState = resetState;
        }
 
        public override void OnAttach(Activity activity)
        {
            base.OnAttach(activity);
            Activity = (BaseActivity<U>)activity;
            App = Activity.App;
        }

        public override sealed void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            HasOptionsMenu = true;
            Created(bundle);
        }

        public virtual void Created(Bundle bundle)
        {

        }

        public override sealed View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle bundle)
        {
            Reset();
            var layout = SetupLayout(inflater);
            CreateChildViews(layout, bundle);
            return layout;
        }

        // This is the main entry point for sub-classes to initialise themselves. 
        public abstract void CreateChildViews(View parent, Bundle bundle);

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
            AttachFabListener();
            AttachSearchListener();
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

        //Automatically set the XML layout for this fragment. 
        //
        // e.g if you have MyFragment then layout my_fragment.xml will be
        // set when the fragment is created.
        // 
        private View SetupLayout(LayoutInflater inflater)
        {
            var layoutId = App.ResolveLayout(GetType());
            if (layoutId == -1)
            {
                var message = string.Format("Layout not found for {0} or its parents", GetType());
                Console.WriteLine(message);
            }
            return inflater.Inflate(layoutId, null);
        }

        // Like above but for menu resoures instead of layouts. 
        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater menuInflater)
        {
            var menuId = App.ResolveMenu(GetType());

            if (menuId != -1)
            {
                menuInflater.Inflate(menuId, menu);
            }
            Menu = menu;
            base.OnCreateOptionsMenu(menu, menuInflater);
            OnMenuCreated();
        }

        protected virtual void OnMenuCreated()
        {
            
        }

        public void SetTitle(int stringResourceId)
        {
            SetTitle(Resources.GetString(stringResourceId));
        }

        public void SetTitle(string titleText)
        {
            Activity.Toolbar.FindViewById<TextView>(Resource.Id.toolbar_title).Text = titleText;
        }

        public void AddHeaderView(View contentToDisplayInHeader)
        {
            var headerView = Activity.FindViewById<ViewGroup>(Resource.Id.screen_header_content);
            headerView.AddView(contentToDisplayInHeader, 
                new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent));            
        }

        protected void CloseNavigationDrawer()
        {
            Activity.CloseNavigationDrawer();
        }

        public View Inflate(int viewId)
        {
            return Activity.LayoutInflater.Inflate(viewId, null);
        }

        private void AttachFabListener()
        {
            Activity.Fab.SetOnClickListener(this);
        }

        protected void SetupFab(int fabDrawableResourceId)
        {
            Activity.Fab.FabDrawable = Resources.GetDrawable(fabDrawableResourceId);
            
        }

        public void ShowFab()
        {
            Activity.Fab.Show();
            Activity.Fab.Visibility = ViewStates.Visible;
        }

        public void HideFab(bool animate = true)
        {
            if (animate)
            {
                Activity.Fab.Hide();
            }
            else
            {
                Activity.Fab.Visibility = ViewStates.Invisible;
            }
                
        }

        // Override this if you want to be notified when the Fab was clicked. 
        // Make sure you have called ShowFab first. 
        protected virtual void OnFabClicked()
        {

        }

        // Expand the FAB so that it takes up the whole screen. 
        // The argument viewContentToDisplay will be displayed as content. 
        // The view is automatically hidden when the user clicks back.
        public void ExpandFabToOverlayView(View viewContentToDisplay)
        {
            Activity.Window.AddFlags(WindowManagerFlags.Fullscreen);

            var fabOverlayView = Activity.FindViewById<ViewGroup>(Resource.Id.fab_overlay_content);
            fabOverlayView.RemoveAllViews();
            fabOverlayView.SetBackgroundColor(Activity.Fab.FabColor);
            fabOverlayView.Visibility = ViewStates.Visible;
            fabOverlayView.AddView(viewContentToDisplay);

            var cx = (Activity.Fab.Left + Activity.Fab.Right) / 2;
            var cy = (Activity.Fab.Top + Activity.Fab.Bottom) / 2;

            var finalRadius = Math.Max(fabOverlayView.Width, fabOverlayView.Height) * 1.5;

            var anim = XamarinViewAnimationUtils.CreateCircularReveal(fabOverlayView, cx, cy, 0, (float)finalRadius);
            anim.SetInterpolator(new AccelerateDecelerateInterpolator());
            anim.SetDuration(750);
            anim.Start();
        }

        // Fragments can host other fragments. When you add a fragment using this method
        // it does not go onto the Back Stack, which is what you usually want for fragments in a tabbed 
        // layout. See http://developer.android.com/training/implementing-navigation/temporal.html
        //
        protected void ShowNestedFragment(int viewId, Fragment fragment)
        {
            ChildFragmentManager
                .BeginTransaction()
                .Replace(viewId, fragment)
                .Commit();
        }

        // Call this to hide the header (ToolBar, Tabs, Search) when the user scrolls down. 
        // This frees up more of the screen which is important on smaller devices. 
        // The header is automatically reshown when the user scrolls upwards. 
        public void EnableAutoHideHeaderOnScroll(ListView list)
        {
            var listener = new HideHeaderOnScrollListener { ActionBar = Activity.SupportActionBar };
            list.SetOnScrollListener(listener);
        }

        private void Reset()
        {
            if (ResetState)
            {
                Activity.RemoveTabs();
            }
            Activity.ResetState(ResetState);
//            var search = Activity.FindViewById<EditText>(Resource.Id.search_widget);
//            search.Visibility = ViewStates.Gone;
        }

        //Used to identify fragments on the Back Stack
        public string GetTag()
        {
            return GetType().Name;
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

        // Return true if back press has been handled, otherwise return false. 
        // When returning false the FragmentHostActivity will remove this fragment from the Back Stack
        // and show the previous fragment. 
        public virtual bool OnBackPressed()
        {
            var overlay = Activity.FindViewById<View>(Resource.Id.fab_overlay_content);
            if (overlay.Visibility == ViewStates.Visible)
            {
                HideOverlayView();
                return true;
            }
            return false;
        }

        protected void HideOverlayView()
        {
            var overlay = Activity.FindViewById<View>(Resource.Id.fab_overlay_content);
            Activity.Window.ClearFlags(WindowManagerFlags.Fullscreen);
            overlay.Visibility = ViewStates.Invisible;            
        }

        protected void SetupSearch()
        {
            Activity.SearchWidget.Visibility = ViewStates.Visible;
        }

        private void AttachSearchListener()
        {
            Activity.SearchWidget.SetOnEditorActionListener(this);
        }

        public bool OnEditorAction(TextView view, ImeAction actionId, KeyEvent e)
        {
            if (actionId == ImeAction.Search)
            {
                OnSearch(view.Text);
                return true;
            }
            return false;
        }

        protected virtual void OnSearch(string text)
        {
            Toast.MakeText(Activity, text, ToastLength.Long).Show();
        }

        public void GoBack()
        {
            Activity.OnBackPressed();
        }
 
        public void GoBackTo(Type fragment) 
        {
            Activity.GoBackTo(fragment);
        }

        public void OnClick(View view)
        {
            if (view == Activity.Fab)
            {
                if (IsVisible)
                {
                    OnFabClicked();
                }
            }
        }
    }
}