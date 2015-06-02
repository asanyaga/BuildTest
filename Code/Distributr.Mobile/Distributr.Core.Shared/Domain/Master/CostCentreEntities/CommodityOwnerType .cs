using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Distributr.Core.Domain.Master.CostCentreEntities
{
#if !SILVERLIGHT
   [Serializable]
#endif
    public class CommodityOwnerType : MasterEntity
    {
        public CommodityOwnerType(Guid id) : base(id) { }
        public CommodityOwnerType(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive)
           : base(id, dateCreated, dateLastUpdated, isActive){}
        
        
        [Required(ErrorMessage = "Name is a Required Field!")]
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
    }
}
