using Android.OS;
using Android.Views;
using Distributr.Mobile.Login;
using Mobile.Common;
using Mobile.Common.Core;

namespace Distributr.Mobile.Payments
{
    public class ReceivePaymentFragment : TabbedFragment<User>
    {
        public override void CreateChildViews(View parent, Bundle bundle)
        {
            SetTitle(Resource.String.receive_payment);
            SetupFab(Resource.Drawable.ic_check);
            SetupTabs();
        }

        private void SetupTabs()
        {
            var viewId = Resource.Id.payment_fragment_container;
            BaseFragment<User> receiveCashFragment = new ReceiveCashPaymentFragment();
            BaseFragment<User> receiveChequeFragment = new ReceiveChequePaymentFragment();
            
            ShowNestedFragment(viewId, receiveCashFragment);

            AddTab(new TabModel(Resources.GetString(Resource.String.cash))		
            {		
                OnTabSelected = () => { ShowNestedFragment(viewId, receiveCashFragment); }		
            });

            AddTab(new TabModel(Resources.GetString(Resource.String.cheque))
            {
                OnTabSelected = () => { ShowNestedFragment(viewId, receiveChequeFragment); }
            });

            AddTab(new TabModel(Resources.GetString(Resource.String.mmoney))
            {
                OnTabSelected = () => { ShowNestedFragment(viewId, receiveChequeFragment); }
            }
            );
        }
    }
}