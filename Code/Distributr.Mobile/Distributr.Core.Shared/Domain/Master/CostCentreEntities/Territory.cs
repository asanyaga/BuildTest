using System;
using System.ComponentModel.DataAnnotations;
namespace Distributr.Core.Domain.Master.CostCentreEntities
{
#if !SILVERLIGHT
   [Serializable]
#endif
    public class Territory : MasterEntity
    {
        public Territory() : base(default(Guid)) { }
        public Territory(Guid id) : base(id)
        {
            
        }
        public Territory(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive)
            : base(id, dateCreated, dateLastUpdated, isActive)
        {

        }

        [Required(ErrorMessage = "Territory name is a required field")]
        public string Name { get; set; }



    }

}
