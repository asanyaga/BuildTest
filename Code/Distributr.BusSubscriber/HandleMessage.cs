using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.WSAPI.Lib.Services.Bus;
using StructureMap;
using log4net;

namespace Distributr.BusSubscriber
{
    public class HandleMessage : IHandleMessage
    {
        private IBusSubscriber _subscriber;
        public HandleMessage(IBusSubscriber subscriber)
        {
            _log.Debug("ctor -=========================================");
            _subscriber = subscriber;
        }
        private static readonly ILog _log = LogManager.GetLogger("Subscription Logger - Handle Message");
        public void Handle(BusMessage message)
        {
            try
            {
                _subscriber.Handle(message);
            }
            catch (Exception ex)
            {
                _log.Error("Failed to handle message " + message.MessageId, ex);
            }

        }

        public void Handle(EnvelopeBusMessage message)
        {
            try
            {
                _subscriber.Handle(message);
            }
            catch (Exception ex)
            {
                _log.Error("Failed to handle message " + message.MessageId, ex);
            }
        }
    }

    public interface IHandleMessage
    {
        [Obsolete("Command Envelope Refactoring")]
        void Handle(BusMessage message);
        void Handle(EnvelopeBusMessage message);
    }
}
