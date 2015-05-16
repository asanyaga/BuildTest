using System.Collections.Generic;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.Product;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.WSAPI.Lib.System.Utility;

namespace Distributr.WSAPI.Lib.Services.Sync
{
  public  interface ISyncPricingMasterDataService
  {
      SyncResponseMasterDataInfo<ProductPricingDTO> GetPricing(QueryMasterData myQuery);
      SyncResponseMasterDataInfo<ProductPricingDTO> GetHubPricing(QueryMasterData myQuery);
  }
}
