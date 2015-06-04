using Distributr.Core.Domain.Master.FarmActivities;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;

namespace Distributr.Core.Repository.Master.Agrimanagr
{
    public interface IActivityTypeRepository : IRepositoryMaster<ActivityType>
    {
        QueryResult<ActivityType> Query(QueryBase query);
    }
}