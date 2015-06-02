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
    public class Printer : Equipment
    {
        public Printer(Guid id) : base(id){}

        public Printer(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive) 
            : base(id, dateCreated, dateLastUpdated, isActive){}
    }
}
