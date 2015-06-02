using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master.CostCentreEntities;

namespace Distributr.Core.Repository.Master.CostCentreRepositories
{
    public interface ITransporterRepository : ICostCentreRepository
    {
        List<Transporter> GetByProducer(Guid producer);
       
      
    }
}
