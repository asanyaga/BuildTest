using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Commands;
using Distributr.Core.Commands.DocumentCommands.InventoryReceivedNotes;

namespace Distributr.Core.CommandHandler.DocumentCommandHandlers.InventoryReceivedNotes
{
    public interface ICreateInventoryReceivedNoteCommandHandler : ICommandHandler<CreateInventoryReceivedNoteCommand>
    {
    }
}
