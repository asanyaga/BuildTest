using Distributr.Core.Domain.Transactional.DocumentEntities.OrderDocumentEntities;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;

namespace Distributr.Core.Repository.Transactional.DocumentRepositories.OrderRepositories
{
    public interface IOutletMainOrderRepository
    {
        QueryResult<OutletMainOrderSummary> ListAllOrders(QueryOutletOrder queryOutletOrder);
        QueryResult<OutletMainOrderSummary> PendingApprovalQuery(QueryOutletOrder q);
        QueryResult<OutletMainOrderSummary> PendingDispatchQuery(QueryOutletOrder q);
        QueryResult<OutletMainOrderSummary> OutstandingPaymentQuery(QueryOutletOrder q);
        QueryResult<OutletMainOrderSummary> RejectedQuery(QueryOutletOrder q);
        QueryResult<OutletMainOrderSummary> DispatchedQuery(QueryOutletOrder q);
    }
}