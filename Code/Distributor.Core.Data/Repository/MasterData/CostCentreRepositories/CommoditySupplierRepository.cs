using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Data.EF;
using Distributr.Core.Data.Utility.Caching;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Factory.Master;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Utility;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.MasterData;

namespace Distributr.Core.Data.Repository.MasterData.CostCentreRepositories
{
    internal class CommoditySupplierRepository : CostCentreRepository, ICommoditySupplierRepository
    {
        public CommoditySupplierRepository(ICostCentreFactory costCentreFactory, CokeDataContext ctx, ICacheProvider cacheProvider, IUserRepository userRepository, IContactRepository contactRepository) : base(costCentreFactory, ctx, cacheProvider, userRepository, contactRepository)
        {
        }

        public override IEnumerable<CostCentre> GetAll(bool includeDeactivated = false)
        {
            var commoditySuppliers = base.GetAll(includeDeactivated).OfType<CommoditySupplier>();
            if (!includeDeactivated)
                return commoditySuppliers.Where(n => n._Status != EntityStatus.Inactive).ToList();
            return commoditySuppliers.ToList();
        }

        public decimal? GetCummulativeWeight(Guid supplierId, Guid commodityId)
        {
            var items=    
                _ctx.tblSourcingLineItem.Where(
                    s =>
                    s.tblSourcingDocument.DocumentOnBehalfOfCostCentreId == supplierId);
            if(commodityId !=Guid.Empty)
               items= items.Where(p=>p.CommodityId==commodityId);

            return items.Sum(s => s.Weight);
        }

        public new QueryResult<CommoditySupplier> Query(QueryBase query)
        {
            var  q= query as QueryCommoditySupplier;

            IQueryable<tblCostCentre> locationQuery;
            if(q.ShowInactive)
               locationQuery = _ctx.tblCostCentre.Where(s=>s.CostCentreType==(int)CostCentreType.CommoditySupplier && (s.IM_Status == (int)EntityStatus.Active || s.IM_Status==(int)EntityStatus.Inactive)).AsQueryable();
            else
                locationQuery = _ctx.tblCostCentre.Where(s => s.CostCentreType == (int)CostCentreType.CommoditySupplier && s.IM_Status == (int)EntityStatus.Active).AsQueryable();

            var queryResult = new QueryResult<CommoditySupplier>();
            if (!string.IsNullOrWhiteSpace(q.Name))
            {
                locationQuery = locationQuery
                    .Where(s => s.Name.ToLower().Contains(q.Name.ToLower()) || s.Cost_Centre_Code.ToLower().Contains(q.Name.ToLower()));
            }
            
            queryResult.Count = locationQuery.Count();
            locationQuery = locationQuery.OrderBy(s => s.Name).ThenBy(s=>s.Cost_Centre_Code);
            if (q.Skip.HasValue && q.Take.HasValue)
                locationQuery = locationQuery.Skip(q.Skip.Value).Take(q.Take.Value);
            var result = locationQuery.ToList();
            queryResult.Data = result.Select(Map).OfType<CommoditySupplier>().ToList();
            q.ShowInactive = false;
            return queryResult;
        }
    }
}
