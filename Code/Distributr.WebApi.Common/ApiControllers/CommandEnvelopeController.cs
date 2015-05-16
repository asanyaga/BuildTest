using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Commands.CommandPackage;
using Distributr.Core.Commands.DocumentCommands;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.WebApi.ApiControllers;
using Distributr.WSAPI.Lib.Services.Bus;
using Distributr.WSAPI.Lib.Services.CommandAudit;
using Distributr.WSAPI.Lib.Services.CostCentreApplications;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Distributr.WebApi.Common.ApiControllers
{
    [RoutePrefix("api/commandenvelope")]
   public class CommandEnvelopeController :BaseApiController
   {
       private ICommandEnvelopeProcessingAuditRepository _commandEnvelopeProcessingAuditRepository;
        private ICostCentreApplicationService _costCentreApplicationService;
      // IBusPublisher _busPublisher;
        public CommandEnvelopeController(ICommandEnvelopeProcessingAuditRepository commandEnvelopeProcessingAuditRepository, ICostCentreApplicationService costCentreApplicationService, IControllerBusPublisher busPublisher)
        {
            _commandEnvelopeProcessingAuditRepository = commandEnvelopeProcessingAuditRepository;
            _costCentreApplicationService = costCentreApplicationService;
            _busPublisher = busPublisher;
        }

        IControllerBusPublisher _busPublisher;
       private ILog _log = LogManager.GetLogger("CommandEnvelopeController");
    
       [System.Web.Http.HttpPost]
       [Route("run")]
       public HttpResponseMessage Post(JObject jcommand)
       {
        
           var responseBasic = new ResponseBasic();
           responseBasic.Status = false;
           HttpStatusCode returnCode = HttpStatusCode.OK;
           try
           {
              
               responseBasic.ResultInfo = "invalid jsoncommand";
               string json = jcommand.ToString();
               CommandEnvelope envelope = JsonConvert.DeserializeObject<CommandEnvelope>(json);
               responseBasic.ResultInfo = "valid jsoncommand";
                bool isValid = envelope != null;
                _log.Info("responseBasic.ResultInfo " + responseBasic.ResultInfo);
               if (isValid)
               {
                   if (!_costCentreApplicationService.IsCostCentreActive(envelope.GeneratedByCostCentreApplicationId))
                   {
                       responseBasic.Status=false;
                       returnCode = HttpStatusCode.OK;
                       responseBasic.Result = "Inactive CostCentre Application Id";
                       _log.InfoFormat("Cost centre is not active for envelope id {0} ccId {1} ccid",envelope.Id, envelope.GeneratedByCostCentreApplicationId,envelope.GeneratedByCostCentreId);
                     return Request.CreateResponse(returnCode, responseBasic);
                   }
                   _log.InfoFormat("EnvelopeId {0} " ,envelope.Id.ToString());
               
                   envelope.EnvelopeArrivedAtServerTick = DateTime.Now.Ticks;

                   var message = new EnvelopeBusMessage
                                 {
                                     DocumentTypeId = envelope.DocumentTypeId,
                                     MessageId = envelope.Id,
                                     BodyJson = JsonConvert.SerializeObject(envelope),
                                     SendDateTime = DateTime.Now.ToString(),
                                     IsSystemMessage = envelope.IsSystemEnvelope
                                 };
                   var envelopeProcessingAudit = new CommandEnvelopeProcessingAudit
                                                 {

                                                     GeneratedByCostCentreApplicationId =
                                                         envelope.GeneratedByCostCentreApplicationId,
                                                    
                                                     DateInserted = DateTime.Now,
                                                     Id = envelope.Id,
                                                     JsonEnvelope = message.BodyJson,
                                                     RetryCounter = 0,
                                                     Status = EnvelopeProcessingStatus.OnQueue,
                                                     SendDateTime = message.SendDateTime,
                                                     DocumentId = envelope.DocumentId,
                                                     ParentDocumentId = envelope.ParentDocumentId,
                                                     DocumentType = (DocumentType) envelope.DocumentTypeId,
                                                     EnvelopeGeneratedTick = envelope.EnvelopeGeneratedTick,
                                                     EnvelopeArrivalAtServerTick = envelope.EnvelopeArrivedAtServerTick,
                                                     EnvelopeProcessOnServerTick = 0,
                                                     GeneratedByCostCentreId = envelope.GeneratedByCostCentreId,
                                                     RecipientCostCentreId = envelope.RecipientCostCentreId,
                                                     LastExecutedCommand = 0,
                                                     NumberOfCommand = envelope.CommandsList.Count

                                                 };
                   envelopeProcessingAudit.DocumentTypeName = envelopeProcessingAudit.DocumentType.ToString();
                   if (_commandEnvelopeProcessingAuditRepository.IsConnected())
                   {
                       var exist = _commandEnvelopeProcessingAuditRepository.GetById(envelope.Id);
                       if (exist == null)
                       {
                           _commandEnvelopeProcessingAuditRepository.AddCommand(envelopeProcessingAudit);
                           _busPublisher.Publish(message);
                       }
                       else
                       {
                           _log.InfoFormat("EnvelopeId {0}  Already Published", envelope.Id.ToString());
                       }
                       _log.InfoFormat("EnvelopeId {0}  Published", envelope.Id.ToString());
                       responseBasic.Status = true;
                       responseBasic.Result = "Envelope Processed";
                       _log.Info("  responseBasic.Result " + responseBasic.Result);
                   }
                   else
                   {
                       responseBasic.Result = "Processing Failed";
                       responseBasic.ErrorInfo = "Mongo down";
                       _log.Info("   responseBasic.ErrorInfo " + responseBasic.ErrorInfo);
                   }
               }
           }
           catch (Exception ex)
           {
               responseBasic.Result = "Processing Failed";
               responseBasic.ErrorInfo = ex.Message;
               _log.Info("   responseBasic.Result" + responseBasic.Result);
               _log.Info("   responseBasic.ErrorInfo " + responseBasic.ErrorInfo);
               
                
           }
           HttpResponseMessage response = Request.CreateResponse(returnCode, responseBasic);

           return response;
       }
    }
}
