using System.Collections.Generic;
using Android.OS;
using Android.Views;
using Android.Widget;
using Distributr.Mobile.Login;
using Distributr.Mobile.SidePanel.Sync;
using Mobile.Common;
using Mobile.Common.Core;

namespace Distributr.Mobile.SidePanel
{
    public class SidePanelFragment : NestedFragment<User>
    {
        private readonly SidePanelNavigationFragment navigationFragment = new SidePanelNavigationFragment() { RetainInstance = true };
        private readonly SidePanelSyncFragment syncFragment = new SidePanelSyncFragment() { RetainInstance = true };
        private readonly SidePanelSettingsFragment settingsFragment = new SidePanelSettingsFragment() { RetainInstance = true };

        private List<ImageView> buttons;

        public override void CreateChildViews(View parent, Bundle bundle)
        {
            buttons = new List<ImageView>();

            SetupView(parent, Resource.Id.screen_list, navigationFragment);
            SetupView(parent, Resource.Id.sync, syncFragment);
            SetupView(parent, Resource.Id.settings, settingsFragment);

            ShowView(buttons[0], navigationFragment);
        }

        private void SetupView(View parent, int viewId, BaseFragment<User> fragment)
        {
            var view = parent.FindViewById<ImageView>(viewId);
            buttons.Add(view);

            view.Click += delegate
            {
                ShowView(view, fragment);
            };
        }

        private void ShowView(ImageView selected, BaseFragment<User> fragment)
        {
            ShowNestedFragment(Resource.Id.navigation_fragment_container, fragment);
            selected.SetAlpha(0xFF);
            buttons.ForEach(v =>
            {
                if (v != selected)
                {
                    //30 % opacity
                    v.SetAlpha(0x4D);
                }
            });
        }
    }
}