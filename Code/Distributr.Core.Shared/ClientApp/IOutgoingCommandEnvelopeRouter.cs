using Distributr.Core.Commands.CommandPackage;

namespace Distributr.Core.ClientApp
{
    public interface IOutgoingCommandEnvelopeRouter
    {
        void RouteCommandEnvelope(CommandEnvelope envelope);
    }
}