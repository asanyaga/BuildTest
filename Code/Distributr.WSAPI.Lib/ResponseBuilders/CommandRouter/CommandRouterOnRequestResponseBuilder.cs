using System;
using System.Collections.Generic;
using System.Text;
using Distributr.Core.Commands;
using Distributr.Core.Commands.DocumentCommands;
using Distributr.Core.Utility;
using Distributr.WSAPI.Lib.Services.CostCentreApplications;
using Distributr.WSAPI.Lib.Services.Routing;
using Distributr.WSAPI.Lib.Services.Routing.Repository;
using System.Linq;
using Distributr.Core.ClientApp.CommandResults;
using Newtonsoft.Json;

namespace Distributr.WSAPI.Lib.ResponseBuilders.CommandRouter
{
    [Obsolete("Command Envelope Refactoring")]
    public class CommandRouterOnRequestResponseBuilder : ICommandRouterResponseBuilder
    {
        ICommandRoutingOnRequestRepository _commandRoutingOnRequestRepository;
        ICostCentreApplicationService _costCentreApplicationService;
        public CommandRouterOnRequestResponseBuilder(ICommandRoutingOnRequestRepository commandRoutingOnRequestRepository, 
            ICostCentreApplicationService costCentreApplicationService
            )
        {
            _commandRoutingOnRequestRepository = commandRoutingOnRequestRepository;
            _costCentreApplicationService = costCentreApplicationService;
        }

        public DocumentCommandRoutingResponse GetNextDocumentCommand(Guid costCentreApplicationId, Guid costCenreId, long lastDeliveredCommandRouteItemId)
        {
            Guid ccid = _costCentreApplicationService.GetCostCentreFromCostCentreApplicationId(costCentreApplicationId);
            if (lastDeliveredCommandRouteItemId >0)
                _commandRoutingOnRequestRepository.MarkAsDelivered(lastDeliveredCommandRouteItemId, costCentreApplicationId);

            CommandRouteOnRequest cri = _commandRoutingOnRequestRepository
                .GetUndeliveredByDestinationCostCentreApplicationId(costCentreApplicationId,ccid);
                 

            if (cri == null)
            {
                return new DocumentCommandRoutingResponse
                {
                    CommandRouteItemId = 0,
                    CommandType = "",
                    Command = null,
                    ErrorInfo = ""
                };
            }

            //CommandType ct = (CommandType)Enum.Parse(typeof(CommandType), cri.CommandType);

            return new DocumentCommandRoutingResponse
            {
                CommandRouteItemId = cri.Id,
                CommandType = cri.CommandType,
                ErrorInfo = "",
                Command = JsonConvert.DeserializeObject<DocumentCommand>(cri.JsonCommand)
            };

        }

        public BatchDocumentCommandRoutingResponse GetNextBatcDocumentCommand(Guid costCentreApplicationId, Guid costCentreId, List<long> lastDeliveredCommandRouteItemIds, int batchSize)
        {

            if (lastDeliveredCommandRouteItemIds!=null && lastDeliveredCommandRouteItemIds.Count>0)
                _commandRoutingOnRequestRepository.MarkBatchAsDelivered(lastDeliveredCommandRouteItemIds, costCentreApplicationId);

            List<CommandRouteOnRequest> criList = _commandRoutingOnRequestRepository
                .GetUnexecutedBatchByDestinationCostCentreApplicationId(costCentreApplicationId, costCentreId, batchSize,true);
            BatchDocumentCommandRoutingResponse responce = new BatchDocumentCommandRoutingResponse();
            if (criList == null || criList.Count()==0)
            {
                responce.RoutingCommands = null;
                responce.CommandRoutingCount = 0;
                responce.ErrorInfo = "No Pending Download";
                responce.LastCommandRouteItemId = 0;
            }
            foreach (CommandRouteOnRequest cri in criList)
            {
                //CommandType ct = (CommandType) Enum.Parse(typeof (CommandType), cri.CommandType);

                DocumentCommandRoutingResponse commandResponse=new DocumentCommandRoutingResponse
                           {
                               CommandRouteItemId = cri.Id,
                               CommandType = cri.CommandType,
                               ErrorInfo = "",
                               Command = JsonConvert.DeserializeObject<DocumentCommand>(cri.JsonCommand)
                           };

                responce.RoutingCommands.Add(commandResponse);
                responce.LastCommandRouteItemId = cri.Id;
            }
            responce.CommandRoutingCount = criList.Count;
            responce.ErrorInfo = "Success";
            return responce;
        }
    }
}
