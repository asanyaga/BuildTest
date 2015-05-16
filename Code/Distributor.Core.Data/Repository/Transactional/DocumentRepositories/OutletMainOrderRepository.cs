using System;
using System.Linq;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Transactional.DocumentEntities.OrderDocumentEntities;
using Distributr.Core.Repository.Transactional.DocumentRepositories.OrderRepositories;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;

namespace Distributr.Core.Data.Repository.Transactional.DocumentRepositories
{
    public class OutletMainOrderRepository: IOutletMainOrderRepository
    {
        private CokeDataContext _ctx;

        public OutletMainOrderRepository(CokeDataContext ctx)
        {
            _ctx = ctx;
        }

        public QueryResult<OutletMainOrderSummary> ListAllOrders(QueryOutletOrder queryOutletOrder)
        {
            var result = new QueryResult<OutletMainOrderSummary>();
            string sql = string.Format(Resources.OutletOrders.OutletOrdersResource.OutletOrders, queryOutletOrder.From.ToString("yyyy-MM-dd hh:mm:ss"), queryOutletOrder.To.ToString("yyyy-MM-dd hh:mm:ss"), queryOutletOrder.OutletId);
            var query = _ctx.ExecuteStoreQuery<OutletMainOrderSummary>(sql).ToList();
            if (!string.IsNullOrWhiteSpace(queryOutletOrder.Name))
                query = query.Where(k => k.Distributor.ToLower().Contains(queryOutletOrder.Name.ToLower())).ToList();
            result.Count = query.Count();
            result.Data = query.ToList();
            return result;
        }

        public QueryResult<OutletMainOrderSummary> PendingApprovalQuery(QueryOutletOrder q)
        {
            var result = new QueryResult<OutletMainOrderSummary>();
            string sql = string.Format(Resources.OutletOrders.OutletOrdersResource.OutletOrdersPendingApproval,
                                       q.From.ToString("yyyy-MM-dd hh:mm:ss"), q.To.ToString("yyyy-MM-dd hh:mm:ss"),
                                       q.OutletId);
            var query = _ctx.ExecuteStoreQuery<OutletMainOrderSummary>(sql).ToList();

            if (!string.IsNullOrWhiteSpace((q.Name)))
                query = query.Where(l => l.Distributor.ToLower().Contains(q.Name.ToLower())).ToList();

            result.Count = query.Count;
            result.Data = query.ToList();
            return result;
        }

        public QueryResult<OutletMainOrderSummary> PendingDispatchQuery(QueryOutletOrder q)
        {
            var result = new QueryResult<OutletMainOrderSummary>();
            string sql = string.Format(Resources.OutletOrders.OutletOrdersResource.OutletOrdersPendingDispatch,
                                       q.From.ToString("yyyy-MM-dd hh:mm:ss"), q.To.ToString("yyyy-MM-dd hh:mm:ss"),
                                       q.OutletId);
            var query = _ctx.ExecuteStoreQuery<OutletMainOrderSummary>(sql).ToList();

            if (!string.IsNullOrWhiteSpace((q.Name)))
                query = query.Where(l => l.Distributor.ToLower().Contains(q.Name.ToLower())).ToList();

            result.Count = query.Count;
            result.Data = query.ToList();
            return result;
        }

        public QueryResult<OutletMainOrderSummary> OutstandingPaymentQuery(QueryOutletOrder q)
        {
            var result = new QueryResult<OutletMainOrderSummary>();
            string sql = string.Format(Resources.OutletOrders.OutletOrdersResource.OutstandingOutletOrders,
                                       q.From.ToString("yyyy-MM-dd hh:mm:ss"), q.To.ToString("yyyy-MM-dd hh:mm:ss"),
                                       q.OutletId);
            var query = _ctx.ExecuteStoreQuery<OutletMainOrderSummary>(sql).ToList();

            if (!string.IsNullOrWhiteSpace((q.Name)))
                query = query.Where(l => l.Distributor.ToLower().Contains(q.Name.ToLower())).ToList();

            result.Count = query.Count;
            result.Data = query.ToList();
            return result;
        }

        public QueryResult<OutletMainOrderSummary> RejectedQuery(QueryOutletOrder q)
        {
            var result = new QueryResult<OutletMainOrderSummary>();
            string sql = string.Format(Resources.OutletOrders.OutletOrdersResource.RejectedOutletOrders,
                                       q.From.ToString("yyyy-MM-dd hh:mm:ss"), q.To.ToString("yyyy-MM-dd hh:mm:ss"),
                                       q.OutletId);
            var query = _ctx.ExecuteStoreQuery<OutletMainOrderSummary>(sql).ToList();

            if (!string.IsNullOrWhiteSpace((q.Name)))
                query = query.Where(l => l.Distributor.ToLower().Contains(q.Name.ToLower())).ToList();

            result.Count = query.Count;
            result.Data = query.ToList();
            return result;
        }

        public QueryResult<OutletMainOrderSummary> DispatchedQuery(QueryOutletOrder q)
        {
            var result = new QueryResult<OutletMainOrderSummary>();
            string sql = string.Format(Resources.OutletOrders.OutletOrdersResource.DispatchedOutletOrders,
                                       q.From.ToString("yyyy-MM-dd hh:mm:ss"), q.To.ToString("yyyy-MM-dd hh:mm:ss"),
                                       q.OutletId);
            var query = _ctx.ExecuteStoreQuery<OutletMainOrderSummary>(sql).ToList();

            if (!string.IsNullOrWhiteSpace((q.Name)))
                query = query.Where(l => l.Distributor.ToLower().Contains(q.Name.ToLower())).ToList();

            result.Count = query.Count;
            result.Data = query.ToList();
            return result;
        }
    }
}