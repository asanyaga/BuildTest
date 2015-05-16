using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Commands;

namespace Distributr.WSAPI.Lib.Services.Bus
{
    public interface IBusSubscriber
    {
        [Obsolete("Command Envelope Refactoring")]
        void Handle(BusMessage command);
        void Handle(EnvelopeBusMessage command);
    }
}
