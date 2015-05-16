using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master.CostCentreEntities;

namespace Distributr.WSAPI.Lib.Services.Routing
{
    [Obsolete("Command Envelope Refactoring")]
    public interface ICommandRoutingOnRequestRepository
    {
        CommandRouteOnRequest GetById(long id);
        List<CommandRouteOnRequestCostcentre> GetByCommandRouteOnRequestId(long id);
        CommandRouteOnRequest GetByCommandId(Guid id);
        List<CommandRouteOnRequest> GetByDocumentId(Guid id);
        List<CommandRouteOnRequest> GetByParentDocumentId(Guid parentId);
        CommandRouteOnRequestCostcentre GetByRouteCentreByIdAndCostcentreId(long id, Guid costcentreId);
        CommandRouteOnRequest GetUndeliveredByDestinationCostCentreApplicationId(Guid costCentreApplicationId, Guid costCentreId);
        long Add(CommandRouteOnRequest commandRouteItem);
        void AddRoutingCentre(CommandRouteOnRequestCostcentre commandRouteOnRequestCostcentre);
        void MarkAsDelivered(long commandRouteOnRequestId, Guid costCentreApplicationId);
        void MardAdDeliveredAndExecuted(long commandRouteOnRequestId, Guid costCentreApplicationId);
        void MarkBatchAsDelivered(List<long> commandRouteOnRequestId, Guid costCentreApplicationId);
        List<CommandRouteOnRequest> GetUnexecutedBatchByDestinationCostCentreApplicationId(Guid costCentreApplicationId, Guid costCentreId, int batchSize, bool includeArchived );
        void RetireCommands(Guid parentCommandId);
        void UnRetireCommands(Guid docParentId);
        void MarkCommandsAsInvalid(Guid costCentreId);
        void CleanApplication(Guid applicationId);

        List<CommandRouteOnRequestCostcentre> TestCC(Guid ccid);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fromDate">Default to 30 days ago</param>
        /// <returns></returns>
        List<RouteOnRequestSummary> GetCCRouteOnRequestSummary(DateTime? fromDate);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fromDate">Default to 30 days ago</param>
        /// <returns></returns>
        List<RouteOnRequestDeliveredSummary> GetCCAppIdRouteOnRequestDeliveredSummary(DateTime? fromDate);
        List<CostCentreRouteOnRequestDetail> GetCostCentreRouteOnRequestDetail(Guid costCentreId, int dayOfYear, int year);
        List<long> GetCommandsAsIntegers(Guid costCentreId, DateTime forDate, int pageIndex, int pageSize, out int count);
        List<CCComandRoutingItem> GetCommandRoutingItems(List<long> costCentreCommands, List<Guid> costCentreApps);
        List<CommandRef> GetCommandRefs(List<long> costCentreCommands);
    }

    public class RouteOnRequestSummary
    {
        public Guid CostCentreId { get; set; }
        public int Count { get; set; }
        public int ValidCount { get; set; }
        public int RetiredCount { get; set; }
    }

    public class RouteOnRequestDeliveredSummary
    {
        public Guid DestinationCostCentreApplicationId { get; set; }
        public int DeliveredCount { get; set; }
    }
}
