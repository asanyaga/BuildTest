using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Transactional.Recollections;
using Distributr.Core.Repository.Master;
using Distributr.Core.Repository.Transactional.RecollectionRepositories;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;

namespace Distributr.Core.Data.Repository.Transactional.RecollectionRepositories
{
    public class ReCollectionRepository :IReCollectionRepository
    {
        private CokeDataContext _ctx;

        public ReCollectionRepository(CokeDataContext ctx)
        {
            _ctx = ctx;
        }

        public List<UnderBankingItem> Query(QueryMasterData q)
        {
            IQueryable<tblRecollection> query = _ctx.tblRecollection.AsQueryable();
            query = query.Where(n => n.IM_DateLastUpdated > q.From)
                    .OrderBy(s => s.IM_DateCreated);
            if(q.CostCentreId.HasValue)
            {
                query = query.Where(s => s.CostCentreId == q.CostCentreId.Value);
            }
            if (q.Skip.HasValue && q.Take.HasValue)
                query = query.Skip(q.Skip.Value).Take(q.Take.Value);
            return query.ToList().Select(Map).ToList();
        }

        public List<UnderBankingItemSummary> QuerySummary(QueryUnderBanking q)
        {
            IQueryable<tblRecollection> query = _ctx.tblRecollection.AsQueryable();
            query = query
                    .OrderByDescending(s => s.IM_DateCreated);
            if (q.CostCentreId.HasValue)
            {
                query = query.Where(s => s.FromCostCentreId == q.CostCentreId.Value);
            }
            if (q.SalesmanId.HasValue)
            {
                query = query.Where(s => s.CostCentreId == q.SalesmanId.Value);
            }
            if (q.Skip.HasValue && q.Take.HasValue)
                query = query.Skip(q.Skip.Value).Take(q.Take.Value);
            return query.ToList().Select(MapSummary).ToList();
        }

        public List<UnderBankingItemReceived> UnderBankingItemReceived(Guid id)
        {
            return _ctx.tblRecollectionItem.Where(s => s.RecollectionId == id).OrderByDescending(s=>s.DateInserted).ToList()
                .Select(s => new UnderBankingItemReceived(s.Id)
                                 {
                                     Amount = s.Amount, 
                                     DateReceived = s.DateInserted,
                                     Type= s.CollectionModeId.HasValue?(ReCollectionType)s.CollectionModeId.Value:0,
                                     IsConfirmed = s.IsComfirmed
                                 }).
                ToList();
        }

        private UnderBankingItemSummary MapSummary(tblRecollection s)
        {
            if (s == null) return null;
            var salesman = _ctx.tblCostCentre.FirstOrDefault(n => n.Id == s.CostCentreId);
            var costcentre = _ctx.tblCostCentre.FirstOrDefault(n => n.Id == s.FromCostCentreId);
            var reco = new UnderBankingItemSummary(s.Id)
            {

                CostCentreId = s.FromCostCentreId,
                CostCentreName = costcentre!=null?costcentre.Name:"",
                CostCentreType = costcentre != null ? ((CostCentreType)costcentre.CostCentreType).ToString() : "",
                SalesmanId = s.CostCentreId,
                SalesmanName = salesman != null ? salesman.Name : "",
                Description = s.Description,
                Amount = s.Amount,
                ReceivedAmount = s.tblRecollectionItem.Sum(o => o.Amount),
                ConfirmedAmount = s.tblRecollectionItem.Where(o=>o.IsComfirmed).Sum(o => o.Amount),
                _DateCreated = s.IM_DateCreated,
                _DateLastUpdated = s.IM_DateLastUpdated,
                
            };
            return reco;
        }
        private UnderBankingItem Map(tblRecollection s)
        {
            if (s == null) return null;
            var reco = new UnderBankingItem(s.Id)
                           {
                               
                               FromCostCentreId = s.CostCentreId,
                               Description = s.Description,
                               Amount = s.Amount,
                               _DateCreated = s.IM_DateCreated,
                               _DateLastUpdated = s.IM_DateLastUpdated,
                               TotalReceivedAmount = s.tblRecollectionItem.Sum(o=>o.Amount),
                           };
            return reco;
        }
    }
}
