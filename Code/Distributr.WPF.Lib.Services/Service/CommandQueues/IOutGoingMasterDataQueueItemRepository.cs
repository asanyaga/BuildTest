using System;
using System.Collections.Generic;

namespace Distributr.WPF.Lib.Services.Service.CommandQueues
{
    public interface IOutGoingMasterDataQueueItemRepository
    {
        List<OutGoingMasterDataQueueItemLocal> GetUnSentMasterDataDTO();
        void MarkMasterDataDTOAsSent(int OID);
        OutGoingMasterDataQueueItemLocal UpdateSerializedObjectWithOID(int OID);
        OutGoingMasterDataQueueItemLocal GetFirstUnSentMasterDataDTO();
        OutGoingMasterDataQueueItemLocal GetByIOD(int OID);
        
        OutGoingMasterDataQueueItemLocal GetByDTOIDId(Guid dtoid);
        List<OutGoingMasterDataQueueItemLocal> GetAll();
        void Add(OutGoingMasterDataQueueItemLocal item);
       
        void DeleteOldCommand(int OID);
        bool IsAnyUnSent();
      
    }
}
