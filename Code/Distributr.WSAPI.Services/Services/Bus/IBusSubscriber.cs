using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Commands;

namespace Distributr.WSAPI.Lib.Services.Bus
{
    public interface IBusSubscriber
    {
        void HandleCommand(ICommand command);
    }
}
