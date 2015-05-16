using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributr.WSAPI.Lib.Services.WebService.MasterDataDTODeserialization
{
   public interface IMasterDataDTOValidation
    {
       bool CanDeserializeMasterDataDTO<T>(string jsonMasterDataDTO, out T deserializedObject);
    }
}
