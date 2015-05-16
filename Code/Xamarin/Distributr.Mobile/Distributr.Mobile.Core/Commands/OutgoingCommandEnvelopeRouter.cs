using System;
using Distributr.Core.ClientApp;
using Distributr.Core.Commands.CommandPackage;
using Newtonsoft.Json;

namespace Distributr.Mobile.Core
{
    public class OutgoingCommandEnvelopeRouter : IOutgoingCommandEnvelopeRouter
    {
        public void RouteCommandEnvelope(CommandEnvelope envelope)
        {
            var json = JsonConvert.SerializeObject(envelope);
        }
    }
}