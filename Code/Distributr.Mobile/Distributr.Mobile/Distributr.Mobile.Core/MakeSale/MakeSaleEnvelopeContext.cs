using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Mobile.Core.OrderSale;
using Distributr.Mobile.Login;
using Order = Distributr.Mobile.Core.OrderSale.Order;

namespace Distributr.Mobile.Core.MakeSale
{
    public class MakeSaleEnvelopeContext : OrderSaleEnvelopeContext
    {
        public MakeSaleEnvelopeContext(long sequenceNumber, Outlet outlet, User user, CostCentre costCentre, Order order) : base(sequenceNumber, outlet, user, costCentre, order)
        {
        }
        
        public override int OrderTypeId
        {
            get { return (int) OrderType.DistributorPOS; }
        }

        public override string InventoryAdjustmentNoteReason
        {
            get { return "Sale Inventory"; }
        }

        public override string OrderSaleReference()
        {
            return ReferenceGenerator.NextSaleReference();
        }
    }
}
