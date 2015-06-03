using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Master.CommodityEntities;
using Distributr.Core.Domain.Master.EquipmentEntities;

namespace Distributr.Core.Domain.Transactional.SourcingDocumentEnitities.LineItems
{
    public class ReceivedDeliveryLineItem : SourcingDocumentLineItem
    {
       public ReceivedDeliveryLineItem(Guid id)
            : base(id)
     {
     }
      
      
      
       public decimal DeliveredWeight { get; set; }
       
       public Guid ParentDocId { get; set; }
       
      
    }
}
