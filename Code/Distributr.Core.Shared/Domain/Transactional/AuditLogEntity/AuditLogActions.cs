using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributr.Core.Domain.Transactional.AuditLogEntity
{
#if !SILVERLIGHT
   [Serializable]
#endif
   public enum AuditLogActions
    {
       Edit=1,
       Create=2,
       Import=3,
      Deactivate=4,
    }
}
