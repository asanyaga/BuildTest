using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.MasterDataDTO.DTOModels.InventoriesDTO;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.CostCentre;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.WSAPI.Lib.System.Utility;

namespace Distributr.WSAPI.Lib.Services.Sync
{
    public interface ISyncAreaMasterDataService
    {
        SyncResponseMasterDataInfo<AreaDTO> GetArea(QueryMasterData myQuery);
    }
    public interface ISyncInventoryService
    {
        SyncResponseMasterDataInfo<InventoryDTO> GetInventory(QueryMasterData myQuery);
    }
}
