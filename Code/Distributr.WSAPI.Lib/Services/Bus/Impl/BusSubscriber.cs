using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;

namespace Distributr.WSAPI.Lib.Services.Bus.Impl
{
    public class MainBusSubscriber : IBusSubscriber
    {
        private ISubscriberMessageHandler _subscriberMessageHandler;
        private ISubscriberSystemMessageHander _subscriberSystemMessageHander;

        public MainBusSubscriber(ISubscriberMessageHandler subscriberMessageHandler, ISubscriberSystemMessageHander subscriberSystemMessageHander)
        {
            _logger.Debug("ctor ------------------------");
            _subscriberMessageHandler = subscriberMessageHandler;
            _subscriberSystemMessageHander = subscriberSystemMessageHander;
        }

        ILog _logger = LogManager.GetLogger("BusSubscriber");

        public void Handle(BusMessage message)
        {
           _logger.InfoFormat("Received message type {0} message id {1} ", message.CommandType, message.MessageId);
            if(message.IsSystemMessage)
                _subscriberSystemMessageHander.Handle(message);
            else
                _subscriberMessageHandler.Handle(message);
        }

        public void Handle(EnvelopeBusMessage message)
        {
            _logger.InfoFormat("Received message type {0} message id {1} ", message.DocumentTypeId, message.MessageId);
            if (message.IsSystemMessage)
                _subscriberSystemMessageHander.Handle(message);
            else
                _subscriberMessageHandler.Handle(message);
        }
    }
}
