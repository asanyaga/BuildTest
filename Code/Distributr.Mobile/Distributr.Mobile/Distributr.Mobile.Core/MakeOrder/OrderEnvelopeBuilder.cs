using System.Collections.Generic;
using Distributr.Core.Commands.CommandPackage;
using Distributr.Mobile.Core.Envelopes;
using Distributr.Mobile.Core.OrderSale;

namespace Distributr.Mobile.Core.MakeOrder
{
    public class OrderEnvelopeBuilder
    {
        private IEnvelopeBuilder RootBuilder { get; set; }
        private Order Order { get; set; }

        public OrderEnvelopeBuilder(Order order, IEnvelopeBuilder builder)
        {
            RootBuilder = builder;
            Order = order;
        }

        public List<CommandEnvelope> Build()
        {
            Order.AllItems.ForEach(lineItem =>
            {
                RootBuilder.Handle(lineItem, lineItem.Quantity);
            });
            Order.Payments.ForEach(payment =>
            {
                RootBuilder.Handle(payment);
            });
            return RootBuilder.Build();
        }
    }
}
