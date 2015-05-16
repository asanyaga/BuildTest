using System;
using System.ComponentModel.DataAnnotations;

namespace Distributr.Core.Domain.Master.CostCentreEntities
{
#if !SILVERLIGHT
   [Serializable]
#endif
   public class SocioEconomicStatus : MasterEntity
    {
        public SocioEconomicStatus() : base(default(Guid)) { }
        public SocioEconomicStatus(Guid id)
            : base(id)
        {
            
        }
        public SocioEconomicStatus(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive)
            : base(id, dateCreated, dateLastUpdated, isActive)
        {

        }

        [Required(ErrorMessage="Status is Required")]
        public string EcoStatus { get; set; }
    }
}
