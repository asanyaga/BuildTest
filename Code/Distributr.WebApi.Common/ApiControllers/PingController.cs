using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.WSAPI.Lib.Services.Routing;

namespace Distributr.WebApi.ApiControllers
{
    public class PingController : ApiController
    {
        private ICommandRoutingOnRequestRepository _onRequestRepository;
        private ICostCentreRepository _costCentreRepository;

        public PingController(ICommandRoutingOnRequestRepository onRequestRepository, ICostCentreRepository costCentreRepository)
        {
            _onRequestRepository = onRequestRepository;
            _costCentreRepository = costCentreRepository;
        }

        [System.Web.Http.HttpGet]
        public HttpResponseMessage GetIsAlive()
        {
            var result = new ResponseBool();
            try
            {
                
             //   _onRequestRepository.RetireCommands(Guid.NewGuid() );
                _costCentreRepository.GetById(Guid.Empty);
                result.Success = true;
            }catch(Exception exception)
            {
                result.ErrorInfo = exception.Message;
                result.Success = false;
            }
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

    }

    [RoutePrefix("api2/ping2")]
    public class Ping2Controller : ApiController
    {
        [HttpGet, Route("getisalive")]
        public HttpResponseMessage GetIsAlive2()
        {
            return Request.CreateResponse(HttpStatusCode.OK, new ResponseBool { Success = true });
        }
    }

    [RoutePrefix("api2/ping3")]
    public class Ping3Controller : ApiController
    {
        private ICostCentreRepository _costCentreRepository;
        public Ping3Controller(ICostCentreRepository ccr)
        {
            _costCentreRepository = ccr;
        }

        [HttpGet, Route("getisalive")]
        public HttpResponseMessage GetIsAlive2()
        {
            var xxx = _costCentreRepository.GetAll();
            return Request.CreateResponse(HttpStatusCode.OK, new ResponseBool { Success = true });
        }
    }

}
