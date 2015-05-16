using System;
using System.Linq;
using Distributr.Core.Commands.CommandPackage;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Utility;
using Distributr.Core.Commands;
using Distributr.Core.Utility.Command;
using Distributr.Core.Workflow.Impl.AuditLogs;
using Distributr.WPF.Lib.Impl.Model.Utility;
using Distributr.WPF.Lib.Impl.Repository.Transactional.AuditLog;
using Distributr.WPF.Lib.Impl.Repository.Utility;
using Distributr.WPF.Lib.Service.Utility;
using Distributr.WPF.Lib.Services.Security;
using Distributr.WPF.Lib.Services.Service.CommandQueues;
using Distributr.WPF.Lib.Services.Service.Transactional.Commands.CommandRouting;
using Distributr.WPF.Lib.Services.Service.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Distributr.WPF.Lib.Impl.Services.Transactional.Commands.IncomingCommandHandlers
{
    public class IncomingCommandHander : IIncomingCommandHandler
    {
        private IConfigService _configService;
        private IExecuteCommandLocally _executeCommandLocally;
        private IResolveCommand _resolveCommand;
        //private ILogToServer _logToServer;
        private IUnExecutedCommandRepository _unExecutedCommandRepository;
        private IAuditLogWFManager _auditLogWFManager;
        private IErrorLogRepository _errorLogService;
        private IIncomingCommandQueueRepository _incomingCommandQueueRepository;
        private IIncomingCommandEnvelopeQueueRepository _incomingCommandEnvelopeQueueRepository;
        private IConfigRepository _configRepository;
        public IncomingCommandHander(IConfigRepository configRepository,IIncomingCommandQueueRepository incomingCommandQueueRepository, IConfigService configService, IExecuteCommandLocally executeCommandLocally, IResolveCommand resolveCommand, IUnExecutedCommandRepository unExecutedCommandRepository, IAuditLogWFManager auditLogWfManager, IErrorLogRepository errorLogService, IIncomingCommandEnvelopeQueueRepository incomingCommandEnvelopeQueueRepository)
        {
            _configService = configService;
            _executeCommandLocally = executeCommandLocally;
            _resolveCommand = resolveCommand;
            //_logToServer = logToServer;
            _unExecutedCommandRepository = unExecutedCommandRepository;
            _auditLogWFManager = auditLogWfManager;
            _errorLogService = errorLogService;
            _incomingCommandEnvelopeQueueRepository = incomingCommandEnvelopeQueueRepository;
            _incomingCommandQueueRepository = incomingCommandQueueRepository;
            _configRepository = configRepository;
        }

        public void HandleCommand(string commandType, ICommand command, long commandRouteItemId)
        {

           
            try
            {
                ResolveCommandItem rc = _resolveCommand.Get(command);
                if (_incomingCommandQueueRepository.ExitOldCommand())
                {
                    _incomingCommandQueueRepository.DeleteOldCommand();
                }
                IncomingCommandQueueItemLocal cmd = new IncomingCommandQueueItemLocal
                {
                    CommandId = command.CommandId,
                    CommandType = rc.CommandType,
                    DateInserted = DateTime.Now,
                    DocumentId = command.DocumentId,
                    Processed = false,
                    DateReceived = DateTime.Now,
                    JsonCommand = JsonConvert.SerializeObject(command),
                    NoOfRetry = 0
                };
                _incomingCommandQueueRepository.Add(cmd);
             
                _executeCommandLocally.ExecuteCommand(rc.CommandType, command);
                _incomingCommandQueueRepository.MarkAsProcessed(command.CommandId);
                _auditLogWFManager.AuditLogEntry("HandleCommand", string.Format("Command {0} Successfully Executed. Command Type {1}   ", commandRouteItemId, commandType));
            }
            catch(Exception ex)
            {
                
                _errorLogService.Log("IncomingCommandHander", string.Format("Command {0} Failed to Execute. Command Type {1}  error {2}", commandRouteItemId, commandType,ex.Message));
                UnExecutedCommandLocal log = new UnExecutedCommandLocal
                                                 {
                                                     Command = JsonConvert.SerializeObject(command, new IsoDateTimeConverter()),
                                                     CommandType = commandType,
                                                     DocumentId=command.DocumentId,
                                                     Reason=ex.AllMessages()+ ex.StackTrace,
                                                 };
                _unExecutedCommandRepository.Save(log);
            }


            //Config config = _configService.Load();
            //config.LastDeliveredCommandRouteItemId = commandRouteItemId;
            //_configService.Save(config);

            if (_incomingCommandQueueRepository.GetByCommandId(command.CommandId) != null)
            {
                _configRepository.AddDeliveredCommand(commandRouteItemId);
            }
            

        }

        public void HandleCommandEnvelope( CommandEnvelope envelope)
        {
            try
            {
                var envelopeToSave = new InComingCommandEnvelopeQueueItemLocal
                {
                    EnvelopeId = envelope.Id,
                    DocumentType =(DocumentType) envelope.DocumentTypeId,
                    DateInserted = DateTime.Now,
                    DocumentId = envelope.DocumentId,
                    Processed = false,
                   
                    JsonEnvelope = JsonConvert.SerializeObject(envelope),
                    NoOfRetry = 0,
                   Info = ""
                };
                _incomingCommandEnvelopeQueueRepository.Add(envelopeToSave);
                foreach (var cmdItem in envelope.CommandsList.OrderBy(s => s.Order))
                {
                    var cmd = cmdItem.Command;
                    ResolveCommandItem rc = _resolveCommand.Get(cmd);
                    _executeCommandLocally.ExecuteCommand(rc.CommandType, cmd);
                    //   _auditLogWFManager.AuditLogEntry("HandleCommand", string.Format("Command {0} Successfully Executed. Command Type {1}   ", commandRouteItemId, commandType));
                    //



                }
                _incomingCommandEnvelopeQueueRepository.MarkAsProcessed(envelope.Id);
                if (_incomingCommandEnvelopeQueueRepository.GetByEnvelopeId(envelope.Id) != null)
                {
                    _configRepository.AddDeliveredCommandEnvelopeId(envelope.Id);
                }
            }
            catch (Exception ex)
            {
                
            }
           
        }

        public void HandleUnProcessCommand(ICommand command)
        {
            try
            {
                ResolveCommandItem rc = _resolveCommand.Get(command);
                
                _executeCommandLocally.ExecuteCommand(rc.CommandType, command);
                _incomingCommandQueueRepository.MarkAsProcessed(command.CommandId);
               // _auditLogWFManager.AuditLogEntry("HandleCommand", string.Format("Command {0} Successfully Executed. Command Type {1} "));
            }
            catch (Exception ex)
            {
                _incomingCommandQueueRepository.IncrimentRetryCounter(command.CommandId);
                _errorLogService.Log("HandleUnProcessCommand", string.Format("Command {0} Failed to Execute. Command Type {1}  error {2}",command.CommandId,command.CommandTypeRef, ex.Message));
              
            }


        }
    }
}
