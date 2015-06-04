using System.Collections.Generic;
using Distributr.Core.Commands.CommandPackage;
using Distributr.Mobile.Core.Envelopes;
using Distributr.Mobile.Core.OrderSale;

namespace Distributr.Mobile.Core.MakeDelivery
{
    public class DeliveryEnvelopeBuilder
    {
        private IEnvelopeBuilder RootBuilder { get; set; }
        private Order Order { get; set; }

        public DeliveryEnvelopeBuilder(Order order, IEnvelopeBuilder builder)
        {
            RootBuilder = builder;
            Order = order;
        }

        public List<CommandEnvelope> Build()
        {
            Order.ApprovedLineItems.ForEach(lineItem =>
            {
                RootBuilder.Handle(lineItem, lineItem.ApprovedQuantity);
            });
            Order.NewPayments.ForEach(payment =>
            {
                RootBuilder.Handle(payment);
            });
            return RootBuilder.Build();
        }
    }
}
