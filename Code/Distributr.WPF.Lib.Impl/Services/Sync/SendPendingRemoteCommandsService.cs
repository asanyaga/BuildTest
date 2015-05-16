using System;
using System.Linq;
using System.Threading.Tasks;
using Distributr.Core.Commands.DocumentCommands;
using Distributr.Core.Notifications;
using Distributr.WPF.Lib.Services.Service.CommandQueues;
using Distributr.WPF.Lib.Services.Service.Sync;
using Distributr.WPF.Lib.Services.Service.WSProxies;
using Distributr.WPF.Lib.Services.Util;
using GalaSoft.MvvmLight.Messaging;
using Newtonsoft.Json;
using log4net;

namespace Distributr.WPF.Lib.Impl.Services.Sync
{
    public class SendPendingRemoteCommandsService : ISendPendingLocalCommandsService
    {

        IOutgoingCommandQueueRepository _outgoingCommandQueueRepository;
        private IOutgoingNotificationQueueRepository _outgoingNotificationQueueRepository;
        private IWebApiProxy _webApiProxy;
        private ILog _log = LogManager.GetLogger("SendPendingRemoteCommandsService");
        public SendPendingRemoteCommandsService(IOutgoingNotificationQueueRepository outgoingNotificationQueueRepository,
            IOutgoingCommandQueueRepository outgoingCommandQueueRepository, IWebApiProxy webApiProxy)
        {
            _outgoingCommandQueueRepository = outgoingCommandQueueRepository;
            _webApiProxy = webApiProxy;
            _outgoingNotificationQueueRepository = outgoingNotificationQueueRepository;
        }

        /// <summary>
        /// Send command asynchronously
        /// </summary>
        /// <param name="maxNoCommands"></param>
        /// <returns>
        ///  a/ no command sent
        ///  b/ 0 if no commands to send
        ///  c/ -1 if sending failed
        /// </returns>
        public async Task<int> SendPendingCommandsAsync(int maxNoCommands = 20)
        {
            var unsentCommands = _outgoingCommandQueueRepository.GetUnSentCommands();
            _log.InfoFormat("Unsent commands {0}", unsentCommands.Count());
            if (!unsentCommands.Any())
                return 0;
            int nocommandSent = 0;
            var commandsToSend = unsentCommands.Take(maxNoCommands);
                foreach (var command in commandsToSend)
                {
                    _log.InfoFormat("Attempting to send {0} commands", commandsToSend.Count());
                    var updatedcommand = _outgoingCommandQueueRepository.UpdateSerializedObjectWithOID(command.Id);
                    DocumentCommand c1 = JsonConvert.DeserializeObject<DocumentCommand>(updatedcommand.JsonCommand);
                    bool result = await _webApiProxy.SendCommandAsync(c1);
                    if (!result)
                    {
                        nocommandSent = -1;
                        break;
                    }
                    nocommandSent++;
                    _outgoingCommandQueueRepository.MarkCommandAsSent(updatedcommand.Id);
                }

            return nocommandSent;
        }

        public async Task<int> SendPendingNotificationAsync()
        {
            var toSend = _outgoingNotificationQueueRepository.GetUnSent();
            int noSent = 0;
            foreach (var notification in toSend)
            {
                _log.InfoFormat("Attempting to send {0} commands", toSend.Count());

                NotificationBase c1 = JsonConvert.DeserializeObject<NotificationBase>(notification.JsonDTO);
                bool result = await _webApiProxy.SendNotificationsAsync(c1);
                if (!result)
                {
                    noSent = -1;
                    break;
                }
                noSent++;
                _outgoingNotificationQueueRepository.MarkAsSent(notification.Id);
            }

            return noSent;
        }
    }
}
