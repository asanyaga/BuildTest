using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Commands;

namespace Distributr.WSAPI.Lib.Services.Bus.Impl
{
    public class StubbedBusPublisher : IBusPublisher
    {
        IBusSubscriber _busSubscriber;
        public StubbedBusPublisher(IBusSubscriber subscriber)
        {
            _busSubscriber = subscriber;
        }

        public void Publish(ICommand command)
        {
            _busSubscriber.HandleCommand(command);
        }

        
    }
}
