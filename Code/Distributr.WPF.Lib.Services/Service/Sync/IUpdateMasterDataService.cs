using System;
using System.Threading.Tasks;
using Distributr.Core;

namespace Distributr.WPF.Lib.Services.Service.Sync
{
    public interface IUpdateMasterDataService
    {
        Task<bool> DoesCostCenreNeedToSyncAsync(Guid costCentreApplicationId, VirtualCityApp vcAppId);
        Task<string[]> ListMasterDataEntitiesAsync();
        Task<bool> GetAndUpdateEntityMasterDataAsync(Guid costCentreApplicationid, string entityName);
        Task<bool> GetByBatchAndUpdateEntityMasterDataAsync(Guid costCentreApplicationid, string entityName);
        Task<bool> GetAndUpdateInventoryAsync(Guid costCentreApplicationId);
        Task<bool> GetAndUpdateUnderBankingAsync(Guid costCentreApplicationId);
        Task<bool> GetAndUpdatePaymentAsync(Guid costCentreApplicationId);
       

    }
}
