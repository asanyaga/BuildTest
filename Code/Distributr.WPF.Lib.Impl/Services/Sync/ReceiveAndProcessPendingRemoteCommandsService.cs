using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Commands.DocumentCommands;
using Distributr.Core.Utility;
using Distributr.Core.Workflow.Impl.AuditLogs;
using Distributr.WPF.Lib.Impl.Repository.Transactional.AuditLog;
using Distributr.WPF.Lib.Impl.Repository.Utility;
using Distributr.WPF.Lib.Impl.Services.Transactional.Commands.IncomingCommandHandlers;
using Distributr.WPF.Lib.Services.Service.CommandQueues;
using Distributr.WPF.Lib.Services.Service.Sync;
using Distributr.WPF.Lib.Services.Service.WSProxies;
using GalaSoft.MvvmLight.Messaging;
using Newtonsoft.Json;

namespace Distributr.WPF.Lib.Impl.Services.Sync
{
    [Obsolete("Command Envelope Refactoring")]
    public class ReceiveAndProcessPendingRemoteCommandsService : IReceiveAndProcessPendingRemoteCommandsService
    {
        private IIncomingCommandHandler _incomingCommandHandler;
        private IAuditLogWFManager _auditLogWFManager;
        private IErrorLogRepository _errorLogRepository;
        private IConfigRepository _configRepository;
        private IWebApiProxy _webApiProxy;
           private IIncomingCommandQueueRepository _incomingCommandQueueRepository;
        public ReceiveAndProcessPendingRemoteCommandsService(IWebApiProxy webApiProxy, IConfigRepository configRepository, 
            IErrorLogRepository errorLogRepository, IAuditLogWFManager auditLogWFManager,  IIncomingCommandHandler incomingCommandHandler, IIncomingCommandQueueRepository incomingCommandQueueRepository)
        {
            _incomingCommandHandler = incomingCommandHandler;
            _incomingCommandQueueRepository = incomingCommandQueueRepository;
            _auditLogWFManager = auditLogWFManager;
            _errorLogRepository = errorLogRepository;
            _configRepository = configRepository;
            _webApiProxy = webApiProxy;
        }

        public async Task<bool> BatchReceiveAndProcessNextCommandAsync(Guid costCentreApplicationId)
        {
            List<long> batchIds = _configRepository.GetDeliveredCommand();
            if (batchIds == null)
            {
                batchIds = new List<long>();
                batchIds.Add(0);
            }
            string batchidsjson = JsonConvert.SerializeObject(batchIds);
            BatchDocumentCommandRoutingResponse response = null;
            
                response =
                    await _webApiProxy.GetNextBatchDocumentCommandAsync(costCentreApplicationId, 0, 30, batchidsjson);
                        if (response == null)
                return false;
            _configRepository.ClearDeliveredCommand();
            if (response.CommandRoutingCount == 0)
            {

                return true;
            }

            foreach (DocumentCommandRoutingResponse r in response.RoutingCommands)
            {
                try
                {
                    
                        //Task.Run(
                        //    () => _incomingCommandHandler.HandleCommand(r.CommandType, r.Command, r.CommandRouteItemId))
                        //    .ConfigureAwait(false);

                    _incomingCommandHandler.HandleCommand(r.CommandType, r.Command, r.CommandRouteItemId);
                       
                    _auditLogWFManager.AuditLogEntry("ProcessBatch",
                                                     string.Format(
                                                         "Command running commandType : {0} - commandid : {1}  Successfull  ",
                                                         r.CommandType.ToString(), r.CommandRouteItemId));
                }
                catch (Exception ex)
                {
                    string error = ex.AllMessages();
                    error += ex.StackTrace;
                    _errorLogRepository.Save("ProcessBatch",
                                             string.Format(
                                                 "Command running commandType : {0} - commandid : {1}  failed  Exception {2}",
                                                 r.CommandType.ToString(), r.CommandRouteItemId, error));
                    break;
                }
            }
            if (response.CommandRoutingCount != 0)
            {
                return await BatchReceiveAndProcessNextCommandAsync(costCentreApplicationId);
            }
            return true;
        }

        public async Task<bool> ProcessUnExecutedCommandAsync(Guid costCentreApplicationId)
        {
            var data = _incomingCommandQueueRepository.GetUnProcessedCommands();
            foreach (var dcom in data)
            {
                try
                {
                    var cmd = JsonConvert.DeserializeObject<DocumentCommand>(dcom.JsonCommand);
                    if (cmd != null)
                        _incomingCommandHandler.HandleUnProcessCommand(cmd);
                    _auditLogWFManager.AuditLogEntry("ProcessBatch",
                        string.Format(
                            "Command running commandType : {0} - commandid : {1}  Successfull  ",
                            cmd.CommandTypeRef.ToString(), cmd.CommandId));



                }
                catch (Exception ex)
                {
                    string error = ex.AllMessages();
                    error += ex.StackTrace;
                    _errorLogRepository.Save("ProcessBatch",
                        string.Format(
                            "Command running commandType : {0} - commandid : {1}  failed  Exception {2}",
                            dcom.CommandType.ToString(), dcom.Id, error));
                }

            }
            return true;
        }
    
    }
}
