using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities;

namespace Distributr.Core.Repository.Transactional.SourcingDocumentRepositories
{
   public interface ICommodityPurchaseRepository:ISourcingDocumentRepository
   {
       CommodityPurchaseNote GetLastTransactionByFarmerId(Guid farmerId);
   }
}
