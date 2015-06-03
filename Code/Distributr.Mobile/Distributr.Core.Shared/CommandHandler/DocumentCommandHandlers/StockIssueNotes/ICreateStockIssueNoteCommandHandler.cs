using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Commands;
using Distributr.Core.Commands.DocumentCommands.StockIssueNotes;

namespace Distributr.Core.CommandHandler.DocumentCommandHandlers.StockIssueNotes
{
    interface ICreateStockIssueNoteCommandHandler: ICommandHandler<CreateStockIssueNoteCommand>
    {
    }
}
