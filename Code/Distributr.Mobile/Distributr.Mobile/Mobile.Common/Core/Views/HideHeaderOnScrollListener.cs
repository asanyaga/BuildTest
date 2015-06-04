
using Android.Support.V7.App;
using Android.Widget;
using System;

namespace Mobile.Common.Core.Views
{
    public class HideHeaderOnScrollListener : Java.Lang.Object, AbsListView.IOnScrollListener
    {
        public ActionBar ActionBar {get; set; }

        private bool hideToolbar;
        private int previousFirstVisvibleItem ;

        public void OnScroll(AbsListView view, int firstVisibleItem, int visibleItemCount, int totalItemCount)
        {

            if (firstVisibleItem > Math.Max(previousFirstVisvibleItem, 3))
            {
                //User scrolls down by at least one item
                previousFirstVisvibleItem = firstVisibleItem;
                hideToolbar = true;
            }
            else if (firstVisibleItem +1 < previousFirstVisvibleItem)
            {
                //User scrolls up by at least one item
                hideToolbar = false;
            }
        }

        public void OnScrollStateChanged(AbsListView view, ScrollState scrollState)
        {
            if (hideToolbar && ActionBar.IsShowing)
            {
                ActionBar.Hide();
            }
            else if (!hideToolbar && !ActionBar.IsShowing)
            {
                ActionBar.Show();
            }
        }
    }
}