using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Distributr.Core.Commands;
using Distributr.Core.Utility;
using EasyNetQ;
using EasyNetQ.Topology;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using RabbitMQ.Client;
using log4net;

namespace Distributr.WSAPI.Lib.Services.Bus.EasyNetQ
{
    public class EasyNetQBusPublisher: IBusPublisher
    {
        
        private IAdvancedBus _bus;
        private string _exName;
        private ILog _log = LogManager.GetLogger("EasyNetQBusPublisher");
        public EasyNetQBusPublisher( string mqname, IAdvancedBus advancedBus)
        {
            _bus = advancedBus;
            _exName = "EX" + mqname;
        }

        public void Publish(BusMessage message)
        {
            var exchange = Exchange.DeclareTopic(_exName);
            using (var channel = _bus.OpenPublishChannel())
            {
                channel.Publish<BusMessage>(exchange, "", new Message<BusMessage>(message));
                _log.InfoFormat("Message for command id {0} published on exchange {1}", message.MessageId, _exName);
                
               
            }
        }

        public void WrapAndPublish(ICommand command, CommandType commandType)
        {
            var message = new BusMessage
            {
                CommandType = commandType.ToString(),
                MessageId = command.CommandId,
                BodyJson = JsonConvert.SerializeObject(command, new IsoDateTimeConverter()),
                SendDateTime = "" //use default
            };
            Publish(message);
        }

        /// <summary>
        /// This is so a testing framework can be signalled when a command is completed processing on the bus
        /// </summary>
        /// <param name="command"></param>
        public void SignalComplete(ICommand command)
        {
            try
            {
                
                if (ConfigurationManager.AppSettings["MQSignalComplete"] != null &&
                ConfigurationManager.AppSettings["MQSignalComplete"] == "true")
                {
                    _log.InfoFormat("Signal complete for commandid {0}" , command.CommandId );
                    string testExchange = "MQExchange1";
                    SignalCompleteMessage message = new SignalCompleteMessage
                    {
                        CommandId = command.CommandId.ToString(),
                        DocumentId = command.DocumentId.ToString()
                    };
                    var exchange = Exchange.DeclareTopic(testExchange);
                    using (var channel = _bus.OpenPublishChannel())
                    {
                        channel.Publish<SignalCompleteMessage>(exchange, "", new Message<SignalCompleteMessage>(message));
                    }
                }
            }
            catch(Exception ex)
            {
                _log.Error("Signal complete failed ", ex );
            }


        }

        public void Publish(EnvelopeBusMessage busMessage)
        {
            var exchange = Exchange.DeclareTopic(_exName);
            using (var channel = _bus.OpenPublishChannel())
            {
                channel.Publish<EnvelopeBusMessage>(exchange, "", new Message<EnvelopeBusMessage>(busMessage));
                _log.InfoFormat("Message for Envelopr id {0} published on exchange {1}", busMessage.MessageId, _exName);

            }
        }
    }
   
}
