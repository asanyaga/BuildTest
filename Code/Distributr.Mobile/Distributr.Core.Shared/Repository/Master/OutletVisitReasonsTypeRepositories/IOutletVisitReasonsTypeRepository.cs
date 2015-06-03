using Distributr.Core.Domain.Master.OutletVisitReasonsTypeEntities;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;

namespace Distributr.Core.Repository.Master.OutletVisitReasonsTypeRepositories
{
    public interface IOutletVisitReasonsTypeRepository : IRepositoryMaster<OutletVisitReasonsType>
    {

        QueryResult<OutletVisitReasonsType> Query(QueryStandard query);
    }
}






