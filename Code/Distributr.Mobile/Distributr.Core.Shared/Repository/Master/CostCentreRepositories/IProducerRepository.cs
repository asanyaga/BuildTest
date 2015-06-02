using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Domain.Master.CostCentreEntities;


namespace Distributr.Core.Repository.Master.CostCentreRepositories
{
    public interface IProducerRepository:ICostCentreRepository 
    {
       Producer GetProducer();
    }
}
