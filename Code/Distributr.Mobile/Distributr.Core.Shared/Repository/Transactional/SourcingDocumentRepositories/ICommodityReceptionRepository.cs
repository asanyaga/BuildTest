using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities.LineItems;

namespace Distributr.Core.Repository.Transactional.SourcingDocumentRepositories
{
   public interface ICommodityReceptionRepository:ISourcingDocumentRepository
   {
       List<CommodityReceptionNote> GetPendingStorage();
       int GetPendingStorageCount();
       bool IsLineItemStored(CommodityReceptionLineItem lineItem);
   }
}
