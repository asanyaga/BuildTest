using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Commands;
using Distributr.Core.Commands.DocumentCommands.DispatchNotes;

namespace Distributr.Core.CommandHandler.DocumentCommandHandlers.DispatchNotes
{
    public interface IConfirmDispatchNoteCommandHandler : ICommandHandler<ConfirmDispatchNoteCommand>
    {
    }
}
