using System.Collections.Generic;
using Distributr.Core.Commands.CommandPackage;
using Distributr.Mobile.Core.OrderSale;

namespace Distributr.Mobile.Core.Envelopes
{
    public interface IEnvelopeBuilder
    {
        void Handle(BaseProductLineItem item, decimal quantity);
        void Handle(Payment payment);
        List<CommandEnvelope> Build();
    }

    //A no-op builder which is used for teminating the chain
    public class NoOpEnvelopeBuilder : IEnvelopeBuilder
    {
        public void Handle(BaseProductLineItem item, decimal quantity) { }
        public void Handle(Payment payment) { }

        public List<CommandEnvelope> Build()
        {
            return new List<CommandEnvelope>();
        }
    }
}
