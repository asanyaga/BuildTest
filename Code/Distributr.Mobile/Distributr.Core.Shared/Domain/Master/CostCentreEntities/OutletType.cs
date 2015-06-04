using System;
using System.ComponentModel.DataAnnotations;

namespace Distributr.Core.Domain.Master.CostCentreEntities
{
#if !SILVERLIGHT
   [Serializable]
#endif
    public class OutletType : MasterEntity
    {
        public OutletType() : base(default(Guid)) { }
        public OutletType(Guid id) : base(id)
        {

        }

        public OutletType(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive)
            : base(id, dateCreated, dateLastUpdated, isActive)
        {

        }
        [Required(ErrorMessage="Name is required")]      
        public string Name { get; set; }
        public string Code { get; set; }
    }
}
