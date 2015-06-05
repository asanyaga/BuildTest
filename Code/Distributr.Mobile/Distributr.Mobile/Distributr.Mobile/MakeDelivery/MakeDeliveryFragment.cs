using Android.OS;
using Android.Views;
using Distributr.Mobile.Core.OrderSale;
using Distributr.Mobile.OrderSale;

namespace Distributr.Mobile.MakeDelivery
{
    public class MakeDeliveryFragment : OrderHistoryFragment
    {
        public override void CreateChildViews(View parent, Bundle bundle)
        {
            base.CreateChildViews(parent, bundle);
            SetTitle(Resource.String.make_delivery);            
        }

        protected override View SetupEmptyView(View parent)
        {
            return parent.FindViewById<View>(Resource.Id.empty_deliveries);
        }

        protected override void HandleItemClicked(OrderOrSale orderOrSale)
        {
            var sale = Resolve<SaleRepository>().GetById(orderOrSale.OrderSaleId);
            App.Put(sale);
            Activity.Show(typeof (MakeDeliverySummaryFragment));
        }

        protected override string GetInitialQuery()
        {
            return OrderOrSale.OrdersPendingDelivery;
        }
    }
}