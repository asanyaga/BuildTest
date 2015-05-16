using Android.OS;
using Android.Views;
using Distributr.Mobile.Login;
using Mobile.Common;
using Mobile.Common.Core;

namespace Distributr.Mobile.Products.Payments
{
    public class ReceivePaymentFragment : TabbedFragment<User>
    {
        public override void CreateChildViews(View parent, Bundle bundle)
        {
            SetTitle(Resource.String.receive_payment);
            SetupTabs();
        }

        private void SetupTabs()
        {
            int viewId = Resource.Id.payment_fragment_container;
            BaseFragment<User> receiveCashFragment = new ReceiveCashPaymentFragment();
            BaseFragment<User> receiveChequeFragment = new ReceiveChequePaymentFragment();
            
            ShowFragment(viewId, receiveCashFragment);

            AddTab(new TabModel(Resources.GetString(Resource.String.cash))
                {
                    OnTabSelected = () => { ShowFragment(viewId, receiveCashFragment); }
                }
            );

            AddTab(new TabModel(Resources.GetString(Resource.String.cheque))
                {
                    OnTabSelected = () => { ShowFragment(viewId, receiveChequeFragment); }
                }
            );

            AddTab(new TabModel(Resources.GetString(Resource.String.mmoney))
                {
                    OnTabSelected = () => { ShowFragment(viewId, receiveChequeFragment); }
                }
            );
        }
    }
}