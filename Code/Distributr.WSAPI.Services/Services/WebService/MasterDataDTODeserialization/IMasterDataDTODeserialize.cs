using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO;

namespace Distributr.WSAPI.Lib.Services.WebService.CommandDeserialization
{
    public interface IMasterDataDTODeserialize
    {
        MasterBaseDTO DeserializeMasterDataDTO(string masterDataCollective, string jsonMasterDataDTO);
    }
}
