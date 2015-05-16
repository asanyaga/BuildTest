using System;
using Distributr.Core.CommandHandler.DocumentCommandHandlers.Retire;
using Distributr.Core.Commands.DocumentCommands;
using Distributr.WSAPI.Lib.Services.Routing;

namespace Distributr.WSAPI.Lib.Retire
{
    [Obsolete("Not being used, can remove")]
    public class RetireDocumentCommandHandler : IRetireDocumentCommandHandler
    {
        private ICommandEnvelopeRouteOnRequestCostcentreRepository _commandRoutingOnRequestRepository;

        public RetireDocumentCommandHandler(ICommandEnvelopeRouteOnRequestCostcentreRepository commandRoutingOnRequestRepository)
        {
            _commandRoutingOnRequestRepository = commandRoutingOnRequestRepository;
        }

        public void Execute(RetireDocumentCommand command)
        {
            _commandRoutingOnRequestRepository.RetireEnvelopes(command.DocumentId);
        }
    }
}
