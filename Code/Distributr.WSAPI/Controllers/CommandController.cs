using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Distributr.WSAPI.Lib.Services.CostCentreApplications;
using Distributr.WSAPI.Lib.Services.WebService.CommandDeserialization;
using Distributr.WSAPI.Lib.Services.Bus;
using Distributr.Core.Commands;
using Distributr.WSAPI.Lib.CommandResults;
using Newtonsoft.Json;
using System.IO;
using log4net;
using Distributr.WSAPI.Utility;
using Distributr.WSAPI.Lib.Services.CommandAudit;

namespace Distributr.WSAPI.Controllers
{
    public class CommandController : BaseController
    {
        ILog _log = LogManager.GetLogger("CommandController");
        ICommandDeserialize _commandDeserialize;
        IBusPublisher _busPublisher;
        private ICostCentreApplicationService _costCentreApplicationService;
        private ICommandProcessingAuditRepository _commandProcessingAuditRepository;

        public CommandController(ICommandDeserialize commandDeserialize, IBusPublisher busPublisher,
            ICostCentreApplicationService costCentreApplicationService, ICommandProcessingAuditRepository commandProcessingAuditRepository)
        {
            _commandDeserialize = commandDeserialize;
            _busPublisher = busPublisher;
            _costCentreApplicationService = costCentreApplicationService;
            _commandProcessingAuditRepository = commandProcessingAuditRepository;
        }

        [HttpPost]
        [JsonFilter]
        public JsonResult SLProcess(string commandType, string jsoncommand, string sendDateTime)
        {
            var responce = ProcessMessage(commandType, jsoncommand, sendDateTime, "DISTRIBUTOR");
            return Json(responce);
        }

        [HttpPost]
        public JsonResult Process(string commandType, string jsoncommand, string sendDateTime)
        {
            var responce = ProcessMessage(commandType, jsoncommand, sendDateTime, "MOBILE");
            return Json(responce);
        }

        private ResponseBasic ProcessMessage(string commandType, string jsoncommand, string sendDateTime, string source)
        {
            string result = "";
            string errorInfo = "";
            Guid ccId = Guid.Empty;
            _log.InfoFormat("Processing command : {0} with {1} from source {2} ", commandType, jsoncommand, source);
            try
            {
                DateTime _sendDateTime = _commandDeserialize.DeserializeSendDateTime(sendDateTime);
                ICommand deserializedCommand = _commandDeserialize.DeserializeCommand(commandType, jsoncommand);

                bool isValid = deserializedCommand != null;
                result = isValid ? "OK" : "Invalid";
                _log.InfoFormat("Deserialization is valid " + result);
                errorInfo = isValid ? "" : "Failed to deserialize " + commandType;

                if (isValid && !_costCentreApplicationService.IsCostCentreActive(deserializedCommand.CommandGeneratedByCostCentreApplicationId))
                {
                    isValid = false;
                    result = "Invalid";
                    errorInfo = "Inactive CostCentre Application Id ";
                }
                if (isValid && !QHealth.IsQueueHealthy)
                {
                    isValid = false;
                    result = "Invalid";
                    errorInfo = "Message Q not available";
                }

                if (isValid)
                {
                    ccId = deserializedCommand.CommandGeneratedByCostCentreId;
                    deserializedCommand.SendDateTime = _sendDateTime;
                    _log.Info("Client SendDateTime " + sendDateTime);
                    _log.InfoFormat("CommandId {0} Placed on bus", deserializedCommand.CommandId);
                    var message = new BusMessage
                    {
                        CommandType = commandType,
                        MessageId = deserializedCommand.CommandId,
                        BodyJson = jsoncommand,
                        SendDateTime = sendDateTime
                    };
                    var commandProcessingAudit = new CommandProcessingAudit
                                                                        {
                                                                            CommandType = commandType,
                                                                            CostCentreApplicationId = deserializedCommand.CommandGeneratedByCostCentreApplicationId,
                                                                            CostCentreCommandSequence = deserializedCommand.CommandSequence,
                                                                            DateInserted = DateTime.Now,
                                                                            Id = deserializedCommand.CommandId,
                                                                            JsonCommand = jsoncommand,
                                                                            RetryCounter = 0,
                                                                            Status = CommandProcessingStatus.OnQueue,
                                                                            SendDateTime=sendDateTime,
                                                                            DocumentId = deserializedCommand.DocumentId,
                                                                            ParentDocumentId=deserializedCommand.PDCommandId
                                                                        };

                   
                    _busPublisher.Publish(message);
                    AuditCCHit(deserializedCommand.CommandGeneratedByCostCentreId, "CommandProcessing", string.Format("Publish : {0} Command {1} of Type {2}", CommandProcessingStatus.OnQueue.ToString(), deserializedCommand.CommandId.ToString(), message.CommandType), CommandProcessingStatus.OnQueue.ToString());
                   _commandProcessingAuditRepository.AddCommand(commandProcessingAudit);
                }
                else
                    _log.Error(errorInfo);
            }
            catch (Exception ex)
            {
                result = "Invalid";
                _log.InfoFormat("ERROR Processing command : {0}", commandType);
                _log.Error(ex);
            }

            ResponseBasic responce = new ResponseBasic { Result = result, ErrorInfo = errorInfo };
            _log.Info("Final responce " + JsonConvert.SerializeObject(responce));
            AuditCCHit(ccId, "slprocess", "CommandType : " + commandType, result);
            return responce;
        }
        private string getStringFromInputStream(Stream str)
        {
            String strmContents;
            Int32 strLen, strRead;
            strLen = Convert.ToInt32(str.Length);
            byte[] strArr = new byte[strLen];
            strRead = str.Read(strArr, 0, strLen);
            strmContents = System.Text.Encoding.UTF8.GetString(strArr);
            return strmContents;
        }
    }

    public class JsonFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var incomingData = new StreamReader(filterContext.HttpContext.Request.InputStream).ReadToEnd();
            filterContext.ActionParameters["jsoncommand"] = incomingData.Replace("jsoncommand=", "");
            base.OnActionExecuting(filterContext);
        }
    }


}
