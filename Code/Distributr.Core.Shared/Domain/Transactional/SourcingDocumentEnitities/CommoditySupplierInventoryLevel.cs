using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Commands.SourcingDocumentCommands.CommodityDeliveryCommands;
using Distributr.Core.Commands.SourcingDocumentCommands.CommodityWarehouseStorageCommands;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.Exceptions;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities.LineItems;

namespace Distributr.Core.Domain.Transactional.SourcingDocumentEnitities
{
  public  class CommoditySupplierInventoryLevel:MasterEntity
  {
       public CommoditySupplierInventoryLevel(Guid id) : base(id)
        {
        }

        public CommoditySupplierInventoryLevel(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus status)
         : base(id, dateCreated, dateLastUpdated, status)
        {
        }

        public Guid Id { get; set; }
        public string Warehouse { get; set; }
        public string CommoditySupplier { get; set; }
        public string Commodity { get; set; }
        public string Grade { get; set; }
        public string Balance { get; set; }

    }
}
