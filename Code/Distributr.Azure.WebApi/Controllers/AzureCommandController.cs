using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Commands.DocumentCommands;
using Distributr.WSAPI.Lib.Services.Bus;
using Distributr.WSAPI.Lib.Services.CommandAudit;
using Distributr.WSAPI.Lib.Services.CostCentreApplications;
using Distributr.WebApi.ApiControllers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using log4net;

namespace Distributr.Azure.WebApi.Controllers
{
    public class AzureCommandController : BaseApiController
    {
        ILog _log = LogManager.GetLogger("CommandController");
        IBusPublisher _busPublisher;
        private ICostCentreApplicationService _costCentreApplicationService;
        private ICommandProcessingAuditRepository _commandProcessingAuditRepository;

        public AzureCommandController(IBusPublisher busPublisher, ICostCentreApplicationService costCentreApplicationService, ICommandProcessingAuditRepository commandProcessingAuditRepository)
        {
            _busPublisher = busPublisher;
            _costCentreApplicationService = costCentreApplicationService;
            _commandProcessingAuditRepository = commandProcessingAuditRepository;
        }

        [HttpPost]
        public HttpResponseMessage Run(JObject jcommand)
        {

            Guid commandId = Guid.Empty;
            var responseBasic = new ResponseBasic();
            HttpStatusCode returnCode = HttpStatusCode.OK;
            try
            {
                responseBasic.ResultInfo = "invalid jsoncommand";
                DocumentCommand command = JsonConvert.DeserializeObject<DocumentCommand>(jcommand.ToString());
                responseBasic.ResultInfo = "valid jsoncommand";
                _log.InfoFormat("Received command id {0} : command type {1} : document id {2}", command.CommandId, command.CommandTypeRef, command.DocumentId);
                bool isValid = command != null;

                commandId = command.CommandId;
                //if (isValid && !_costCentreApplicationService.IsCostCentreActive(command.CommandGeneratedByCostCentreApplicationId))
                //{
                //    isValid = false;
                //    returnCode = HttpStatusCode.NotAcceptable;
                //    responseBasic.Result = "Inactive CostCentre Application Id ";
                //    _log.InfoFormat("Cost centre is not active for command id {0} cost centre application id {1}", command.CommandId, command.CommandGeneratedByCostCentreApplicationId);
                //}
                if (isValid)
                {
                    _log.InfoFormat("CommandId {0} Placed on bus", command.CommandId);
                    var message = new BusMessage
                    {
                        CommandType = command.CommandTypeRef,
                        MessageId = command.CommandId,
                        BodyJson = JsonConvert.SerializeObject(command),
                        SendDateTime = DateTime.Now.ToString(),
                        IsSystemMessage = command.IsSystemCommand
                    };
                    var commandProcessingAudit = new CommandProcessingAudit
                    {
                        CommandType = command.CommandTypeRef,
                        CostCentreApplicationId = command.CommandGeneratedByCostCentreApplicationId,
                        CostCentreCommandSequence = command.CommandSequence,
                        DateInserted = DateTime.Now,
                        Id = command.CommandId,
                        JsonCommand = message.BodyJson,
                        RetryCounter = 0,
                        Status = CommandProcessingStatus.OnQueue,
                        SendDateTime = message.SendDateTime,
                        DocumentId = command.DocumentId,
                        ParentDocumentId = command.PDCommandId
                    };

                    _busPublisher.Publish(message);
                    AuditCCHit(command.CommandGeneratedByCostCentreId, "CommandProcessing",
                               string.Format("Publish : {0} Command {1} of Type {2}",
                                             CommandProcessingStatus.OnQueue.ToString(), command.CommandId.ToString(),
                                             message.CommandType), CommandProcessingStatus.OnQueue.ToString());
                    _commandProcessingAuditRepository.AddCommand(commandProcessingAudit);
                    responseBasic.Result = "Command Processed";


                }
            }
            catch (Exception ex)
            {
                responseBasic.Result = "Processing Failed";
                responseBasic.ErrorInfo = ex.Message;
                _log.Error("Failed to process command", ex);
            }
            HttpResponseMessage response = Request.CreateResponse(returnCode, responseBasic);
            _log.InfoFormat("ResponseMessage : commandId = {0}  : response code = {1}  : Response Result = {2}", commandId, returnCode, responseBasic.Result);
            return response;
        }
    }
}
