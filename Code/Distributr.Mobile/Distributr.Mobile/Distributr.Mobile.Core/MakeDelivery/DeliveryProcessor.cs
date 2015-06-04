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
        private readonly SaleRepository saleRepository;
        private readonly IOutgoingCommandEnvelopeRouter envelopeRouter;
        private readonly IInventoryRepository inventoryRepository;

        public DeliveryProcessor(Database database, IOutgoingCommandEnvelopeRouter envelopeRouter, SaleRepository saleRepository, IInventoryRepository inventoryRepository)
        {
            this.database = database;
            this.envelopeRouter = envelopeRouter;
            this.saleRepository = saleRepository;
            this.inventoryRepository = inventoryRepository;
        }

        public Result<object> Process(Sale sale, IEnvelopeContext context)
        {
            var envelopeBuilder = new DeliveryEnvelopeBuilder(sale,
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
                sale.ConfirmNewPayments();

                sale.ProcessingStatus = sale.HasNoBackorderItems ? ProcessingStatus.Confirmed : ProcessingStatus.PartiallyFulfilled;
                saleRepository.Save(sale);
            });
        }
    }
}
