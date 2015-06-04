using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Master;

namespace Distributr.Core.Domain.InventoryEntities
{
    public class InventoryTransaction : MasterEntity
    {
        public InventoryTransaction(Guid id)
            : base(id) { }

        public InventoryTransaction(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive)
            : base(id, dateCreated, dateLastUpdated, isActive) { }

        public Inventory Inventory { get; set; }
        public decimal NoItems { get; set; }
        public decimal NetValue { get; set; }
        public decimal GrossValue { get; set; }
        public DocumentType DocumentType { get; set; }
        public Guid DocumentId { get; set; }
        public DateTime DateInserted { get; set; }
        public Guid CostCentreId { get; set; }

    }
}
