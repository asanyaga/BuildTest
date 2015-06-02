using System;

namespace Distributr.Core.Domain.Transactional.SourcingDocumentEnitities.LineItems
{
 public   class CommodityPurchaseLineItem : SourcingDocumentLineItem
    {
     public CommodityPurchaseLineItem(Guid id) : base(id)
     {
     }
     public decimal TareWeight { get; set; }
     public decimal NoOfContainers { get; set; }
    }
}
