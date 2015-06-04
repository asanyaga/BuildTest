using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Commands;
using Distributr.Core.Commands.DocumentCommands.Receipts;

namespace Distributr.Core.CommandHandler.DocumentCommandHandlers.Receipts
{
    public interface IConfirmReceiptCommandHandler : ICommandHandler<ConfirmReceiptCommand>
    {
    }
}
