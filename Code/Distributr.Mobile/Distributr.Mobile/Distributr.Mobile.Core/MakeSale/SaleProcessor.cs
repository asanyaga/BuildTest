using Distributr.Core.ClientApp;
using Distributr.Mobile.Core.Data;
using Distributr.Mobile.Core.Envelopes;
using Distributr.Mobile.Core.OrderSale;
using Distributr.Mobile.Core.OrderSale.EnvelopeBuilders;
using Distributr.Mobile.Core.Outlets;
using Distributr.Mobile.Core.Products;
using Distributr.Mobile.Core.Util;
using Distributr.Mobile.Data;

namespace Distributr.Mobile.Core.MakeSale
{
    public class SaleProcessor
    {
        private readonly Database database;
        private readonly IOutgoingCommandEnvelopeRouter envelopeRouter;
        private readonly OrderRepository orderRepository;
        private readonly InventoryRepository inventoryRepository;

        public SaleProcessor(Database database, IOutgoingCommandEnvelopeRouter envelopeRouter, OrderRepository orderRepository, InventoryRepository inventoryRepository)
        {
            this.database = database;
            this.envelopeRouter = envelopeRouter;
            this.orderRepository = orderRepository;
            this.inventoryRepository = inventoryRepository;
        }

        public Result<object> Process(Order order, IEnvelopeContext context)
        {
            order.ApproveNewLineItems();

            var envelopeBuilder = new SaleEnvelopeBuilder(order,
                new MainOrderEnvelopeBuilder(context,
                new CloseOrderEnvelopeBuilder(context,
                new ExternalDocRefEnvelopeBuilder(context,
                new DispatchNoteEnvelopeBuilder(context,
                new InvoiceEnvelopeBuilder(context,
                new PaymentNoteEnvelopeBuilder(context,
                new ReceiptEnvelopeBuilder(context,
                new InventoryAdjustmentNoteEnvelopeBuilder(context, inventoryRepository,
                new OutletVisitNoteEnvelopeBuilder(context))))))))));

            return new Transactor(database).Transact(() =>
            {
                envelopeBuilder.Build().ForEach(e => envelopeRouter.RouteCommandEnvelope(e));
                inventoryRepository.AdjustInventoryForSale(order);
                order.OrderReference = context.OrderSaleReference();
                order.ConfirmAllLineItems();
                order.ConfirmNewPayments();
                order.ProcessingStatus = ProcessingStatus.Confirmed;
                orderRepository.Save(order);
            });
        }
    }
}
