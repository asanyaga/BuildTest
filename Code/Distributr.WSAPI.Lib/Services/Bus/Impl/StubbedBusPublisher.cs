using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Distributr.Core.Commands;
using Distributr.Core.Utility;
using Newtonsoft.Json;


namespace Distributr.WSAPI.Lib.Services.Bus.Impl
{
    /// <summary>
    /// Synchronous handler
    /// Not to be used in production environments
    /// </summary>
    public class StubbedBusPublisher : IBusPublisher
    {
        //IBusSubscriber _busSubscriber;
        public StubbedBusPublisher()
        {
           // _busSubscriber = subscriber;
            //throw new NotImplementedException("StubbedBusPublisher");

        }

        public void Publish(BusMessage message)
        {
           // _busSubscriber.Handle(message);
            throw new NotImplementedException("StubbedBusPublisher");

        }

        public void WrapAndPublish(ICommand command, CommandType commandType)
        {
            //var message = new BusMessage
            //{
            //    CommandType = commandType.ToString(),
            //    MessageId = command.CommandId,
            //    BodyJson = JsonConvert.SerializeObject(command),
            //    SendDateTime = "" //use default
            //};
            //Publish(message);
            throw new NotImplementedException("StubbedBusPublisher");

        }

        public void SignalComplete(ICommand command)
        {
            throw new NotImplementedException("StubbedBusPublisher");

        }

        public void Publish(EnvelopeBusMessage busMessage)
        {
            throw new NotImplementedException();
        }
    }

    public class StubbedControllerBusPublisher : IControllerBusPublisher
    {
        private IBusSubscriber _busSubscriber;

        public StubbedControllerBusPublisher(IBusSubscriber busSubscriber)
        {
            _busSubscriber = busSubscriber;
        }

        public void Publish(EnvelopeBusMessage busMessage)
        {
            _busSubscriber.Handle(busMessage);
        }
    }
}
