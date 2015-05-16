using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Distributr.Core.Commands;
using Distributr.Core.Commands.CommandPackage;
using Distributr.Core.Commands.DocumentCommands;
using Distributr.Core.Commands.DocumentCommands.Orders;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Repository.Transactional.DocumentRepositories;
using Distributr.Core.Utility.Serialization;
using Distributr.WSAPI.Lib.Services.CommandAudit;
using Distributr.WSAPI.Lib.Services.Routing;
using Distributr.WSAPI.Lib.Services.WebService.CommandDeserialization;
using Distributr.WSAPI.Lib.System.Utility;
using Distributr.WSAPI.Lib.System.Utility.CCAudit;
using Newtonsoft.Json;
using log4net;

namespace Distributr.WSAPI.Lib.Services.Bus.Impl
{
    public class SubscriberMessageHandler : ISubscriberMessageHandler
    {
        ICommandRoutingOnRequestRepository _commandRoutingOnRequestRepository;
        ICommandRoutingOnRequestResolver _commandRoutingOnRequestResolver;
        IRunCommandOnRequestInHostedEnvironment _runCommandOnRequestInHostedEnvironment;
        ICommandDeserialize _commandDeserialize;
        //private IBusPublisher _busPublisher;
        private ISubscriberCommandExecutionGuard _subscriberCommandExecutionGuard;
        private ICommandProcessingAuditRepository _commandProcessingAuditRepository;
        private ICommandEnvelopeProcessingAuditRepository _envelopeProcessingAuditRepository;
        private ICCAuditRepository _auditRepository;
        private IDocumentHelper _documentHelper;

        private ICommandEnvelopeRouteOnRequestCostcentreRepository _commandEnvelopeRouteOn;

        public SubscriberMessageHandler(ICommandRoutingOnRequestRepository commandRoutingOnRequestRepository, ICommandRoutingOnRequestResolver commandRoutingOnRequestResolver, IRunCommandOnRequestInHostedEnvironment runCommandOnRequestInHostedEnvironment, ICommandDeserialize commandDeserialize,  ISubscriberCommandExecutionGuard subscriberCommandExecutionGuard, ICommandProcessingAuditRepository commandProcessingAuditRepository, ICCAuditRepository auditRepository, IDocumentHelper documentHelper, ICommandEnvelopeRouteOnRequestCostcentreRepository commandEnvelopeRouteOn, ICommandEnvelopeProcessingAuditRepository envelopeProcessingAuditRepository)
        {
            _commandRoutingOnRequestRepository = commandRoutingOnRequestRepository;
            _commandRoutingOnRequestResolver = commandRoutingOnRequestResolver;
            _runCommandOnRequestInHostedEnvironment = runCommandOnRequestInHostedEnvironment;
            _commandDeserialize = commandDeserialize;
            //_busPublisher = busPublisher;
            _subscriberCommandExecutionGuard = subscriberCommandExecutionGuard;
            _commandProcessingAuditRepository = commandProcessingAuditRepository;
            _auditRepository = auditRepository;
            _documentHelper = documentHelper;
            _commandEnvelopeRouteOn = commandEnvelopeRouteOn;
            _envelopeProcessingAuditRepository = envelopeProcessingAuditRepository;
        }

        ILog _logger = LogManager.GetLogger("SubscriberMessageHandler");
        [Obsolete("Command Envelope Refactoring")]
        public void Handle(BusMessage message)
        {
            Stopwatch handlerTimer = new Stopwatch();
            handlerTimer.Start();
            _logger.InfoFormat(" Subscriber  Processing command : {0} with {1} ", message.CommandType, message.BodyJson);
            CommandProcessingStatus currentStatus = CommandProcessingStatus.SubscriberProcessBegin;
            //ICommand command = _commandDeserialize.DeserializeCommand(message.CommandType, message.BodyJson);
            ICommand command = JsonConvert.DeserializeObject<DocumentCommand>(message.BodyJson);
            try
            {
                
                //run guard to confirm if message can be run
                _subscriberCommandExecutionGuard.CanExecute(command);
                //record to command audit that message has arrived
               
                Audit("CommandProcessing", string.Format("Handle : {0} Command {1} of Type {2}", currentStatus.ToString(), command.CommandId.ToString(), message.CommandType), currentStatus.ToString(), command.CommandGeneratedByCostCentreId);
                if (!string.IsNullOrWhiteSpace(message.SendDateTime))
                {
                    DateTime _sendDateTime = _commandDeserialize.DeserializeSendDateTime(message.SendDateTime);
                    command.SendDateTime = _sendDateTime;
                }
                _logger.InfoFormat("Handle Command {4} : {0} for Document id {1} from cost centre {2} : {3}", command.CommandId, command.DocumentId, command.CommandGeneratedByCostCentreId, command.CommandGeneratedByCostCentreApplicationId, command.GetType().ToString());
                CommandType commandType = GetCommandType(message.CommandType);
                command = _documentHelper.GetExternalRef(command);
                CommandRouteOnRequestDTO commandRouteItems = _commandRoutingOnRequestResolver.GetCommand(command, commandType);

                if (commandRouteItems != null)
                {
                    
                    _runCommandOnRequestInHostedEnvironment.RunCommandInHostedenvironment(commandRouteItems.RouteOnRequest, command);

                    _commandRoutingOnRequestRepository.Add(commandRouteItems.RouteOnRequest);
                    var ccAdded = new List<Guid>();
                    foreach (var rc in commandRouteItems.CommandRouteCentres.Distinct())
                    {
                        if (ccAdded.Any(n => n == rc.CostCentreId))
                            continue;
                        ccAdded.Add(rc.CostCentreId);
                        rc.CommandRouteOnRequestId = commandRouteItems.RouteOnRequest.Id;
                        rc.DateAdded = DateTime.Now;
                        _commandRoutingOnRequestRepository.AddRoutingCentre(rc);
                    }
                    RouteExternalRef(commandRouteItems,command);
                   
                }
                currentStatus = CommandProcessingStatus.Complete;
              // _busPublisher.SignalComplete(command);

            }
            catch (MarkForRetryException ex)
            {
                _logger.Error("Markfor retry", ex);
                currentStatus = CommandProcessingStatus.MarkedForRetry;
            }
            catch (CommandAlreadyExecutedException ex)
            {
                _logger.Error("Command already executed", ex);
                currentStatus = CommandProcessingStatus.Failed;
            }
            catch (Exception ex)
            {
                //record to command audit that message failed to be processed
                _logger.Error("Command processing error ", ex);
                currentStatus = CommandProcessingStatus.MarkedForRetry;

                //throw ex;
            }

            Audit("CommandProcessing", string.Format("Handle : {0} Command {1} of Type {2}", currentStatus.ToString(), command.CommandId.ToString(), message.CommandType), currentStatus.ToString(), command.CommandGeneratedByCostCentreId);
            _commandProcessingAuditRepository.SetCommandStatus(message.MessageId, currentStatus);
            //record that message was processed successfully
            handlerTimer.Stop();
            _logger.InfoFormat("Total Request Time(m/s) {0} for command {1} ",handlerTimer.Elapsed.TotalMilliseconds, message.CommandType);
        }

        public void Handle(EnvelopeBusMessage message)
        {
            Stopwatch handlerTimer = new Stopwatch();
            handlerTimer.Start();
            var docType = (DocumentType) message.DocumentTypeId;
            _logger.InfoFormat(" Subscriber  Processing Envelope : {0} with {1} ", docType, message.BodyJson);
            EnvelopeProcessingStatus currentStatus = EnvelopeProcessingStatus.SubscriberProcessBegin;
              int lastExecutedCommand = 0;
          
            CommandEnvelope envelope = JsonConvert.DeserializeObject<CommandEnvelope>(message.BodyJson);
            try
            {
                var audit = _envelopeProcessingAuditRepository.GetById(envelope.Id);
                lastExecutedCommand = audit.LastExecutedCommand;
              

                Audit("CommandEnvelopeProcessing", string.Format("Handle : {0} Envelope {1} of Type {2}", currentStatus.ToString(), envelope.Id.ToString(), docType), currentStatus.ToString(), envelope.GeneratedByCostCentreId);
                if (!string.IsNullOrWhiteSpace(message.SendDateTime))
                {
                    DateTime _sendDateTime = _commandDeserialize.DeserializeSendDateTime(message.SendDateTime);
                   // envelope.SendDateTime = _sendDateTime;
                }
               
                if (envelope.CommandsList.Any())
                {
                  //  envelope = _documentHelper.GetExternalRef(envelope);
                    foreach (var envelopeItem in envelope.CommandsList.OrderBy(s=>s.Order) )
                    {
                        CommandType commandType = GetCommandType(envelopeItem.Command.CommandTypeRef);

                        _runCommandOnRequestInHostedEnvironment.RunCommandInHostedenvironment(envelopeItem.Command);
                        if (commandType == CommandType.CreateMainOrder)
                        {
                           RouteDocumentExternalRef(envelope.DocumentId,envelope.GeneratedByCostCentreId,envelope.RecipientCostCentreId, envelopeItem);
                        }
                        lastExecutedCommand = envelopeItem.Order;
                    }
                    _commandEnvelopeRouteOn.AddCommandEnvelopeRouteCentre(envelope);
                }
                
              
              
                currentStatus = EnvelopeProcessingStatus.Complete;
                // _busPublisher.SignalComplete(command);

            }
            catch (MarkForRetryException ex)
            {
                _logger.Error("Markfor retry", ex);
                currentStatus = EnvelopeProcessingStatus.MarkedForRetry;
            }
            catch (CommandAlreadyExecutedException ex)
            {
                _logger.Error("Command already executed", ex);
                currentStatus = EnvelopeProcessingStatus.Failed;
            }
            catch (Exception ex)
            {
                //record to command audit that message failed to be processed
                _logger.Error("Command processing error ", ex);
                currentStatus = EnvelopeProcessingStatus.MarkedForRetry;

                //throw ex;
            }
            Audit("CommandEnvelopeProcessing", string.Format("Handle : {0} Envelope {1} of Type {2}", currentStatus.ToString(), envelope.Id.ToString(), docType), currentStatus.ToString(), envelope.GeneratedByCostCentreId);
               
           // Audit("CommandProcessing", string.Format("Handle : {0} Command {1} of Type {2}", currentStatus.ToString(), envelope.CommandId.ToString(), message.CommandType), currentStatus.ToString(), envelope.CommandGeneratedByCostCentreId);
            _envelopeProcessingAuditRepository.SetStatus(message.MessageId, currentStatus,lastExecutedCommand);
            //record that message was processed successfully
            handlerTimer.Stop();
          //  _logger.InfoFormat("Total Request Time(m/s) {0} for command {1} ", handlerTimer.Elapsed.TotalMilliseconds, message.CommandType);
        }

        private void RouteDocumentExternalRef(Guid documentId, Guid generatedByCostCentreId, Guid recipientCostCentreId, CommandEnvelopeItem envelopeItem)
        {
                _documentHelper.GetExternalRef(envelopeItem.Command);
                var command = GetExternalCommnandRef(envelopeItem.Command);
                if (command != null)
                {
                    var env = new CommandEnvelope();
                    env.Id = Guid.NewGuid();
                    env.EnvelopeGeneratedTick = DateTime.Now.Ticks;
                    env.GeneratedByCostCentreId = generatedByCostCentreId;
                    env.RecipientCostCentreId = recipientCostCentreId;
                    env.DocumentTypeId = (int)DocumentType.Order;
                    env.GeneratedByCostCentreApplicationId = Guid.Empty;
                    env.ParentDocumentId = documentId;
                    env.DocumentId = documentId;
                    env.CommandsList.Add(new CommandEnvelopeItem(1, command));
                    env.EnvelopeArrivedAtServerTick = DateTime.Now.Ticks;
                    var audit = new CommandEnvelopeProcessingAudit
                    {

                        GeneratedByCostCentreApplicationId =
                            env.GeneratedByCostCentreApplicationId,

                        DateInserted = DateTime.Now,
                        Id = env.Id,
                        JsonEnvelope = JsonConvert.SerializeObject(env),
                        RetryCounter = 0,
                        Status = EnvelopeProcessingStatus.OnQueue,

                        DocumentId = env.DocumentId,
                        ParentDocumentId = env.ParentDocumentId,
                        DocumentType = (DocumentType)env.DocumentTypeId,
                        EnvelopeGeneratedTick = env.EnvelopeGeneratedTick,
                        EnvelopeArrivalAtServerTick = DateTime.Now.Ticks,
                        EnvelopeProcessOnServerTick = 0,
                        GeneratedByCostCentreId = env.GeneratedByCostCentreId,
                        RecipientCostCentreId = env.RecipientCostCentreId,
                        LastExecutedCommand = 1,
                        NumberOfCommand = env.CommandsList.Count,
                       
                        

                    };
                    audit.EnvelopeArrivalAtServerTick = DateTime.Now.Ticks;

                    audit.DocumentTypeName = audit.DocumentType.ToString();
                    _runCommandOnRequestInHostedEnvironment.RunCommandInHostedenvironment(command);
                    _envelopeProcessingAuditRepository.AddCommand(audit);
                    _commandEnvelopeRouteOn.AddCommandEnvelopeRouteCentre(env);
                }
           
        }

        private AddExternalDocRefCommand GetExternalCommnandRef(ICommand cmd)
        {
            if (cmd is CreateMainOrderCommand)
            {
                var cmdorder = cmd as CreateMainOrderCommand;
               
                var command = new AddExternalDocRefCommand();
                command.CommandId = Guid.NewGuid();
                command.DocumentId = cmdorder.DocumentId;
                command.CommandCreatedDateTime = DateTime.Now;
                command.SendDateTime = DateTime.Now;
                command.PDCommandId = cmdorder.PDCommandId;
                command.CommandGeneratedByCostCentreApplicationId = Guid.Empty;
                command.ExternalDocRef = cmdorder.ExtDocumentReference;

                return command;
            }
            return null;
        }

        private void RouteExternalRef(CommandRouteOnRequestDTO commandRouteItems,ICommand cmd)
        {
            if(cmd is CreateMainOrderCommand)
            {
                var cmdorder = cmd as CreateMainOrderCommand;
                commandRouteItems.RouteOnRequest.CommandType = CommandType.AddExternalDocRef.ToString();
                var command = new AddExternalDocRefCommand();
                command.CommandId = Guid.NewGuid();
                command.DocumentId = cmdorder.DocumentId;
                command.CommandCreatedDateTime = DateTime.Now;
                command.SendDateTime = DateTime.Now;
                command.PDCommandId = cmdorder.PDCommandId;
                command.CommandGeneratedByCostCentreApplicationId = Guid.Empty;
                command.ExternalDocRef = cmdorder.ExtDocumentReference;
                commandRouteItems.RouteOnRequest.Id = 0;
                commandRouteItems.RouteOnRequest.DocumentId = cmdorder.DocumentId;
                commandRouteItems.RouteOnRequest.DateAdded = DateTime.Now;
                commandRouteItems.RouteOnRequest.CommandGeneratedByCostCentreApplicationId = Guid.Empty;
                commandRouteItems.RouteOnRequest.DocumentParentId = cmdorder.DocumentId;
                commandRouteItems.RouteOnRequest.CommandId = command.CommandId;
                commandRouteItems.RouteOnRequest.JsonCommand = JsonConvert.SerializeObject(command);
                _commandRoutingOnRequestRepository.Add(commandRouteItems.RouteOnRequest);
                var ccAdded = new List<Guid>();
                foreach (var rc in commandRouteItems.CommandRouteCentres.Distinct())
                {
                    if (ccAdded.Any(n => n == rc.CostCentreId))
                        continue;
                    ccAdded.Add(rc.CostCentreId);
                    rc.Id = Guid.NewGuid();
                    rc.CommandRouteOnRequestId = commandRouteItems.RouteOnRequest.Id;
                    _commandRoutingOnRequestRepository.AddRoutingCentre(rc);
                }
            }
        }

        private void Audit(string action, string info, string results, Guid costcentreId)
        {
            _auditRepository.Add(new CCAuditItem
                                     {
                                         Action = action,
                                         CostCentreId = costcentreId,
                                         DateInsert = DateTime.Now,
                                         Id = Guid.NewGuid(),
                                         Info = info,
                                         Result = results,
                                     });
        }

        CommandType GetCommandType(string commandType)
        {
            CommandType _commandType;
            Enum.TryParse(commandType, out _commandType);
            return _commandType;
        }
    }
}
