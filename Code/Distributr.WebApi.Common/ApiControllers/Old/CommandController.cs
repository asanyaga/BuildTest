using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Commands.DocumentCommands;
using Distributr.WSAPI.Lib.Services.Bus;
using Distributr.WSAPI.Lib.Services.CommandAudit;
using Distributr.WSAPI.Lib.Services.CostCentreApplications;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using log4net;

namespace Distributr.WebApi.ApiControllers
{
    public class CommandController : BaseApiController
    {
        ILog _log = LogManager.GetLogger("CommandController");
        IBusPublisher _busPublisher;
        private ICostCentreApplicationService _costCentreApplicationService;
        private ICommandProcessingAuditRepository _commandProcessingAuditRepository;

        public CommandController(IBusPublisher busPublisher, ICostCentreApplicationService costCentreApplicationService, ICommandProcessingAuditRepository commandProcessingAuditRepository)
        {
            _busPublisher = busPublisher;
            _costCentreApplicationService = costCentreApplicationService;
            _commandProcessingAuditRepository = commandProcessingAuditRepository;
        }

        [HttpPost]
        public HttpResponseMessage Run(JObject jcommand)
        {
            
            var request = Request;
          
            //var str = Request.Content.ReadAsStringAsync().Result;
            //var stst = request.Content.IsFormData();
           
          
            
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
                    if (_commandProcessingAuditRepository.TestMyConnection())
                    {
                        _busPublisher.Publish(message);
                        _commandProcessingAuditRepository.AddCommand(commandProcessingAudit);
                        AuditCCHit(command.CommandGeneratedByCostCentreId, "CommandProcessing",
                                   string.Format("Publish : {0} Command {1} of Type {2}",
                                                 CommandProcessingStatus.OnQueue.ToString(),
                                                 command.CommandId.ToString(),
                                                 message.CommandType), CommandProcessingStatus.OnQueue.ToString());

                        responseBasic.Result = "Command Processed";
                    }
                    else
                    {
                        responseBasic.Result = "Processing Failed";
                        responseBasic.ErrorInfo = "Mongo down";
                    }


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
