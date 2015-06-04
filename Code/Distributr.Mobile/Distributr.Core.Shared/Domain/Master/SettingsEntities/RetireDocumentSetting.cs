using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributr.Core.Domain.Master.SettingsEntities
{ 
    public enum RetireType{Paid=1,Delivered=2}
#if !SILVERLIGHT
   [Serializable]
#endif
   public  class RetireDocumentSetting: MasterEntity
    {
       public RetireDocumentSetting(Guid id) : base(id)
       {
       }

       public RetireDocumentSetting(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive)
           : base(id, dateCreated, dateLastUpdated, isActive)
       {
       }

       public RetireType RetireType { get; set; }
       public int Duration { get; set; } 
    }
}
