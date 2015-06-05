
using System.Collections.Generic;
using Android.OS;
using Android.Views;
using Distributr.Mobile.Core.Data.Sequences;
using Distributr.Mobile.Core.MakeOrder;
using Distributr.Mobile.Core.MakeSale;
using Distributr.Mobile.Core.OrderSale;
using Distributr.Mobile.Core.Products;
using Distributr.Mobile.Core.Util;
using Distributr.Mobile.Data;
using Distributr.Mobile.Products;
using Distributr.Mobile.Summary;

namespace Distributr.Mobile.MakeOrder
{
    public class MakeOrderSummaryFragment : BaseSummaryFragment
    {
      private OrderProcessor orderProcessor;

        public override void CreateChildViews(View parent, Bundle bundle)
        {
            base.CreateChildViews(parent, bundle);
            orderProcessor = Resolve<OrderProcessor>();
        }

        protected override Result<object> OnConfirmed()
        {
            var sequnceNumber = Resolve<Database>().SequenceNextValue(SequenceName.DocumentReference);

            var context = new MakeOrderEnvelopeContext(sequnceNumber, Order.Outlet, User, User.DistributorSalesman, Order);
            return  orderProcessor.Process(Order, context);
        }

        protected override void SetupConfirmationOverlayView(SummaryConfirmationOverlayView view)
        {
            view.SetTitleText("Confirm Order and send to Hub?");
        }

        protected override IEnumerable<BaseProductLineItem> GetLineItems()
        {
            return Order.LineItems;
        }

        protected override void ShowProductEditor(ProductLineItem lineItem)
        {
            var dialog = new ProductSelectorDialog(Activity);

            dialog.ItemStateChanged += delegate(ProductDetails productDetails)
            {
                MakeOrderFragment.AdjustOrder(Order, productDetails);
                ApplyOrder();
            };
            dialog.Show(new ProductDetails(lineItem));
        }
    }
}