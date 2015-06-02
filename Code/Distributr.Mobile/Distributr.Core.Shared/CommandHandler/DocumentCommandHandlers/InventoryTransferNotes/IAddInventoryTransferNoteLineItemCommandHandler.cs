using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Commands;
using Distributr.Core.Commands.DocumentCommands.InventoryTransferNotes;

namespace Distributr.Core.CommandHandler.DocumentCommandHandlers.InventoryTransferNotes
{
    public interface IAddInventoryTransferNoteLineItemCommandHandler : ICommandHandler<AddInventoryTransferNoteLineItemCommand>
    {
    }
}
