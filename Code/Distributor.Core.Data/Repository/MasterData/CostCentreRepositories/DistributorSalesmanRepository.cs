using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Factory.Master;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Data.Utility.Caching;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Utility.Caching;

namespace Distributr.Core.Data.Repository.MasterData.CostCentreRepositories
{
    internal class DistributorSalesmanRepository : CostCentreRepository, IDistributorSalesmanRepository
    {
        public DistributorSalesmanRepository(ICostCentreFactory costCentreFactory, CokeDataContext ctx, ICacheProvider cacheProvider, IUserRepository _userRepository, IContactRepository contactRepository)
            : base(costCentreFactory, ctx, cacheProvider, _userRepository, contactRepository)
        {
        }

        public override IEnumerable<CostCentre> GetAll(bool includeDeactivated = false)
        {
            var all= base.GetAll(includeDeactivated).ToList();
            if (all.Any())
                return all.OfType<DistributorSalesman>();
            return null;
        }


        public List<DistributorSalesman> GetByDistributor(Guid Distributorid)
        {
            return base.GetByParentId(Distributorid)
                .OfType<DistributorSalesman>()
                .ToList();
        }

        

    }
}
