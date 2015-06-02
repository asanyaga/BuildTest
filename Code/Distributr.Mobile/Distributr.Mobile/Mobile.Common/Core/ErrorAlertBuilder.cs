using System;
using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;

namespace Mobile.Common.Core
{
    public class ErrorAlertBuilder
    {
        private readonly Context context;
        private readonly LayoutInflater inflater;

        public ErrorAlertBuilder(Activity activity)
        {
            context = activity;
            inflater = activity.LayoutInflater;
        }

        public AlertDialog build(string summaryText, Exception exception)
        {
            var layout = inflater.Inflate(Resource.Layout.error_alert, null);
            var summary = layout.FindViewById<TextView>(Resource.Id.summary_text);
            summary.Text = summaryText;

            if (exception != null)
            {
                var technical = layout.FindViewById<TextView>(Resource.Id.technical_text);
                technical.Text = exception.ToString();
            }

            var dialog = new AlertDialog.Builder(context)
                .SetTitle(Resource.String.error_alert_title)
                .SetCancelable(false)
                .SetView(layout)
                .SetPositiveButton("OK", (sender, args) =>
                {
                    //Do nothing - the dialog is automatically closed on click
                }).Create();

            var tabs = layout.FindViewById<TabHost>(Android.Resource.Id.TabHost);

            tabs.Setup();

            var spec = tabs.NewTabSpec("tag1");

            spec.SetContent(Resource.Id.summary_scroll);
            spec.SetIndicator("Summary");
            tabs.AddTab(spec);

            spec = tabs.NewTabSpec("tag2");
            spec.SetContent(Resource.Id.technical_scroll);
            spec.SetIndicator("Technical");
            tabs.AddTab(spec);

            return dialog;
        }
    }
}