using System.Collections.Generic;
using Android.Views;
using Android.Widget;
using Distributr.Mobile.Core.Data.Sequences;
using Distributr.Mobile.Core.MakeDelivery;
using Distributr.Mobile.Core.MakeOrder;
using Distributr.Mobile.Core.OrderSale;
using Distributr.Mobile.Core.Products;
using Distributr.Mobile.Core.Util;
using Distributr.Mobile.Data;
using Distributr.Mobile.MakeSale;
using Distributr.Mobile.Products;
using Distributr.Mobile.Summary;

namespace Distributr.Mobile.MakeDelivery
{
    public class MakeDeliverySummaryFragment : BaseSummaryFragment
    {
        protected override Result<object> OnConfirmed()
        {
            var sequnceNumber = Resolve<Database>().SequenceNextValue(SequenceName.DocumentReference);
            var context = new MakeOrderEnvelopeContext(sequnceNumber, Order.Outlet, User, User.DistributorSalesman, Order);
            return Resolve<DeliveryProcessor>().Process(Order, context);
        }

        protected override void SetupConfirmationOverlayView(SummaryConfirmationOverlayView view)
        {
            view.SetTitleText("Confirm Delivery and send to Hub?");
        }

        protected override IEnumerable<BaseProductLineItem> GetLineItems()
        {
            return Order.AllInvoiceItems;
        }

        protected override void SetupDeliveryAddress(View parent)
        {
            parent.FindViewById<TextView>(Resource.Id.outlet_address).Text = GetAddress();
        }

        protected override void ShowProductEditor(ProductLineItem lineItem)
        {
            var dialog = new ProductSelectorDialog(Activity);

            dialog.ItemStateChanged += delegate(ProductDetails productWrapper)
            {
                MakeSaleFragment.AdjustSale(Order, productWrapper);
                ApplyOrder();
            };
            
            dialog.Show(new ProductDetails(lineItem), editProducts:false, allowSellReturnables:true);
        }
    }
}