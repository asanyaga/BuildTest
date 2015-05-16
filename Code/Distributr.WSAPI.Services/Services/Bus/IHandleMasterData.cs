﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master;
using Distributr.Core.MasterDataDTO;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO;

namespace Distributr.WSAPI.Lib.Services.Bus
{
    public interface IHandleMasterData
    {
        void HandleMasterDataDTO(MasterBaseDTO masterBaseDTO, MasterDataDTOSaveCollective collective);
    }
}
