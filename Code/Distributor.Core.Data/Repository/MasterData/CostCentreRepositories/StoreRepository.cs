using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Data.EF;
using Distributr.Core.Data.Repository.MasterData.CostCentreRepositories;
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
    internal class StoreRepository : CostCentreRepository, IStoreRepository
    {
        public StoreRepository(ICostCentreFactory costCentreFactory, CokeDataContext ctx, ICacheProvider cacheProvider, IUserRepository userRepository, IContactRepository contactRepository)
            : base(costCentreFactory, ctx, cacheProvider, userRepository, contactRepository)
        {
        }

        public override IEnumerable<CostCentre> GetAll(bool includeDeactivated = false)
        {
            var storeList = base.GetAll(includeDeactivated).OfType<Store>();
            if (!includeDeactivated)
                return storeList.Where(n => n._Status != EntityStatus.Inactive).ToList();
            return storeList.ToList();
        }

        public QueryResult<Store> Query(QueryStandard query)
        {
            var q = query as QueryStandard;

            IQueryable<tblCostCentre> locationQuery;
            if (q.ShowInactive)
                locationQuery = _ctx.tblCostCentre.Where(s => s.IM_Status != (int)EntityStatus.Deleted && s.CostCentreType == (int)CostCentreType.Store).AsQueryable();
            else
                locationQuery = _ctx.tblCostCentre.Where(s => s.CostCentreType == (int)CostCentreType.Store && s.IM_Status == (int)EntityStatus.Active).AsQueryable();

            var queryResult = new QueryResult<Store>();

            if (!string.IsNullOrWhiteSpace(q.Name))
            {
                locationQuery = locationQuery
                    .Where(s => s.Name.ToLower().Contains(q.Name.ToLower()) || s.Cost_Centre_Code.ToLower().Contains(q.Name.ToLower()));
            }

            locationQuery = locationQuery.OrderBy(s => s.Name).ThenBy(s => s.Cost_Centre_Code);
            queryResult.Count = locationQuery.Count();
            if (q.Skip.HasValue && q.Take.HasValue)
                locationQuery = locationQuery.Skip(q.Skip.Value).Take(q.Take.Value);
                queryResult.Data = locationQuery.Select(Map).OfType<Store>().ToList();
            return queryResult;
        }
    }
}
