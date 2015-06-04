using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distributr.Core.Domain.Transactional.SourcingDocumentEnitities.LineItems
{
    public class CommodityTransferLineItem: SourcingDocumentLineItem
    {
        public CommodityTransferLineItem(Guid id)
            : base(id)
       {
       }
       public Guid ParentDocId { get; set; }
    }
}
