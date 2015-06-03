using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Commands;
using Distributr.Core.Commands.DocumentCommands.CreditNotes;

namespace Distributr.Core.CommandHandler.DocumentCommandHandlers.CreditNotes
{
    public interface IConfirmCreditNoteCommandHandler: ICommandHandler<ConfirmCreditNoteCommand>
    {
    }
}
