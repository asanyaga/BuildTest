using Android.Content;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace Mobile.Common.Core.Views
{
    public class ScreenHeader : LinearLayout
    {
        public ScreenHeader(Context context) : base(context)
        {
        }

        public ScreenHeader(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        public ScreenHeader(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
        }

        public ScreenHeader(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
        }

        public void Show()
        {
            Visibility = ViewStates.Visible;
        }

        public void Hide()
        {
            Visibility = ViewStates.Gone;
        }
    }
}