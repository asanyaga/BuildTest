using System;
using System.Collections.Generic;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities;

namespace Distributr.Core.Repository.Transactional.SourcingDocumentRepositories
{
    public interface IReceivedDeliveryRepository : ISourcingDocumentRepository
    {
        List<ReceivedDeliveryNote> GetPendingStorage();
    }
}
