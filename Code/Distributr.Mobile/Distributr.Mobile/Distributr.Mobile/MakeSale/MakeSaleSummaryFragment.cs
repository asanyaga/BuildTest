using System.Collections.Generic;
using Android.OS;
using Android.Views;
using Distributr.Mobile.Core.Data.Sequences;
using Distributr.Mobile.Core.MakeSale;
using Distributr.Mobile.Core.OrderSale;
using Distributr.Mobile.Core.Products;
using Distributr.Mobile.Core.Util;
using Distributr.Mobile.Data;
using Distributr.Mobile.Products;
using Distributr.Mobile.Summary;

namespace Distributr.Mobile.MakeSale
{
    public class MakeSaleSummaryFragment : BaseSummaryFragment
    {
        private SaleProcessor saleProcessor;

        public override void CreateChildViews(View parent, Bundle bundle)
        {
            base.CreateChildViews(parent, bundle);
            saleProcessor = Resolve<SaleProcessor>();
        }

        protected override Result<object> OnConfirmed()
        {
            var sequnceNumber = Resolve<Database>().SequenceNextValue(SequenceName.DocumentReference);

            var context = new MakeSaleEnvelopeContext(sequnceNumber, Order.Outlet, User, User.DistributorSalesman, Order);
            return saleProcessor.Process(Order, context);
        }

        protected override void SetupConfirmationOverlayView(SummaryConfirmationOverlayView view)
        {
            view.SetTitleText("Confirm sale and send to Hub?");
        }

        protected override IEnumerable<BaseProductLineItem> GetLineItems()
        {
            return Order.AllInvoiceItems;
        }

        protected override void ShowProductEditor(ProductLineItem lineItem)
        {
            var dialog = new ProductSelectorDialog(Activity);
            dialog.ItemStateChanged += delegate(ProductDetails productWrapper)
            {
                MakeSaleFragment.AdjustSale(Order, productWrapper);
                ApplyOrder();
            };

            dialog.Show(new ProductDetails(lineItem), allowSellReturnables: true, showAvailable: true);
        }
    }
}