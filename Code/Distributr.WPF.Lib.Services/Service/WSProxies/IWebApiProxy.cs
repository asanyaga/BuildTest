using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Distributr.Core;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Commands.CommandPackage;
using Distributr.Core.Commands.DocumentCommands;
using Distributr.Core.MasterDataDTO;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO;
using Distributr.Core.Notifications;
using Distributr.Import.Entities;

namespace Distributr.WPF.Lib.Services.Service.WSProxies
{
    public interface IWebApiProxy
    {
        Task<bool> DoesCostCenreNeedToSyncAsync(Guid costCentreApplicationId, VirtualCityApp vcAppId);
        Task<string[]> ListMasterDataEntitiesAsync();
        Task<string> GetEntityMasterDataQueryAsync(Guid costCentreApplicationId, string entityName);
        Task<ResponseMasterDataInfo> GetEntityMasterDataAsync(Guid costCentreApplicationId, string entityName);
        Task<ResponseMasterDataInfo> GetEntityMasterDataAsync(Guid costCentreApplicationId, string entityName,int page,int pagesize,DateTime lastsynctimeTimeStamp);
        Task<ResponseMasterDataInfo> GetInventoryAsync(Guid costCentreApplicationId);
        Task<ResponseMasterDataInfo> GetInventoryAsync(Guid costCentreApplicationId, int page, int pagesize);
        Task<ResponseMasterDataInfo> GetUnderBankingAsync(Guid costCentreApplicationId);
        Task<ResponseMasterDataInfo> GetPaymentsAsync(Guid costCentreApplicationId);
        Task<ResponseMasterDataInfo> GetPaymentsAsync(Guid costCentreApplicationId, int page, int pagesize);
        [Obsolete("Command Envelope refactoring")]
        Task<bool> SendCommandAsync(DocumentCommand command);
        Task<bool> SendCommandEnvelope(CommandEnvelope envelope);
        Task<bool> SendNotificationsAsync(NotificationBase notification);
        Task<bool> PingServerAsync();
        Task<bool> InvalidateCacheAsync();

        Task<DocumentCommandRoutingResponse> GetNextDocumentCommandAsync(Guid costCentreApplicationId,
                                                                         long lastDeliveredCommandRouteItemId);
        Task<BatchDocumentCommandRoutingResponse> GetNextBatchDocumentCommandAsync(Guid costCentreApplicationId,
                                                                      long lastDeliveredCommandRouteItemId,
                                                                        int batchSize, string batchIdsJson);
        Task<BatchDocumentCommandEnvelopeRoutingResponse> GetNextCommandEnvelopesAsync(EnvelopeRoutingRequest request);

        Task<Guid> LoginAsync(string userName, string password, string userType);
        Task<CreateCostCentreApplicationResponse> CreateCostCentreApplicationAsync(Guid costCentreId, string applicationDescription);
        Task<bool> PushMasterDataAsync(MasterDataDTOSaveCollective masterDataCollective, MasterBaseDTO jsonDto);
        Task<bool> SendInventoryImport(List<InventoryImport> list );
    }
}
