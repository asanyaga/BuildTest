using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master;
using Distributr.Core.MasterDataDTO;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO;
using Distributr.Core.Repository.Master.CostCentreRepositories;

namespace Distributr.WSAPI.Lib.Services.Bus.Impl
{
    public class PublishMasterData : IPublishMasterData
    {
        private IHandleMasterData _handleMasterData;
       

        public PublishMasterData(IHandleMasterData handleMasterData)
        {
            _handleMasterData = handleMasterData;
        }

        public void Publish(MasterBaseDTO masterBase, MasterDataDTOSaveCollective collective)
        {
            _handleMasterData.HandleMasterDataDTO(masterBase, collective);
        }
    }
}
