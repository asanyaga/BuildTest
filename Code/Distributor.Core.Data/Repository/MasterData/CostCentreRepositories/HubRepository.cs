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
    internal class HubRepository : CostCentreRepository, IHubRepository
    {
        public HubRepository(ICostCentreFactory costCentreFactory, CokeDataContext ctx, ICacheProvider cacheProvider, IUserRepository userRepository, IContactRepository contactRepository)
            : base(costCentreFactory, ctx, cacheProvider, userRepository, contactRepository)
        {
        }

        public override IEnumerable<CostCentre> GetAll(bool includeDeactivated = false)
        {
            var hubList = base.GetAll(includeDeactivated).OfType<Hub>().Where(n => n.CostCentreType == CostCentreType.Hub);
            if (!includeDeactivated)
            {
                return hubList.Where(n => n._Status != EntityStatus.Inactive).ToList();
            }
            return hubList.ToList();
        }


        public override List<CostCentre> GetByRegionId(Guid regionId, bool includeDeactivated)
        {
            var hub =
                _ctx.tblCostCentre.Where(
                    n =>
                    n.tblRegion.id == regionId && n.CostCentreType == (int)CostCentreType.Hub &&
                    n.IM_Status == (int)EntityStatus.Active);
            if (includeDeactivated)
                hub =
                _ctx.tblCostCentre.Where(
                    n =>
                    n.tblRegion.id == regionId && n.CostCentreType == (int)CostCentreType.Hub &&
                    (n.IM_Status == (int)EntityStatus.Active || n.IM_Status == (int)EntityStatus.Inactive));

            var retVal = hub.Select(base.Map).ToList();
            return retVal;
        }

        

        public QueryResult<Hub> Query(QueryStandard query)
        {

            IQueryable<tblCostCentre> costCentreQuery;

            //retreave inactive costCentres 
            if (query.ShowInactive)
                costCentreQuery = _ctx.tblCostCentre.Where(
                    p => p.IM_Status != (int)EntityStatus.Deleted && p.CostCentreType==(int)CostCentreType.Hub).AsQueryable();
            else
                //retreave active costCentres 
                costCentreQuery = _ctx.tblCostCentre.Where(
                    p => p.IM_Status == (int)EntityStatus.Active && p.CostCentreType == (int)CostCentreType.Hub).AsQueryable();


            var queryResult = new QueryResult<Hub>();

            //excecute search filter
            if (!string.IsNullOrEmpty(query.Name))
            {
                costCentreQuery = costCentreQuery.Where(p => p.Name.ToLower()
                                                                 .Contains(query.Name.ToLower()) ||
                                                             p.Cost_Centre_Code.ToLower()
                                                                 .Contains(query.Name.ToLower()) ||
                                                             p.tblRegion.Name.ToLower()
                                                                 .Contains(query.Name.ToLower()));
            }
          

            //order costCentreQuery
            costCentreQuery = costCentreQuery.OrderBy(p => p.Name).
                ThenBy(p => p.Cost_Centre_Code).
                ThenBy(p => p.tblRegion.Name);


            

            //paging
            if (query.Skip.HasValue && query.Take.HasValue)
            
                costCentreQuery = costCentreQuery.Skip(query.Skip.Value).Take(query.Take.Value);

            queryResult.Count = costCentreQuery.Count();
            queryResult.Data = costCentreQuery.Select(Map).OfType<Hub>().ToList();

            return queryResult;
        }
    }
}
