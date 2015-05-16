using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Distributr.WSAPI.Lib.Services.WebService.MasterDataDTODeserialization
{
   public class MasterDataDTOValidation : IMasterDataDTOValidation
    {
        public bool CanDeserializeMasterDataDTO<T>(string jsonMasterDataDTO, out T deserializedObject)
        {
            deserializedObject = default(T);
            try
            {
                deserializedObject = JsonConvert.DeserializeObject<T>(jsonMasterDataDTO, new IsoDateTimeConverter());
            }
            catch (Exception ex)
            {
                
            }
            return deserializedObject != null;
        }
    }
}
