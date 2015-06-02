using Android.Content;
using Android.Views;
using Android.Widget;
using Distributr.Mobile.Core.ErrorLog;
using Mobile.Common.Core.Views;

namespace Distributr.Mobile.ErrorLog
{
    public class ErrorLogEntryListAdapter : FixedSizeListAdapter<ErrorLogEntry>
    {
        private readonly Context context;

        public ErrorLogEntryListAdapter(Context context) : base(context)
        {
            this.context = context;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            ViewHolder holder;

            if (convertView == null)
            {
                var inflator = LayoutInflater.From(context);
                convertView = inflator.Inflate(Resource.Layout.error_log_list_item, null);

                var errorMessage = convertView.FindViewById<TextView>(Resource.Id.error_message);
                var errorDate = convertView.FindViewById<TextView>(Resource.Id.error_date);

                holder = new ViewHolder() {ErrorMessage = errorMessage, ErrorDate = errorDate};
                convertView.Tag = holder;
            }
            else
            {
                holder = convertView.Tag as ViewHolder;
            }

            var errorLogEntry = GetItem(position);

            holder.ErrorMessage.Text = errorLogEntry.ErrorMessage;
            holder.ErrorDate.Text = errorLogEntry.DateLastUpdated.ToString("yyyy-MM-dd HH:mm:ss");

            return convertView;
        }

        private class ViewHolder : Java.Lang.Object
        {
            public TextView ErrorMessage { get; set; }
            public TextView ErrorDate { get; set; }
        }
    }
}