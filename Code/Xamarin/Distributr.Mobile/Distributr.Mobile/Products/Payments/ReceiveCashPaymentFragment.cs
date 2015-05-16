using Android.OS;
using Android.Views;
using Distributr.Mobile.Login;
using Mobile.Common;
using Mobile.Common.Core;

namespace Distributr.Mobile.Products.Payments
{
    public class ReceiveCashPaymentFragment : NestedFragment<User>
    {
        public override void CreateChildViews(View parent, Bundle bundle)
        {
            SetTitle(Resource.String.receive_cash);
            SetupFab();
        }

        private void SetupFab()
        {            
            var color = Resources.GetColor(Resource.Color.colorAlternateAction);
            var drawable = Resources.GetDrawable(Resource.Drawable.ic_fab_accept);
            ShowFab(color, drawable);
        }
    }
}