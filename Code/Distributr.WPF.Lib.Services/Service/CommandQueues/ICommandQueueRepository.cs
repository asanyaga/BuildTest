using System;
using System.Collections.Generic;

namespace Distributr.WPF.Lib.Services.Service.CommandQueues
{
    public interface ICommandQueueRepository<T> where T : CommandQueueItemLocal
    {
        T GetByIOD(int OID);
        T GetByCommandId(Guid commandId);
        List<T> GetByDocumentId(Guid documentId);
        void Add(T itemToAdd);
        void DropType();
        List<T> GetAll();
       //void DeleteItem(T itemToDelete);
        
    }
}
