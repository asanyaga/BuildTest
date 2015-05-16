using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.WSAPI.Lib.Services.Routing.Repository;
using Distributr.Core.Utility;
using Distributr.WSAPI.Lib.Services.Routing;
using Distributr.Core.Commands;

namespace Distributr.WSAPI.Lib.ResponseBuilders.CommandRouter
{
    //public class CommandRouterResponseBuilder : ICommandRouterResponseBuilder
    //{
    //    ICommandRoutingRepository _commandRoutingRepository;
    //    ICoreObjectSerializationHelper _serializeHelper;
    //    public CommandRouterResponseBuilder(ICommandRoutingRepository commandRoutingRepository, ICoreObjectSerializationHelper serializationHelper)
    //    {
    //        _serializeHelper = serializationHelper;
    //        _commandRoutingRepository = commandRoutingRepository;
    //    }

    //    public DocumentCommandRoutingResponse GetNextDocumentCommand(Guid costCentreApplicationId, long lastDeliveredCommandRouteItemId)
    //    {
    //        if (lastDeliveredCommandRouteItemId > -1)
    //            _commandRoutingRepository.MarkAsDelivered(lastDeliveredCommandRouteItemId);

    //        CommandRouteItem cri = _commandRoutingRepository
    //            .GetUndeliveredByDestinationCostCentreApplicationId(costCentreApplicationId)
    //             .OrderBy(n => n.Id).FirstOrDefault();

    //        if (cri == null)
    //        {
    //            return new DocumentCommandRoutingResponse
    //            {
    //                CommandRouteItemId = 0,
    //                CommandType = "",
    //                Command = null,
    //                ErrorInfo = ""
    //            };
    //        }

    //        CommandType ct = (CommandType) Enum.Parse(typeof(CommandType), cri.CommandType);

    //        return new DocumentCommandRoutingResponse 
    //        {
    //            CommandRouteItemId = cri.Id,
    //            CommandType = cri.CommandType,
    //            ErrorInfo = "",
    //            Command = _serializeHelper.DeserializeCommandFromJSON(ct, cri.JsonCommand) as ICommand
    //        };

    //    }

    //    public BatchDocumentCommandRoutingResponse GetNextBatcDocumentCommand(Guid costCentreApplicationId, List<long> lastDeliveredCommandRouteItemIds, int batchSize)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public BatchDocumentCommandRoutingResponse GetNextBatcDocumentCommand(Guid costCentreApplicationId, long lastDeliveredCommandRouteItemId, int batchSize)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}
