using Distributr.Core.ClientApp;
using Distributr.Mobile.Core.Data;
using Distributr.Mobile.Core.Envelopes;
using Distributr.Mobile.Core.OrderSale;
using Distributr.Mobile.Core.OrderSale.EnvelopeBuilders;
using Distributr.Mobile.Core.Outlets;
using Distributr.Mobile.Core.Products;
using Distributr.Mobile.Core.Util;
using Distributr.Mobile.Data;

namespace Distributr.Mobile.Core.MakeDelivery
{
    public class DeliveryProcessor
    {
        private readonly Database database;
        private readonly OrderRepository orderRepository;
        private readonly IOutgoingCommandEnvelopeRouter envelopeRouter;
        private readonly IInventoryRepository inventoryRepository;

        public DeliveryProcessor(Database database, IOutgoingCommandEnvelopeRouter envelopeRouter, OrderRepository orderRepository, IInventoryRepository inventoryRepository)
        {
            this.database = database;
            this.envelopeRouter = envelopeRouter;
            this.orderRepository = orderRepository;
            this.inventoryRepository = inventoryRepository;
        }

        public Result<object> Process(Order order, IEnvelopeContext context)
        {

            var envelopeBuilder = new DeliveryEnvelopeBuilder(order,
                new CloseOrderEnvelopeBuilder(context,
                new DispatchNoteEnvelopeBuilder(context,
                new PaymentNoteEnvelopeBuilder(context,
                new ReceiptEnvelopeBuilder(context,
                new CreditNoteEnvelopeBuilder(context,
                new InventoryAdjustmentNoteEnvelopeBuilder(context, inventoryRepository,
                new OutletVisitNoteEnvelopeBuilder(context))))))));

            return new Transactor(database).Transact(() =>
            {
                envelopeBuilder.Build().ForEach(e => envelopeRouter.RouteCommandEnvelope(e));
                order.ConfirmApprovedLineItems();
                order.ConfirmNewPayments();

                order.ProcessingStatus = order.HasNoBackorderItems ? ProcessingStatus.Confirmed : ProcessingStatus.PartiallyFulfilled;
                orderRepository.Save(order);
            });
        }
    }
}
