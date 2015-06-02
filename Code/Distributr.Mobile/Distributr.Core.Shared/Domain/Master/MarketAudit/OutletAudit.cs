using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributr.Core.Domain.Master.MarketAudit
{
#if !SILVERLIGHT
   [Serializable]
#endif
    public class OutletAudit : MasterEntity
    {
        public OutletAudit(Guid id) : base(id) { }
        public string Question { get; set; }
    }
}
