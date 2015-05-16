using System.Diagnostics;
using Distributr.WSAPI.Lib.Services.Bus;
using Distributr.WSAPI.Lib.Services.CommandAudit;
using Distributr.WSAPI.Lib.Services.CostCentreApplications;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Distributr.Azure.WebApi.Controllers
{
    public class ValuesController : ApiController
    {
      
        IBusPublisher _busPublisher;
        private ICommandProcessingAuditRepository _commandProcessingAuditRepository;

        public ValuesController(IBusPublisher busPublisher, ICommandProcessingAuditRepository commandProcessingAuditRepository)
        {
            _busPublisher = busPublisher;
            _commandProcessingAuditRepository = commandProcessingAuditRepository;
        }

        private ILog _log = LogManager.GetLogger("Values");
        // GET api/values
        public IEnumerable<string> Get()
        {
            Trace.TraceInformation("Trace test...............");
            _log.Info("Get hit.................###############################");
            return new string[] { "value1", "value2", "value3"  };
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
