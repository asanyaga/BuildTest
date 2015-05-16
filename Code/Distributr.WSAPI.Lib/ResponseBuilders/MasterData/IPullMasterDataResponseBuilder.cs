using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core;
using Distributr.Core.ClientApp.CommandResults;

namespace Distributr.WSAPI.Lib.ResponseBuilders.MasterData

{
    public interface IPullMasterDataResponseBuilder
    {
        ResponseSyncRequired DoesCostCentreApplicationNeedToSync(Guid costCentreApplicationId, VirtualCityApp vcAppId);
        ResponseCostCentreSyncTables RepositoryList();
        ResponseMasterDataInfo GetTableContents(Guid costCentreApplicationId, string entityName);
        ResponseCostCentreTest GetTestCostCentre();
        ResponseMasterDataInfo GetInventory(Guid costCentreApplicationId);
        ResponseMasterDataInfo GetPayments(Guid costCentreApplicationId);
        ResponseFarmerCummulativeDataInfo FamersCummulative(Guid costCentreApplicationId);
        
    }
}
