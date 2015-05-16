using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.MasterDataDTO.DTOModels.FinancialDTO;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;

namespace Distributr.WSAPI.Lib.Services.Sync
{
    public interface ISyncUnderBankingService
    {
        SyncResponseMasterDataInfo<UnderBankingDTO> GetUnderBanking(QueryMasterData myQuery);
    }
}