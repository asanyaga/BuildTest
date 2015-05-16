using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Data.EF;
using Distributr.Core.Factory.Master;
using Distributr.Core.Data.Utility.Caching;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Utility.Caching;


namespace Distributr.Core.Data.Repository.MasterData.CostCentreRepositories
{
    internal class TransporterRepository : CostCentreRepository, ITransporterRepository
    {
        public TransporterRepository(ICostCentreFactory costCentreFactory, CokeDataContext ctx, ICacheProvider cacheProvider, IUserRepository _userRepository, IContactRepository contactRepository)
            : base(costCentreFactory, ctx, cacheProvider, _userRepository, contactRepository)
        {

        }




        public List<Transporter> GetByProducer(Guid producerId)
        {
            return base.GetByParentId(producerId)
                .OfType<Transporter>()
                .ToList();
        }

       
        
    }
}


