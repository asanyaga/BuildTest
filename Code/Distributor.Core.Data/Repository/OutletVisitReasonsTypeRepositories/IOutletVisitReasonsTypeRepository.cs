
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Master.OutletVisitReasonsTypeEntities;
using Distributr.Core.Repository;
using Distributr.Core.Utility;


namespace Distributr.Core.Data.Repository.OutletVisitReasonsTypeRepositories
{
    public interface IOutletVisitReasonsTypeRepository : IRepositoryMaster<OutletVisitReasonsType>
    {

        QueryResult<OutletVisitReasonsType> Query(QueryStandard query);
    }
}






