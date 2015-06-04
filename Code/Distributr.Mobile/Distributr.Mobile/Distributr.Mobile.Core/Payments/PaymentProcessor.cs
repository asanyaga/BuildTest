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
        private readonly SaleRepository saleRepository;
        private readonly IOutgoingCommandEnvelopeRouter envelopeRouter;

        public PaymentProcessor(Database database, IOutgoingCommandEnvelopeRouter envelopeRouter, SaleRepository saleRepository)
        {
            this.database = database;
            this.envelopeRouter = envelopeRouter;
            this.saleRepository = saleRepository;
        }

        public Result<object> Process(Sale sale, IEnvelopeContext context)
        {
            var envelopeBuilder = new DeliveryEnvelopeBuilder(sale,
                new PaymentNoteEnvelopeBuilder(context,
                new ReceiptEnvelopeBuilder(context,
                new OutletVisitNoteEnvelopeBuilder(context))));

            return new Transactor(database).Transact(() =>
            {
                envelopeBuilder.Build().ForEach(e => envelopeRouter.RouteCommandEnvelope(e));
                sale.ConfirmNewPayments();
                saleRepository.Save(sale);
            });
        }
    }
}