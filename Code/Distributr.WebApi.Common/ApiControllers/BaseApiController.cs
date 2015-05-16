using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using Distributr.WSAPI.Lib.System.Utility;
using Distributr.WSAPI.Lib.System.Utility.CCAudit;
using log4net;

namespace Distributr.WebApi.ApiControllers
{
    public abstract class BaseApiController : ApiController
    {
        ILog _log = LogManager.GetLogger("ApiController");
        protected void AuditCCHit(Guid costCentreId, string action, string Info, string result)
        {
            //Azure return to exclude mongo
            

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
            catch (Exception ex)
            {
                _log.Error("Failed to log audit item(Mongo) ", ex);
            }
        }


        protected T Using<T>() where T : class
        {
            return DependencyResolver.Current.GetService<T>();
        }
    }
}
