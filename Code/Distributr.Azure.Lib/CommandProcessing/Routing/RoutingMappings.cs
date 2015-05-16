using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Commands.DocumentCommands;
using Distributr.WSAPI.Lib.Services.Routing;
using Newtonsoft.Json;

namespace Distributr.Azure.Lib.CommandProcessing.Routing
{
    public static class RoutingMappings
    {
        public static CommandRouteOnRequest Map(this CommandRouteOnRequestTable routeOnRequestTable)
        {
            return new CommandRouteOnRequest
                {
                    Id = routeOnRequestTable.Id,
                    CommandId = routeOnRequestTable.CommandId,
                    DocumentId = routeOnRequestTable.DocumentId,
                    DateCommandInserted = routeOnRequestTable.DateCommandInserted,
                    CommandGeneratedByCostCentreApplicationId = routeOnRequestTable.CommandGeneratedByCostCentreApplicationId,
                    CommandGeneratedByUserId = routeOnRequestTable.CommandGeneratedByUserId,
                    CommandType = routeOnRequestTable.CommandType,
                    JsonCommand = routeOnRequestTable.JsonCommand,
                    DocumentParentId = routeOnRequestTable.DocumentParentId,
                    DateAdded = routeOnRequestTable.DateAdded,
                    IsRetired = routeOnRequestTable.IsRetired
                };
        }

        public static CommandRouteOnRequestTable Map(this CommandRouteOnRequest routeOnRequest)
        {
             DocumentCommand dc = JsonConvert.DeserializeObject<DocumentCommand>(routeOnRequest.JsonCommand);
            return new CommandRouteOnRequestTable
                {
                    PartitionKey = dc.CommandGeneratedByCostCentreId.ToString(),
                    RowKey = routeOnRequest.Id.RouteOnRequestIdFormat(),
                    Id = routeOnRequest.Id,
                    CommandId = routeOnRequest.CommandId,
                    DocumentId = routeOnRequest.DocumentId,
                    DateCommandInserted = routeOnRequest.DateCommandInserted,
                    CommandGeneratedByCostCentreApplicationId = routeOnRequest.CommandGeneratedByCostCentreApplicationId,
                    CommandGeneratedByUserId = routeOnRequest.CommandGeneratedByUserId,
                    CommandType = routeOnRequest.CommandType,
                    JsonCommand = routeOnRequest.JsonCommand,
                    DocumentParentId = routeOnRequest.DocumentParentId,
                    DateAdded = routeOnRequest.DateAdded,
                    IsRetired = routeOnRequest.IsRetired
                };
        }

        public static CommandRouteOnRequestCostcentre Map(this CommandRouteOnRequestCostCentreTable routeOnRequestCostCentreTable)
        {
            return new CommandRouteOnRequestCostcentre
                {
                    Id = routeOnRequestCostCentreTable.Id,
                    CommandRouteOnRequestId = routeOnRequestCostCentreTable.CommandRouteOnRequestId,
                    CostCentreId = routeOnRequestCostCentreTable.DestinationCostCentreId,
                    IsValid = routeOnRequestCostCentreTable.IsValid,
                    DateAdded = routeOnRequestCostCentreTable.DateAdded,
                    IsRetired = routeOnRequestCostCentreTable.IsRetired,
                    CommandType = routeOnRequestCostCentreTable.CommandType
                };
        }

        public static CommandRouteOnRequestCostCentreTable Map(this CommandRouteOnRequestCostcentre routeOnRequestCostcentre, Guid sourceCCId)
        {
            return new CommandRouteOnRequestCostCentreTable
                {
                    PartitionKey = routeOnRequestCostcentre.CommandRouteOnRequestId.RouteOnRequestIdFormat(),
                    RowKey = routeOnRequestCostcentre.CostCentreId.ToString(),
                    Id = routeOnRequestCostcentre.Id,
                    CommandRouteOnRequestId = routeOnRequestCostcentre.CommandRouteOnRequestId,
                    DestinationCostCentreId = routeOnRequestCostcentre.CostCentreId,
                    SourceCostCentreId = sourceCCId,
                    IsValid = routeOnRequestCostcentre.IsValid,
                    DateAdded = routeOnRequestCostcentre.DateAdded,
                    IsRetired = routeOnRequestCostcentre.IsRetired,
                    CommandType = routeOnRequestCostcentre.CommandType
                };
        }

        public static string RouteOnRequestIdFormat(this long id)
        {
            return string.Format("{0:0000000000}", id);
        }
        public static string RouteOnRequestIdFormat(this int id)
        {
            return string.Format("{0:0000000000}", id);
        }
    }
}
