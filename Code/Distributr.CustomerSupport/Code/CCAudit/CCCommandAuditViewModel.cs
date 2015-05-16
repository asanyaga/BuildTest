using System;
using System.Collections.Generic;
using Distributr.Core.Commands;
using Distributr.CustomerSupport.Code.CCCommandProcessing;
using Distributr.CustomerSupport.Paging;
using Distributr.WSAPI.Lib.Services.CommandAudit;
using Distributr.WSAPI.Lib.Services.Routing;

namespace Distributr.CustomerSupport.Code.CCAudit
{
    public class CCCommandAuditViewModel
    {
        public CostCentreRef CostCentre { get; set; }
        public Guid CostCentreId { get; set; }
        public Guid CostCentreAppId { get; set; }
        public CommandProcessingStatus CommandProcessingStatus { get; set; }
        public string Date { get; set; }
        public int PageIndex { get; set; }
        public int PageNumber { get { return PageIndex + 1; } }
        public int PageSize { get; set; }
        public int TotalItemCount { get; set; }
        public IPagedList<CCCommandProcessingDetailViewModel.CommandProcessingAuditItem> AuditItems { get; set; }
        public CCCommandProcessingDetailViewModel Details { get; set; }
        public IPagedList<CCComandRoutingItem> RoutingItems { get; set; }
    }

    public class CCCommandAuditGroupViewModel
    {
        public CostCentreRef CostCentre { get; set; }
        public Guid CostCentreId { get; set; }
        public Guid CostCentreAppId { get; set; }
        public string Date { get; set; }
        public int PageIndex { get; set; }
        public int PageNumber { get { return PageIndex + 1; } }
        public int PageSize { get; set; }
        public int TotalItemCount { get; set; }
        public IPagedList<CommandProcessingAuditGroup> AuditGroupItems { get; set; }
        public CCCommandProcessingDetailViewModel Details { get; set; }
        public IPagedList<CCComandRoutingItem> RoutingItems { get; set; }
    }
}