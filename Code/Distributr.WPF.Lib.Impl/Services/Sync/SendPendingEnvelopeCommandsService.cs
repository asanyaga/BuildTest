using System.Linq;
using System.Threading.Tasks;
using Distributr.Core.Commands.CommandPackage;
using Distributr.WPF.Lib.Services.Service.CommandQueues;
using Distributr.WPF.Lib.Services.Service.Sync;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr.WPF.Lib.Services.Service.WSProxies;
using log4net;
using Newtonsoft.Json;

namespace Distributr.WPF.Lib.Impl.Services.Sync
{
    public class SendPendingEnvelopeCommandsService : ISendPendingEnvelopeCommandsService
    {
        IOutgoingCommandEnvelopeQueueRepository _outgoingCommandEnvelopeQueueRepository;
        private IConfigService _configService;
        private IWebApiProxy _webApiProxy;
        private ILog _log = LogManager.GetLogger("SendPendingEnvelopeCommandsService");

        public SendPendingEnvelopeCommandsService(IOutgoingCommandEnvelopeQueueRepository outgoingCommandEnvelopeQueueRepository, IWebApiProxy webApiProxy, IConfigService configService)
        {
            _outgoingCommandEnvelopeQueueRepository = outgoingCommandEnvelopeQueueRepository;
            _webApiProxy = webApiProxy;
            _configService = configService;
        }

        public async Task<int> SendPendingEnvelopeCommandsAsync(int maxNoCommands = 100)
        {
            Config config = _configService.Load();
            var unsentenvelope = _outgoingCommandEnvelopeQueueRepository.GetUnSentEnvelope();
            _log.InfoFormat("Unsent Envelope {0}", unsentenvelope.Count());
            if (!unsentenvelope.Any())
                return 0;
            int nocommandSent = 0;
            var commandEnvelopeToSend = unsentenvelope.Take(maxNoCommands);
            foreach (var envelope in commandEnvelopeToSend)
            {
                _log.InfoFormat("Attempting to send {0} Envelope", commandEnvelopeToSend.Count());
                var updatedcommand = _outgoingCommandEnvelopeQueueRepository.UpdateSerializedObject(envelope.Id);
                CommandEnvelope c1 = JsonConvert.DeserializeObject<CommandEnvelope>(envelope.JsonEnvelope);
                c1.GeneratedByCostCentreApplicationId = config.CostCentreApplicationId;
               // c1.GeneratedByCostCentreId = config.CostCentreId;
                bool result = await _webApiProxy.SendCommandEnvelope(c1);
                if (!result)
                {
                    nocommandSent = -1;
                    break;
                }
                nocommandSent++;
                _outgoingCommandEnvelopeQueueRepository.MarkEnvelopeAsSent(updatedcommand.Id);
            }

            return nocommandSent;
        }
    }
}