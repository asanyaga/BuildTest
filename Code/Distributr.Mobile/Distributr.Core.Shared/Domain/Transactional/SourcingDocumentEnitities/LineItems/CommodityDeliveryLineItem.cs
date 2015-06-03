using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distributr.Core.Domain.Transactional.SourcingDocumentEnitities.LineItems
{
   public class CommodityDeliveryLineItem: SourcingDocumentLineItem
    {
       private CommodityDeliveryLineItem(Guid id)
            : base(id)
     {
     }
       public decimal NoOfContainers { get; set; }
    }
}
