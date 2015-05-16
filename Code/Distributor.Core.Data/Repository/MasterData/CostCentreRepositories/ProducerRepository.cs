using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Factory.Master;
using Distributr.Core.Data.EF;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Data.Utility.Caching;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Utility.Caching;


namespace Distributr.Core.Data.Repository.MasterData.CostCentreRepositories
{
   internal  class ProducerRepository: CostCentreRepository, IProducerRepository 
    {
       // protected static readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        //CokeDataContext _ctx;

        public ProducerRepository(ICostCentreFactory costCentreFactory, CokeDataContext ctx, ICacheProvider cacheProvider, IUserRepository _userRepository, IContactRepository contactRepository)
            : base(costCentreFactory, ctx, cacheProvider, _userRepository, contactRepository)
        {
           // _ctx = ctx;
        }


       public Producer GetProducer()
       {
           var qry = _ctx.tblCostCentre as IQueryable<tblCostCentre>;
           Producer producer = qry.Where(n => n.CostCentreType == (int)CostCentreType.Producer).ToList().Select(n => Map(n)).First() as Producer ;
           return producer;
       }

       


    }
}
