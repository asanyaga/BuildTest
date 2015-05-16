using System;
using System.Collections.Generic;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.CustomerSupport.Code.CCCommandProcessing;
using Distributr.CustomerSupport.Paging;
using Distributr.WSAPI.Lib.Services.CommandAudit;
using Distributr.WSAPI.Lib.Services.Routing;

namespace Distributr.CustomerSupport.Code.CCAudit
{
    public interface ICCCommandAuditViewModelBuilder
    {
        CCCommandProcessingDetailViewModel GetCommandProcessingDetails(Guid costCentreId, Guid applicationId,
            int currentPageIndex, int itemsPerPage, CommandProcessingStatus status, out int count);
        IEnumerable<CCComandRoutingItem> GetCommandRoutingItems(Guid costCentreId, DateTime date,
            int currentPageIndex, int itemsPerPage, out int count);
        IEnumerable<CostCentreApplication> GetCostCentreApplications(Guid costCentreId);
        IEnumerable<CostCentreRef> GetCostCentres();
        CCCommandProcessingDetailViewModel GetCommandAuditDetails(Guid costCentreId, Guid costCentreAppId, int pageIndex, int pageSize, out int count);
    }

    public class CostCentreRef
    {
        public string Name { get; set; }
        public Guid Id { get; set; }
        public string CostCentreType { get; set; }
    }
}
