using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master;
using Distributr.Core.Factory.Master;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Data.Utility.Caching;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Utility;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.MasterData;

namespace Distributr.Core.Data.Repository.MasterData.CostCentreRepositories
{
    internal class DistributorRepository: CostCentreRepository, IDistributorRepository
    {
      
        public DistributorRepository(ICostCentreFactory costCentreFactory, CokeDataContext ctx, ICacheProvider cacheProvider, IUserRepository _userRepository, IContactRepository contactRepository)
            : base(costCentreFactory, ctx, cacheProvider, _userRepository, contactRepository)
        {
         
        }

      

        public List<Distributor> GetByFreetextName(string distributor)
        {
            _log.DebugFormat("Get By Free Text Name {0}", distributor);
            return GetAll().OfType<Distributor>().Where(n=> n.Name.Contains(distributor) ).ToList();
        }   

        public override IEnumerable<CostCentre> GetAll(bool includeDeactivated = false)
        {
            var distributorList = base.GetAll(includeDeactivated).OfType<Distributor>();
            if (!includeDeactivated)
                return distributorList.Where(n => n._Status!=EntityStatus.Inactive).ToList();
            return distributorList.ToList();
        }




        public Distributor GetDistributor()
        {
            var qry = _ctx.tblCostCentre as IQueryable<tblCostCentre>;
            Distributor distributor = qry.Where(n => n.CostCentreType == (int)CostCentreType.Distributor || n.Name == "Ernest Mburu").ToList().Select(n => Map(n)).First() as Distributor;
            return distributor;
        }

        public QueryResult<Distributor> Query(QueryStandard query)
        {
            IQueryable<tblCostCentre> distributorquery;
            if (query.ShowInactive)
                distributorquery = _ctx.tblCostCentre.Where(p => p.IM_Status != (int)EntityStatus.Deleted && p.CostCentreType==(int)CostCentreType.Distributor).AsQueryable();
            else
                distributorquery = _ctx.tblCostCentre.Where(p => p.IM_Status == (int)EntityStatus.Active && p.CostCentreType == (int)CostCentreType.Distributor).AsQueryable();

            var queryResult = new QueryResult<Distributor>();
            if (!string.IsNullOrEmpty(query.Name))
            {
                distributorquery = distributorquery.Where(p => p.Name.ToLower().Contains(query.Name.ToLower())
                                                         || p.Distributor_Owner.ToLower().Contains(query.Name.ToLower())
                                                         || p.tblRegion.Name.ToLower().Contains(query.Name.ToLower()));

            }

            queryResult.Count = distributorquery.Select(Map).OfType<Distributor>().ToList().Count();

            distributorquery = distributorquery.OrderBy(p => p.Name).ThenBy(p => p.Cost_Centre_Code);
            
            if (query.Skip.HasValue && query.Take.HasValue)
                distributorquery = distributorquery.Skip(query.Skip.Value).Take(query.Take.Value);
            
            queryResult.Data = distributorquery.Select(Map).OfType<Distributor>().ToList();
           

            return queryResult;
        }


        public override List<CostCentre> GetByRegionId(Guid regionId, bool includeDeactivated)
        {
            var hubList = GetAll().Where(n => ((Distributor) n).Region.Id == regionId);
            if (!includeDeactivated)
            {
                return hubList.Where(n => n._Status != EntityStatus.Inactive).ToList();
            }
            return hubList.ToList();
        }
    }
}
