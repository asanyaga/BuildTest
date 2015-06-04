using Android.Content;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace Distributr.Mobile.Summary
{
    public class SummaryConfirmationOverlayView : LinearLayout
    {
        private TextView title;
        private Button confirmButton;
        private ViewGroup items;

        public SummaryConfirmationOverlayView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        protected override void OnFinishInflate()
        {
            base.OnFinishInflate();
            title = FindViewById<TextView>(Resource.Id.page_title);
            confirmButton = FindViewById<Button>(Resource.Id.confirm_button);
            items = FindViewById<ViewGroup>(Resource.Id.confirmation_items);
        }

        public void SetTitleText(string titleText)
        {
            title.Text = titleText;
        }

        public void SetConfirmButtonText(string confirmButtonText)
        {
            confirmButton.Text = confirmButtonText;
        }

        public void AddConfirmationItem(View item)
        {
            items.AddView(item);   
        }
    }

}