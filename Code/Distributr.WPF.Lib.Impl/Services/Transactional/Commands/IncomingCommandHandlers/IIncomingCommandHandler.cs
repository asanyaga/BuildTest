using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Commands;
using Distributr.Core.Commands.CommandPackage;

namespace Distributr.WPF.Lib.Impl.Services.Transactional.Commands.IncomingCommandHandlers
{
    public interface IIncomingCommandHandler
    {
        void HandleCommand(string commandType, ICommand command, long commandRouteItemId);
        void HandleCommandEnvelope( CommandEnvelope command);
        void HandleUnProcessCommand(ICommand command);
    }
}
