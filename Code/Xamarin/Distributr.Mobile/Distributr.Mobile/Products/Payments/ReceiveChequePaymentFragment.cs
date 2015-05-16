using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Views;
using Distributr.Mobile.Login;
using Mobile.Common.Core;

namespace Distributr.Mobile.Products.Payments
{
    public class ReceiveChequePaymentFragment : NestedFragment<User>
    {

        public override void CreateChildViews(View parent, Bundle bundle)
        {
            SetTitle(Resource.String.receive_cheque);
            SetupFab();
        }

        private void SetupFab()
        {
            Color color = Resources.GetColor(Resource.Color.colorAlternateAction);
            Drawable drawable = Resources.GetDrawable(Resource.Drawable.ic_fab_accept);
            ShowFab(color, drawable);
        }
    }
}