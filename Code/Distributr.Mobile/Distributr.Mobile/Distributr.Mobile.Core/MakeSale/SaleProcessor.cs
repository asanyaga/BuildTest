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
        private readonly SaleRepository saleRepository;
        private readonly InventoryRepository inventoryRepository;

        public SaleProcessor(Database database, IOutgoingCommandEnvelopeRouter envelopeRouter, SaleRepository saleRepository, InventoryRepository inventoryRepository)
        {
            this.database = database;
            this.envelopeRouter = envelopeRouter;
            this.saleRepository = saleRepository;
            this.inventoryRepository = inventoryRepository;
        }

        public Result<object> Process(Sale sale, IEnvelopeContext context)
        {

            var envelopeBuilder = new SaleEnvelopeBuilder(sale,
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
                inventoryRepository.AdjustInventoryForSale(sale);
                sale.OrderReference = context.OrderSaleReference();
                sale.ConfirmNewPayments();
                sale.ProcessingStatus = ProcessingStatus.Confirmed;
                saleRepository.Save(sale);
            });
        }
    }
}
