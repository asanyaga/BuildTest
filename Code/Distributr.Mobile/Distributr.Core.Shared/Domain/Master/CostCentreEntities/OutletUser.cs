using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distributr.Core.Domain.Master.CostCentreEntities
{
    public class OutletUser : MasterEntity
    {
        public OutletUser(Guid id) : base(id){}
            public string Username{get; set; }
            public string Password { get; set; }
            public string UserId { get; set; }
    }
}
