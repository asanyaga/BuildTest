using Distributr.Core.ClientApp;
using Distributr.Mobile.Core.Data;
using Distributr.Mobile.Core.Envelopes;
using Distributr.Mobile.Core.MakeOrder;
using Distributr.Mobile.Core.OrderSale;
using Distributr.Mobile.Core.OrderSale.EnvelopeBuilders;
using Distributr.Mobile.Core.Outlets;
using Distributr.Mobile.Core.Util;
using Distributr.Mobile.Data;

namespace Distributr.Mobile.Core.MakeSale
{
    public class OrderProcessor
    {
        private readonly Database database;
        private readonly SaleRepository saleRepository;
        private readonly IOutgoingCommandEnvelopeRouter envelopeRouter;

        public OrderProcessor(Database database, IOutgoingCommandEnvelopeRouter envelopeRouter, SaleRepository saleRepository)
        {
            this.database = database;
            this.envelopeRouter = envelopeRouter;
            this.saleRepository = saleRepository;
        }

        public Result<object> Process(Sale sale, IEnvelopeContext context)
        {
            var envelopeBuilder = new OrderEnvelopeBuilder(sale,
                new MainOrderEnvelopeBuilder(context,
                new ExternalDocRefEnvelopeBuilder(context,
                new PaymentNoteEnvelopeBuilder(context,
                new OutletVisitNoteEnvelopeBuilder(context)))));

            return new Transactor(database).Transact(() =>
            {
                envelopeBuilder.Build().ForEach(e => envelopeRouter.RouteCommandEnvelope(e));
                sale.OrderReference = context.OrderSaleReference();
                sale.ConfirmNewPayments();
                sale.ProcessingStatus = ProcessingStatus.Submitted;
                saleRepository.Save(sale);
            });
        }
    }
}
