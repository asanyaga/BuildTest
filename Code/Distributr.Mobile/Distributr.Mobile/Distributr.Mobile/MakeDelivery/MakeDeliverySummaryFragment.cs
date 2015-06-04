using System;
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

            var product = Order.ItemAsProductWrapper(lineItem.Product.Id);

            dialog.ItemStateChanged += delegate(ProductWrapper productWrapper)
            {
                Order.AddOrUpdateSaleLineItem(productWrapper);
                ApplyOrder();
            };

            product.EachQuantity = lineItem.ApprovedQuantity % lineItem.Product.ContainerCapacity;
            product.MaxEachQuantity = lineItem.ApprovedQuantity;
            product.CaseQuantity = product.MaxCaseQuantity = Math.Floor(lineItem.ApprovedQuantity / lineItem.Product.ContainerCapacity);

            product.MaxCaseReturnableQuantity = lineItem.ContainerReturnable == null ? 0 : lineItem.ContainerReturnable.ApprovedQuantity;
            product.MaxEachReturnableQuantity = lineItem.ItemReturnable == null ? 0 : lineItem.ItemReturnable.ApprovedQuantity;
            
            dialog.Show(product, editProducts:false);
        }
    }
}