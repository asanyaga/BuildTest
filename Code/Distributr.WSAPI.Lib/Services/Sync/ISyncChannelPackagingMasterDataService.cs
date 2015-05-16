using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.ChannelPackaging;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;

namespace Distributr.WSAPI.Lib.Services.Sync
{
    public interface ISyncChannelPackagingMasterDataService
    {
        SyncResponseMasterDataInfo<ChannelPackagingDTO> GetChannelPackaging(QueryMasterData myQuery);
    }
}