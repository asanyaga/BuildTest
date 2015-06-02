using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distributr.Core.Domain.Master.EquipmentEntities
{
#if !SILVERLIGHT
   [Serializable]
#endif
   public class Vehicle:Equipment
    {
       public Vehicle(Guid id) : base(id)
       {
       }

       public Vehicle(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive) : base(id, dateCreated, dateLastUpdated, isActive)
       {
       }
    }
}
