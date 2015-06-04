using Distributr.Core.ClientApp;
using Distributr.Mobile.Core.Data;
using Distributr.Mobile.Core.Envelopes;
using Distributr.Mobile.Core.MakeDelivery;
using Distributr.Mobile.Core.OrderSale;
using Distributr.Mobile.Core.OrderSale.EnvelopeBuilders;
using Distributr.Mobile.Core.Outlets;
using Distributr.Mobile.Core.Util;
using Distributr.Mobile.Data;

namespace Distributr.Mobile.Core.Payments
{
    public class PaymentProcessor
    {
        private readonly Database database;
        private readonly OrderRepository orderRepository;
        private readonly IOutgoingCommandEnvelopeRouter envelopeRouter;

        public PaymentProcessor(Database database, IOutgoingCommandEnvelopeRouter envelopeRouter, OrderRepository orderRepository)
        {
            this.database = database;
            this.envelopeRouter = envelopeRouter;
            this.orderRepository = orderRepository;
        }

        public Result<object> Process(Order order, IEnvelopeContext context)
        {
            var envelopeBuilder = new DeliveryEnvelopeBuilder(order,
                new PaymentNoteEnvelopeBuilder(context,
                new ReceiptEnvelopeBuilder(context,
                new OutletVisitNoteEnvelopeBuilder(context))));

            return new Transactor(database).Transact(() =>
            {
                envelopeBuilder.Build().ForEach(e => envelopeRouter.RouteCommandEnvelope(e));
                order.ConfirmNewPayments();
                orderRepository.Save(order);
            });
        }
    }
}