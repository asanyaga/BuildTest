using System;
using System.ComponentModel.DataAnnotations;

namespace Distributr.Core.Domain.Master.CoolerEntities
{
#if !SILVERLIGHT
   [Serializable]
#endif
   public class AssetType:MasterEntity 
    {
       public AssetType() : base(default(Guid)) { }
       public AssetType(Guid id) : base(id) { }
       public AssetType(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive)
           : base(id, dateCreated, dateLastUpdated, isActive){}
       [Required(ErrorMessage="Name is a Required Field!")]
       public string Name { get; set; }
        [Required(ErrorMessage = "Code is a Required Field!")]
       public string Description { get; set; }
    }
}
