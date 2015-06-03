using System;
using Distributr.Core.Domain.Master.ProductEntities;

namespace Distributr.Core.Domain.Transactional.ActivityDocumentEntities
{
    public class ActivityInputItem : TransactionalEntity
    {
        private ActivityInputItem(Guid id)
            : base(id)
        {
        }

        public Product Product { get; set; }
        public decimal Quantity { get; set; }
        public DateTime? ManufacturedDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string Description { get; set; }
        public string SerialNo { get; set; }

    }
}