using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Mobile.Core.OrderSale;
using Distributr.Mobile.Login;
using Order = Distributr.Mobile.Core.OrderSale.Order;

namespace Distributr.Mobile.Core.MakeOrder
{
    public class MakeOrderEnvelopeContext : OrderSaleEnvelopeContext
    {
        public MakeOrderEnvelopeContext(long sequenceNumber, Outlet outlet, User user, CostCentre costCentre, Order order) : base(sequenceNumber, outlet, user, costCentre, order)
        {
        }

        public override int OrderTypeId
        {
            get { return (int) OrderType.OutletToDistributor; }
        }

        public override string InventoryAdjustmentNoteReason
        {
            get { return "Order Inventory"; }
        }

        public override string OrderSaleReference()
        {
            return ReferenceGenerator.NextOrderReference();
        }
    }
}
