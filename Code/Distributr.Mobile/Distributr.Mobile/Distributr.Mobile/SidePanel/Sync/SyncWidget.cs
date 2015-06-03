using System;
using Android.Content;
using Android.Util;
using Android.Views;
using Android.Widget;
using Distributr.Mobile.Core.Sync.Incoming;
using Distributr.Mobile.Sync;

namespace Distributr.Mobile.SidePanel.Sync
{
    public class SyncWidget : LinearLayout
    {
        private TextView title;
        private TextView lastUpdateTime;
        private TextView syncStatus;
        private View syncStatusIndicator;
        private ImageButton syncNowButton;

        public SyncWidget(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        protected override void OnFinishInflate()
        {
            base.OnFinishInflate();
            title = FindViewById<TextView>(Resource.Id.sync_type_label);
            lastUpdateTime = FindViewById<TextView>(Resource.Id.last_update_time);
            syncStatus = FindViewById<TextView>(Resource.Id.sync_status_message);
            syncStatusIndicator = FindViewById<LinearLayout>(Resource.Id.sync_status_indicator);
            syncNowButton = FindViewById<ImageButton>(Resource.Id.sync_now_button);
        }

        public void OnSyncNow(EventHandler handler)
        {
            syncNowButton.Click += handler;
        }

        public void SetActive()
        {
            syncStatusIndicator.SetBackgroundColor(Resources.GetColor(Resource.Color.color_action));
            syncNowButton.Enabled = false;
        }

        public void SetPaused()
        {
            syncStatusIndicator.SetBackgroundColor(Resources.GetColor(Resource.Color.sync_paused_color));
        }

        public void SetInacvtive()
        {
            syncStatusIndicator.SetBackgroundDrawable(Resources.GetDrawable(Resource.Drawable.sync_view_inactive));
            syncStatus.Text = Resources.GetString(Resource.String.side_panel_sync_status_inactive);
            syncNowButton.Enabled = true;
        }

        public void SetTitle(string title)
        {
            this.title.Text = title;
        }

        public void UpdateStatusMessage(string message)
        {
            syncStatus.Text = message;
        }

        public void SetLastUpdateTime(string time)
        {
            lastUpdateTime.Text = string.Format("Last synced {0}", time);
        }

        public void SetError()
        {
            syncStatus.Text = "Failed";
            syncStatusIndicator.SetBackgroundColor(Resources.GetColor(Resource.Color.color_error));
            syncNowButton.Enabled = true;
        }
    }
}