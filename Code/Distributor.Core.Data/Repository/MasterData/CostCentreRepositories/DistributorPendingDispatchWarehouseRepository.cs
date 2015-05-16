using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Data.EF;
using Distributr.Core.Data.Utility.Caching;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Factory.Master;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Utility.Caching;

namespace Distributr.Core.Data.Repository.MasterData.CostCentreRepositories
{
    internal class DistributorPendingDispatchWarehouseRepository : CostCentreRepository, IDistributorPendingDispatchWarehouseRepository
    {
        public DistributorPendingDispatchWarehouseRepository(ICostCentreFactory costCentreFactory, CokeDataContext ctx, ICacheProvider cacheProvider, IUserRepository userRepository, IContactRepository contactRepository)
            :base(costCentreFactory, ctx, cacheProvider,userRepository, contactRepository)
        {
            
        }


        public override IEnumerable<CostCentre> GetAll(bool includeDeactivated = false)
        {
            return base.GetAll().OfType<DistributorPendingDispatchWarehouse>();
        }
    }
}
