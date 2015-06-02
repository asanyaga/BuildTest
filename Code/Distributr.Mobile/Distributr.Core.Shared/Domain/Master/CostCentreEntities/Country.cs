using System;
using System.ComponentModel.DataAnnotations;

namespace Distributr.Core.Domain.Master.CostCentreEntities
{
#if !SILVERLIGHT
   [Serializable]
#endif
    public class Country : MasterEntity
    {
       public Country() : base(default(Guid)) { }
       //sdfsdf
        public Country(Guid id) : base(id)
        {
            
        }
        public Country(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive)
            : base(id, dateCreated, dateLastUpdated, isActive)
        {

        }

        [Required(ErrorMessage="Country name is Required")]
        public string Name { get; set; }
        public string Currency { get; set; }
        public string Code { get; set; }
    }
}
