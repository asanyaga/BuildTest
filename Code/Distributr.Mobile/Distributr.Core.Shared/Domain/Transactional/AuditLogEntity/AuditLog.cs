using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.UserEntities;

namespace Distributr.Core.Domain.Transactional.AuditLogEntity
{
#if !SILVERLIGHT
   [Serializable]
#endif
    public class AuditLog : TransactionalEntity
    {
        public AuditLog(Guid id)
            : base(id)
        {
        }

        public AuditLog(Guid id, CostCentre actionOwner, User actionUser, string module, string action, DateTime actionTimeStamp)
            :base(id)
        {
            ActionOwner = actionOwner;
            ActionUser = actionUser;
            Module = module;
            Action = action;
            ActionTimeStamp = actionTimeStamp;
        }

        public CostCentre ActionOwner { get; set; }
        public User ActionUser { get; set; }
        public string Module { get; set; }
        public string Action { get; set; }
        public DateTime ActionTimeStamp { get; set; }
    }
}
