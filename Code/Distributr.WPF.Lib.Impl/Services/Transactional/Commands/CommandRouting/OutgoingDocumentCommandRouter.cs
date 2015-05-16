using System;
using Distributr.Core.ClientApp;
using Distributr.Core.Commands;
using Distributr.Core.Commands.CommandPackage;
using Distributr.Core.Utility;
using Distributr.Core.Utility.Command;
using Distributr.WPF.Lib.Services.Service.CommandQueues;
using Distributr.WPF.Lib.Services.Service.Transactional.Commands.CommandRouting;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Distributr.WPF.Lib.Impl.Services.Transactional.Commands.CommandRouting
{
    //REFACTOR long term goal - all docs save should be through the RouteDocumentCommand

    public class OutgoingDocumentCommandRouter : IOutgoingDocumentCommandRouter
    {
        IOutgoingCommandQueueRepository _outgoingCommandQueueRepository;
       
        private IResolveCommand _resolveCommand;
        private IExecuteCommandLocally _executeCommandLocally;
        ILog _log = LogManager.GetLogger("OutgoingDocumentCommandRouter");

        public OutgoingDocumentCommandRouter(IOutgoingCommandQueueRepository outgoingCommandQueueRepository, IResolveCommand resolveCommand, IExecuteCommandLocally executeCommandLocally)
        {
            _outgoingCommandQueueRepository = outgoingCommandQueueRepository;
            _resolveCommand = resolveCommand;
            _executeCommandLocally = executeCommandLocally;
        }

        public void RouteDocumentCommand(ICommand command)
        {
            _log.InfoFormat("RouteDocumentCommand -- commandtype {0} --  commandid {1} -- documentid {1} --  ", command.CommandTypeRef, command.CommandId, command.DocumentId);
            if (_outgoingCommandQueueRepository.GetByCommandId(command.CommandId) != null)
                return;
            CommandType commandType = _resolveCommand.Get(command).CommandType;
           
            var queueItem = new OutgoingCommandQueueItemLocal
                                {
                                    CommandId = command.CommandId,
                                    CommandType = commandType,
                                    DateInserted = DateTime.Now,
                                    DocumentId = command.DocumentId,
                                    IsCommandSent = false,
                                    DateSent = DateTime.Now,
                                    JsonCommand = JsonConvert.SerializeObject(command, new IsoDateTimeConverter())
                                };
            _outgoingCommandQueueRepository.Add(queueItem);
            if (!command.IsSystemCommand)
            {
                
                _executeCommandLocally.ExecuteCommand(commandType, command);
              
            }
            

        }


        public void RouteDocumentCommandWithOutSave(ICommand command)
        {
            if (_outgoingCommandQueueRepository.GetByCommandId(command.CommandId) != null)
                return;
            CommandType commandType = _resolveCommand.Get(command).CommandType;
            //DateTime executeLocally = DateTime.Now;
            //_executeCommandLocally.ExecuteCommand(commandType, command);
            //TimeSpan endexecuteLocally = DateTime.Now.Subtract(executeLocally);
            DateTime createoutgoingcommads = DateTime.Now;
            var queueItem = new OutgoingCommandQueueItemLocal
            {
                CommandId = command.CommandId,
                CommandType = commandType,
                DateInserted = DateTime.Now,
                DocumentId = command.DocumentId,
                IsCommandSent = false,
                DateSent = DateTime.Now,
                JsonCommand = JsonConvert.SerializeObject(command, new IsoDateTimeConverter())
            };
            _outgoingCommandQueueRepository.Add(queueItem);
            //TimeSpan endcreateInvoice = DateTime.Now.Subtract(createoutgoingcommads);
        }

        public void RouteCommandEnvelope(CommandEnvelope envelope)
        {
            throw new NotImplementedException();
        }
    }
}
