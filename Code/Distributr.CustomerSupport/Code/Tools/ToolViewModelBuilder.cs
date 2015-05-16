using System;
using Distributr.WSAPI.Lib.Services.Routing;

namespace Distributr.CustomerSupport.Code.Tools
{
    public class ToolViewModelBuilder : IToolViewModelBuilder
    {
        private readonly ICommandRoutingOnRequestRepository _commandRoutingOnRequestRepository;

        public ToolViewModelBuilder(ICommandRoutingOnRequestRepository commandRoutingOnRequestRepository)
        {
            _commandRoutingOnRequestRepository = commandRoutingOnRequestRepository;
        }

        public void Retire(Guid docParentId)
        {
            _commandRoutingOnRequestRepository.RetireCommands(docParentId);
        }

        public void UnRetire(Guid docParentId)
        {
            _commandRoutingOnRequestRepository.UnRetireCommands(docParentId);
        }
    }
}