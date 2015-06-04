using Android.OS;
using Android.Views;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Mobile.Core.OrderSale;

namespace Distributr.Mobile.MakeDelivery
{
    public class MakeDeliveryForOutletFragment : MakeDeliveryFragment
    {
        private Outlet outlet;

        public override void CreateChildViews(View parent, Bundle bundle)
        {
            outlet = App.Get<Outlet>();
            base.CreateChildViews(parent, bundle);
        }

        protected override string GetInitialQuery()
        {
            return OrderOrSale.OrdersPendingDeliveryForOutlet(outlet.Id);
        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater menuInflater)
        {
            
        }
    }
}
