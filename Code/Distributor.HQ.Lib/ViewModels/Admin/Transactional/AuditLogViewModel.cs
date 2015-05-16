using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Distributr.HQ.Lib.ViewModels.Admin.Transactional
{
    public class AuditLogViewModel
    {
       
            public Guid id { get; set; }
            public Guid CostCentreId { get; set; }
            public string CostCentreName { get; set; }
            public Guid UserId { get; set; }
            public string UserName { get; set; }
            public string moduleName { get; set; }
            public string actionName { get; set; }
            public DateTime timeStamp { get; set; }
        
        }
    }
