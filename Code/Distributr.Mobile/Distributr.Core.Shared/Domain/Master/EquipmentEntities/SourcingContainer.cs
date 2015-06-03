using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distributr.Core.Domain.Master.EquipmentEntities
{
#if !SILVERLIGHT
   [Serializable]
#endif
    public class SourcingContainer : Equipment
    {
        public SourcingContainer(Guid id) : base(id){}

        public SourcingContainer(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive) 
            : base(id, dateCreated, dateLastUpdated, isActive){}
        [Required]
        public ContainerType ContainerType { get; set; }
    }
#if !SILVERLIGHT
   [Serializable]
#endif
   public enum ContainerUseType { Unknown=0, WeighingContainer = 1, StorageContainer = 2 }
}

