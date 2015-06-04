using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Commands;
using Distributr.Core.Commands.DocumentCommands.DisbursementNotes;

namespace Distributr.Core.CommandHandler.DocumentCommandHandlers.DisbursementNotes
{
    public interface IAddDisbursementNoteLineItemCommandHandler : ICommandHandler<AddDisbursementNoteLineItemCommand>
    {
    }
}
