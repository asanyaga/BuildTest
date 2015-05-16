using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributr.WSAPI.Lib.Services.Bus
{
    public interface ISubscriberMessageHandler
    {
        [Obsolete("Command Envelope Refactoring")]
        void Handle(BusMessage message);
        void Handle(EnvelopeBusMessage message);
    }
}
