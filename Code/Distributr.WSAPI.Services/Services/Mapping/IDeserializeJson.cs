using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.WSAPI.Lib.CommandResults;

namespace Distributr.WSAPI.Lib.Services.Mapping
{
    public interface IDeserializeJson
    {
        ResponseMasterDataInfo DeserializeResponseMasterDataInfo(string responseMasterDataInfo);
        DocumentCommandRoutingResponse DeserializeDocumentCommandRoutingResponse(string jsonString);
        BatchDocumentCommandRoutingResponse DeserializeBatchDocumentCommandRoutingResponse(string jsonString);
    }
}
