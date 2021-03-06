using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.MasterDataDTO.DTOModels.AgrimanagrDTO.FarmActivities;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;

namespace Distributr.WSAPI.Lib.Services.Sync
{
    public interface ISyncActivityTypeMasterDataService
    {
        SyncResponseMasterDataInfo<ActivityTypeDTO> GetActivityType(QueryMasterData myQuery);
    }
}