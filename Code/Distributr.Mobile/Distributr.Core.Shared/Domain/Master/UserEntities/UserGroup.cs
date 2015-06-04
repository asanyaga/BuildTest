using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Distributr.Core.Domain.Master.UserEntities
{
#if !SILVERLIGHT
   [Serializable]
#endif
    public class UserGroup : MasterEntity
    {
        public UserGroup(Guid id) : base(id)
        {

        }

        public UserGroup(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive)
            : base(id, dateCreated, dateLastUpdated, isActive)
        {
        }
        public string Name { get; set; }
        public string Descripition { get; set; }
    }
}
