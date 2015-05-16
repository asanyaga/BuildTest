using System;
using Distributr.Core.Commands;
using Distributr.Core.Utility;
using Distributr.Core.Utility.Command;
using Distributr.WPF.Lib.Impl.Repository.Transactional.AuditLog;
using Distributr.WPF.Lib.Service.Utility;
using Distributr.WPF.Lib.Services.Service.CommandQueues;
using Distributr.WPF.Lib.Services.Service.Transactional.Commands.CommandRouting;
using log4net;
using StructureMap;

namespace Distributr.WPF.Lib.Data.ExecuteCommands
{
    public class ExecuteCommandLocally : IExecuteCommandLocally
    {
        private IResolveCommand _resolveCommand;
        //private ILogToServer _logToServer;
        private IErrorLogRepository _errorLogService;
        private IIncomingCommandQueueRepository _incomingCommandQueueRepository;
        ILog _log = LogManager.GetLogger("ExecuteCommandLocally");
        public ExecuteCommandLocally(IIncomingCommandQueueRepository incomingCommandQueueRepository,IResolveCommand resolveCommand, IErrorLogRepository errorLogService)
        {
            _resolveCommand = resolveCommand;
            //_logToServer = logToServer;
            _errorLogService = errorLogService;
            _incomingCommandQueueRepository = incomingCommandQueueRepository;
        }

        public void ExecuteCommand(CommandType commandType, ICommand command)
        {
           
            try
            {
                _log.InfoFormat("Attempting to execute command locally commandtype {0} --- commandid {1} --- documentid {2} ", command.CommandTypeRef, command.CommandId, command.DocumentId);
                ResolveCommandItem rci = _resolveCommand.Get(commandType);
                //AJM TODO
                using (var container = ObjectFactory.Container.GetNestedContainer())
                {
                    object ch = container.GetInstance(rci.CommandHandlerContract);
                    //object ch = ObjectFactory.GetInstance(rci.CommandHandlerContract); //IoC.Get(rci.CommandHandlerContract);
                    Type u = typeof (ICommandHandler<>);
                    Type c = u.MakeGenericType(rci.Command);
                    //object inst = Activator.CreateInstance(c);
                    c.GetMethod("Execute").Invoke(ch, new object[] {command});
                    _log.InfoFormat("Command locally executed =>commandtype {0} --- commandid {1} --- documentid {2} ", command.CommandTypeRef, command.CommandId, command.DocumentId);

                }
            }
            catch (Exception ex)
            {
                _log.Error("error executing local command id " + command.CommandId + " command type " + command.CommandTypeRef  , ex);
                string logInfoFormat = "Failed processing commandtype : {0} - commandid : {1} - Error : {2} ";
                string error = string.Format(logInfoFormat, commandType, command.CommandId, ex.Message + ex.StackTrace);
                _errorLogService.Log("ExecuteCommand- generic", error);
                //_logToServer.SendError(error);
                throw ex; //after logging rethrow
            }
        }
    }
}
