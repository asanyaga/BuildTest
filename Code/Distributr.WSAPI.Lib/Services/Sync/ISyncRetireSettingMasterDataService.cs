using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.Retire;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.WSAPI.Lib.System.Utility;

namespace Distributr.WSAPI.Lib.Services.Sync
{
    public interface ISyncRetireSettingMasterDataService
    {
        SyncResponseMasterDataInfo<RetireSettingDTO> GetRetireSetting(QueryMasterData myQuery);
    }
}
