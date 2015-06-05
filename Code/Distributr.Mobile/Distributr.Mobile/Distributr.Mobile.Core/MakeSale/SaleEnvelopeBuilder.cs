using System.Collections.Generic;
using Distributr.Core.Commands.CommandPackage;
using Distributr.Mobile.Core.Envelopes;
using Distributr.Mobile.Core.OrderSale;

namespace Distributr.Mobile.Core.MakeSale
{
    public class SaleEnvelopeBuilder
    {
        private IEnvelopeBuilder RootBuilder { get; set; }
        private Order Order { get; set; }

        public SaleEnvelopeBuilder(Order order, IEnvelopeBuilder builder)
        {
            RootBuilder = builder;
            Order = order;
        }

        public List<CommandEnvelope> Build()
        {
            Order.AllInvoiceItems.ForEach(lineItem =>
            {
                RootBuilder.Handle(lineItem, lineItem.SaleQuantity);
            });
            Order.Payments.ForEach(payment =>
            {
                RootBuilder.Handle(payment);
            });
            return RootBuilder.Build();
        }
    }
}
