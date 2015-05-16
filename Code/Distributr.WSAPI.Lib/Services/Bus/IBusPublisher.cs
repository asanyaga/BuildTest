using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Commands;

namespace Distributr.WSAPI.Lib.Services.Bus
{
    /// <summary>
    /// Hack to stop circular references with StubbedBusPublisher
    /// </summary>
    public interface IControllerBusPublisher
    {
        void Publish(EnvelopeBusMessage busMessage);
    }

    public interface IBusPublisher : IControllerBusPublisher
    {
        void Publish(BusMessage busMessage);
        void WrapAndPublish(ICommand command,CommandType commandType);
        void SignalComplete(ICommand command);
        
    }
}
