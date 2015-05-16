using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities.LineItems;

namespace Distributr.Core.Repository.Transactional.SourcingDocumentRepositories
{
   public interface ICommodityDeliveryRepository:ISourcingDocumentRepository
   {
       List<CommodityDeliveryNote> GetAllByStatus(DocumentSourcingStatus status);

   }
}
