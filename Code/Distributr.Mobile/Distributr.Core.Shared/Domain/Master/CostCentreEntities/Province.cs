using System;
using System.ComponentModel.DataAnnotations;
#if __MOBILE__
using SQLiteNetExtensions.Attributes;
#endif
namespace Distributr.Core.Domain.Master.CostCentreEntities
{
#if !SILVERLIGHT
   [Serializable]
#endif
  public class Province:MasterEntity 
    {
       public Province() : base(default(Guid)) { }
      public Province(Guid id) : base(id)
        {
            
        }
      public Province(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive)
            : base(id, dateCreated, dateLastUpdated, isActive)
        {

        }

        [Required(ErrorMessage = "Province name is a required field")]
        public string Name { get; set; }

        public string Description { get; set; }
       [Required(ErrorMessage = "Country is a required field")]
          #if __MOBILE__
         [OneToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
          #endif
        public Country Country { get; set; }
    }
}
