using System.Collections.Generic;
using System.Linq;
using Android.OS;
using Android.Text;
using Android.Views;
using Android.Widget;
using Distributr.Mobile.Core.MakeOrder;
using Distributr.Mobile.Core.OrderSale;
using Distributr.Mobile.Core.Payments;
using Distributr.Mobile.Core.Util;

namespace Distributr.Mobile.Summary
{
    public class ReadOnlySummaryFragment : BaseSummaryFragment
    {
        public override void CreateChildViews(View parent, Bundle bundle)
        {
            base.CreateChildViews(parent, bundle);
            SetupFab(Resource.Drawable.ic_action_payment);
            HideFab(animate: false);
        }

        protected override Result<object> OnConfirmed()
        {
            var context = new MakeOrderEnvelopeContext(0, Order.Outlet, User, User.DistributorSalesman, Order);
            return Resolve<PaymentProcessor>().Process(Order, context);
        }

        protected override void OnMenuCreated()
        {
            HidePaymentOptionIfRequired();
        }

        protected override void Resumed()
        {
            if (Order.NewPayments.Any()) ShowFab();            
        }

        private void HidePaymentOptionIfRequired()
        {
            if (Order.ProcessingStatus == ProcessingStatus.Rejected
                || Order.ProcessingStatus == ProcessingStatus.Submitted)
            {
                Menu.FindItem(Resource.Id.receive_payment).SetVisible(false);
            }
        }

        protected override void SetupDeliveryAddress(View parent)
        {
            parent.FindViewById<TextView>(Resource.Id.outlet_address).Text = GetAddress();
        }

        protected override IEnumerable<BaseProductLineItem> GetLineItems()
        {
            return Order.AllInvoiceItems;
        }

        protected override void ShowProductEditor(ProductLineItem lineItem)
        {
            //Not shown for in read-only mode
        }

        protected override void SetupConfirmationOverlayView(SummaryConfirmationOverlayView view)
        {
            view.SetTitleText(Resources.GetString(Resource.String.payment_confirmation_screen_title));
        }

        protected override void SetupProductLineItem(ProductLineItem productLine, TextView quantity)
        {
            //Do nothing in read-only mode
        }
    }
}