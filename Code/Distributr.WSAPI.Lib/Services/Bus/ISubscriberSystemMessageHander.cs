using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributr.WSAPI.Lib.Services.Bus
{
    public interface ISubscriberSystemMessageHander
    {
         [Obsolete("Command Envelope Refactoring")]
        void Handle(BusMessage busMessage);
         void Handle(EnvelopeBusMessage busMessage);
    }
}
