using System;

namespace Distributr.Core.Domain.Transactional.SourcingDocumentEnitities.LineItems
{
    public class CommodityWarehouseStorageLineItem : SourcingDocumentLineItem
    {
        private CommodityWarehouseStorageLineItem(Guid id)
            : base(id)
        {
        }
        public decimal NoOfContainers { get; set; }
        public decimal FinalWeight { get; set; }
    }
}