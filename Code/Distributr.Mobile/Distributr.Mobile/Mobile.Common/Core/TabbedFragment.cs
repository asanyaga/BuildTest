using System.Collections.Generic;

namespace Mobile.Common.Core
{
    public abstract class TabbedFragment<U> :BaseFragment<U>
    {
        private readonly List<TabModel> tabs = new List<TabModel>();

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