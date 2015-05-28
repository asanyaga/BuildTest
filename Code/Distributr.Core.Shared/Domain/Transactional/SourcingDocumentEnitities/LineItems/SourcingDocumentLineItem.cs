using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Master.CommodityEntities;
using Distributr.Core.Domain.Master.EquipmentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;

namespace Distributr.Core.Domain.Transactional.SourcingDocumentEnitities.LineItems
{
    public class SourcingDocumentLineItem : TransactionalEntity
    {
        public SourcingDocumentLineItem(Guid id)
            : base(id)
        {
        }

        public Guid ParentLineItemId { get; set; }
        public Commodity Commodity { get; set; }
        public CommodityGrade CommodityGrade { get; set; }
        public decimal Weight { get; set; }

        public int WeighType { get; set; }
        public string ContainerNo { get; set; }
        public ContainerType ContainerType { get; set; }
        public string Note { get; set; }
        public string Description { get; set; }
        public bool IsSelected { get; set; }
        public SourcingLineItemStatus LineItemStatus { get; set; }

    }

    public enum SourcingLineItemStatus
    {
        New = 0,
        Confirmed = 1,
        Received = 2,
        Stored = 3,
        Weighed = 4,
        Transfered = 5,
        ReceiptGenerated = 6
    }
}
