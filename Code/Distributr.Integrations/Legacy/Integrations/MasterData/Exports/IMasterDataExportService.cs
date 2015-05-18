using Distributr.Core.Utility.MasterData;

namespace Distributr.Integrations.Legacy.Integrations.MasterData.Exports
{
    public interface IMasterDataExportService
    {
        MasterdataExportResponse GetResponse(ThirdPartyMasterDataQuery query);
    }
}
