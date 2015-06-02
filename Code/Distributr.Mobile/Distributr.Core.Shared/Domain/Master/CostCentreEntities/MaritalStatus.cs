using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributr.Core.Domain.Master.CostCentreEntities
{
#if !SILVERLIGHT
   [Serializable]
#endif
   public class MaritalStatus:MasterEntity
    {
       public MaritalStatus(Guid id):base(id)
       {

       }
       public MaritalStatus(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive)
           :base(id,dateCreated,dateLastUpdated,isActive)
       {

       }
       public string Code { get; set; }
       public string MStatus { get; set; }
       public string Description { get; set; }
    }
}
