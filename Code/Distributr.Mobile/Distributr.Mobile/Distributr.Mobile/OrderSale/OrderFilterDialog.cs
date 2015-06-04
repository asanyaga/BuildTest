using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Widget;

namespace Distributr.Mobile.OrderSale
{
    public delegate void EventHandler(OrderFilter filter);

    public class OrderFilterDialog : Dialog
    {
        public EventHandler OnApply;

        public OrderFilterDialog(Context context): base(new ContextThemeWrapper(context, Resource.Style.AppTheme))
        {
            RequestWindowFeature((int)WindowFeatures.NoTitle);
        }

        public void Show(OrderFilter filter)
        {
            SetupViews(filter);
            FindViewById<Button>(Resource.Id.apply).Click += delegate
            {
                Dismiss();
                OnApply(filter);
            };

            base.Show();
        }

        private void SetupViews(OrderFilter filter)
        {
            //SetTitle("Filter Orders");
            SetContentView(Resource.Layout.order_filter_dialog);

            var orderStatuses = FindViewById<ViewGroup>(Resource.Id.order_statuses);
            filter.ProcessingStatuses.ForEach(s =>
            {
                var checkBox = (CheckBox)LayoutInflater.From(Context).Inflate(Resource.Layout.order_filter_dialog_checkbox, null);
                checkBox.CheckedChange += delegate { s.Selected = checkBox.Checked; };
                checkBox.Text = s.DisplayName;
                checkBox.Checked = s.Selected;
                orderStatuses.AddView(checkBox);
            });

            var startDate = FindViewById<TextView>(Resource.Id.start_date);
            startDate.PaintFlags = startDate.PaintFlags | PaintFlags.UnderlineText;
            startDate.Text = filter.StartDate.ToString("yyyy-MM-dd");

            startDate.Click += delegate
            {
                new DatePickerDialog(Context,
                    delegate(object sender, DatePickerDialog.DateSetEventArgs e)
                    {
                        filter.StartDate = e.Date;
                        startDate.Text = filter.StartDate.ToString("yyyy-MM-dd");
                    },
                    filter.StartDate.Year,
                    filter.StartDate.Month - 1,
                    filter.StartDate.Day)
                    .Show(); 
            };

            var endDate = FindViewById<TextView>(Resource.Id.end_date);
            endDate.PaintFlags = endDate.PaintFlags | PaintFlags.UnderlineText;
            endDate.Text = filter.EndDate.ToString("yyyy-MM-dd");

            endDate.Click += delegate
            {
                new DatePickerDialog(Context,
                    delegate(object sender, DatePickerDialog.DateSetEventArgs e)
                    {
                        filter.EndDate = e.Date;
                        endDate.Text = filter.EndDate.ToString("yyyy-MM-dd");
                    },
                    filter.EndDate.Year,
                    filter.EndDate.Month - 1,
                    filter.EndDate.Day)
                    .Show();
            };
        }
    }
}