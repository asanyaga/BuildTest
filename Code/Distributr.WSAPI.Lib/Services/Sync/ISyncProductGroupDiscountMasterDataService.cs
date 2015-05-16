using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.Product;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.WSAPI.Lib.System.Utility;

namespace Distributr.WSAPI.Lib.Services.Sync
{
    public interface ISyncProductGroupDiscountMasterDataService
    {
        SyncResponseMasterDataInfo<ProductGroupDiscountDTO> GetProductGroupDiscount(QueryMasterData myQuery);
        SyncResponseMasterDataInfo<ProductGroupDiscountDTO> GetHubProductGroupDiscount(QueryMasterData myQuery);

        
    }
}
