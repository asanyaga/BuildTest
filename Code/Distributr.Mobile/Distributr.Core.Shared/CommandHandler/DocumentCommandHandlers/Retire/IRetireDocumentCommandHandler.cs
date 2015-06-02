using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Commands;
using Distributr.Core.Commands.DocumentCommands;

namespace Distributr.Core.CommandHandler.DocumentCommandHandlers.Retire
{
    public interface IRetireDocumentCommandHandler : ICommandHandler<RetireDocumentCommand>
    {
    }
}
