using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


using Distributr.WSAPI.Lib.System.Utility;
using Distributr.WSAPI.Lib.System.Utility.CCAudit;
using log4net;

namespace Distributr.WSAPI.Controllers
{
    public abstract class BaseController : Controller
    {
        ILog _log = LogManager.GetLogger("CommandController");
        protected void AuditCCHit(Guid costCentreId, string action, string Info, string result)
        {
            try
            {
                ICCAuditRepository audit = Using<ICCAuditRepository>();
                var ai = new CCAuditItem
                             {
                                 Action = action,
                                 CostCentreId = costCentreId,
                                 Info = Info,
                                 DateInsert = DateTime.Now,
                                 Result = result
                             };
                audit.Add(ai);
            }
            catch(Exception ex)
            {
                _log.Error("Failed to log audit item(Mongo) " ,ex );
            }
        }


        protected T Using<T>() where T : class
        {
            return DependencyResolver.Current.GetService<T>();
        }
    }
}
