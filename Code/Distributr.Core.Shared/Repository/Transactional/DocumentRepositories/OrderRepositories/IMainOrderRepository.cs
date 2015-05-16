using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.Util;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.OrderDocumentEntities;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Workflow;

namespace Distributr.Core.Repository.Transactional.DocumentRepositories.OrderRepositories
{
    public interface IMainOrderRepository : IDocumentRepository<MainOrder>
    {
        IPagedDocumentList<MainOrderSummary> PagedDocumentList(int page, int pageSize, DateTime startdate,
                                                               DateTime endDate, OrderType orderType,
                                                               DocumentStatus? documentStatus = null, string search = "");

        IEnumerable<MainOrderSummary> GetMainOrderSummariyList(DateTime startdate,
                                                              DateTime endDate, OrderType orderType,
                                                              DocumentStatus? documentStatus = null, string search = "");

        List<MainOrderSummary> GetPendingDispatch(Guid? routeId, Guid? salesmanId);
        List<MainOrderSummary> GetPurchaseOrderPendingReceive();
        MainOrder GetByDocumentReference(string docReference);
        IPagedDocumentList<MainOrderSummary> PagedPurchaseDocumentList(int page, int pageSize, DateTime startdate,
                                                              DateTime endDate, DocumentStatus? documentStatus , Guid? distributrId=null,
                                                               string search = "");

        bool HasOrdersPendingDispatch(Guid salesmanId);
        IEnumerable<MainOrderSummary> GetApproveOrderAndDateProcessedList(DateTime startdate,DateTime endDate,Guid? salesmanId);
        IEnumerable<MainOrderSummary> GetPendingOrderAndDateProcessedList(DateTime startdate, DateTime endDate, Guid? salesmanId);
        IEnumerable<MainOrderSummary> GetApproveOrderAndDateProcessedList(DateTime startdate, DateTime endDate, Guid? salesmanId,Guid? outletId);

        QueryResult<MainOrderSummary> Query(QueryStandard query, DateTime startdate,
                                                              DateTime endDate, DocumentStatus? documentStatus, Guid? distributrId = null);
    }
}
