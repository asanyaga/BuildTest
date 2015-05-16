using System;
using System.Collections.Generic;
using Android.OS;
using Android.Views;

namespace Mobile.Common.Core
{
    public class TabbedFragment<U> :BaseFragment<U>
    {
        private List<TabModel> tabs = new List<TabModel>();

        public override void CreateChildViews(View parent, Bundle bundle)
        {
            throw new NotImplementedException();
        }

        public void AddTab(TabModel model)
        {
            tabs.Add(model);
        }

        protected override void Paused()
        {
            Activity.RemoveTabs();
        }

        protected override void Resumed()
        {
            foreach(var tab in tabs)
            {
                Activity.AddTab(tab);
            }
        }
    }
}