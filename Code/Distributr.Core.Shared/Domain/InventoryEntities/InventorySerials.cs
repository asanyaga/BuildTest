using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.ProductEntities;

namespace Distributr.Core.Domain.InventoryEntities
{
    
#if !SILVERLIGHT
   [Serializable]
#endif
    public class InventorySerials : MasterEntity
    {
        public InventorySerials(Guid id) : base(id)
        {
        }

        public InventorySerials(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus status) : base(id, dateCreated, dateLastUpdated, status)
        {
        }

        public string From { get; set; }

        public string To { get; set; }

        public ProductRef ProductRef { get; set; }

        public CostCentreRef CostCentreRef { get; set; }

        public Guid DocumentId { get; set; }
    }
}
