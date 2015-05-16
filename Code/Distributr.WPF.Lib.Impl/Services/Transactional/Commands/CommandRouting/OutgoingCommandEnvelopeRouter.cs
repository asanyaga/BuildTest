using System;
using Distributr.Core.ClientApp;
using Distributr.Core.Commands;
using Distributr.Core.Commands.CommandPackage;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Utility;
using Distributr.Core.Utility.Command;
using Distributr.WPF.Lib.Services.Service.CommandQueues;
using Distributr.WPF.Lib.Services.Service.Transactional.Commands.CommandRouting;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Distributr.WPF.Lib.Impl.Services.Transactional.Commands.CommandRouting
{
    public class OutgoingCommandEnvelopeRouter : IOutgoingCommandEnvelopeRouter
    {
        private IOutgoingCommandEnvelopeQueueRepository _envelopeQueueRepository;
        private IResolveCommand _resolveCommand;
        private IExecuteCommandLocally _executeCommandLocally;
        ILog _log = LogManager.GetLogger("OutgoingCommandEnvelopeRouter");
        public OutgoingCommandEnvelopeRouter(IOutgoingCommandEnvelopeQueueRepository envelopeQueueRepository, IResolveCommand resolveCommand, IExecuteCommandLocally executeCommandLocally)
        {
            _envelopeQueueRepository = envelopeQueueRepository;
            _resolveCommand = resolveCommand;
            _executeCommandLocally = executeCommandLocally;
        }

        public void RouteCommandEnvelope(CommandEnvelope envelope)
        {
            _log.InfoFormat("RouteCommandEnvelope -- DocumentId {0} --  commandid {1} -- documentid {1} --  ",
                envelope.DocumentTypeId, envelope.Id, envelope.DocumentId);
            if (_envelopeQueueRepository.GetByEnvelopeId(envelope.Id) != null)
                return;


            var queueItem = new OutGoingCommandEnvelopeQueueItemLocal()
                            {
                                DocumentId = envelope.DocumentId,
                                DocumentType = (DocumentType) envelope.DocumentTypeId,
                                DateInserted = DateTime.Now,
                              
                                IsEnvelopeSent = false,
                                DateSent = DateTime.Now,
                                JsonEnvelope = JsonConvert.SerializeObject(envelope, new IsoDateTimeConverter()),
                                EnvelopeId = envelope.Id,
                                
                            };
            _envelopeQueueRepository.Add(queueItem);
            if (!envelope.IsSystemEnvelope)
            {
                foreach (var commandEnvelopeItem in envelope.CommandsList)
                {
                    CommandType commandType = _resolveCommand.Get(commandEnvelopeItem.Command).CommandType;
                    _executeCommandLocally.ExecuteCommand(commandType, commandEnvelopeItem.Command);
                }
            }
        }
    }
}