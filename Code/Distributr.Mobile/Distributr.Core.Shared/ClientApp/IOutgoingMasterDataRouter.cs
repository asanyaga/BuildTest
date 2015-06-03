using Distributr.Core.MasterDataDTO;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO;

namespace Distributr.Core.Workflow.Impl
{
    public interface IOutgoingMasterDataRouter
    {
        void RouteMasterData(MasterBaseDTO dTO, MasterDataDTOSaveCollective type);
    }
}
