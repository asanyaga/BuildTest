﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.CostCentre;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;

namespace Distributr.WSAPI.Lib.Services.Sync
{
    public interface ISyncOutletVisitReasonsTypeMasterDataService
    {
        SyncResponseMasterDataInfo<OutletVisitReasonTypeDTO> GetOutletVisitReasonsType(QueryMasterData myQuery);
    }
}
