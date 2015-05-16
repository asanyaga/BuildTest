using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Commands.DocumentCommands;
using Distributr.WSAPI.Lib.Services.CommandAudit;
using Distributr.WSAPI.Lib.Services.Routing;

namespace Distributr.WSAPI.Lib.System.Impl
{
    public class ReRouteDocumentCommandHandler : IReRouteDocumentCommandHandler
    {
        private ICommandRoutingOnRequestRepository _commandRoutingOnRequestRepository;
        private ICommandProcessingAuditRepository _commandProcessingAuditRepository;

        public ReRouteDocumentCommandHandler(ICommandRoutingOnRequestRepository commandRoutingOnRequestRepository, ICommandProcessingAuditRepository commandProcessingAuditRepository)
        {
            _commandRoutingOnRequestRepository = commandRoutingOnRequestRepository;
            _commandProcessingAuditRepository = commandProcessingAuditRepository;
        }

        public void Execute(ReRouteDocumentCommand command)
        {
            try
            {
                var commands = _commandRoutingOnRequestRepository.GetByParentDocumentId(command.DocumentId);
                foreach (var commandRouteOnRequest in commands)
                {
                    var routeCentre =
                        _commandRoutingOnRequestRepository.GetByRouteCentreByIdAndCostcentreId(
                            commandRouteOnRequest.Id, command.ReciepientCostCentreId);
                    if (routeCentre == null)
                    {
                        routeCentre = new CommandRouteOnRequestCostcentre();
                        routeCentre.CostCentreId = command.ReciepientCostCentreId;
                        routeCentre.CommandRouteOnRequestId = commandRouteOnRequest.Id;
                        routeCentre.CommandType = commandRouteOnRequest.CommandType;
                        routeCentre.DateAdded = DateTime.Now;
                        routeCentre.IsRetired = commandRouteOnRequest.IsRetired;
                        routeCentre.IsValid = true;
                        _commandRoutingOnRequestRepository.AddRoutingCentre(routeCentre);
                    }


                }
                _commandProcessingAuditRepository.SetCommandStatus(command.CommandId, CommandProcessingStatus.Complete);
            }catch(Exception ex)
            {
                _commandProcessingAuditRepository.SetCommandStatus(command.CommandId, CommandProcessingStatus.MarkedForRetry);
            }
        }
    }
}
