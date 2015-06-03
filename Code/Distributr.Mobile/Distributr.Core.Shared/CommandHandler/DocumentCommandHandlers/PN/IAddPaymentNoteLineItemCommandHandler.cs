using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Commands.DocumentCommands.Losses;
using Distributr.Core.Commands;

namespace Distributr.Core.CommandHandler.DocumentCommandHandlers.Losses
{
    public interface IAddPaymentNoteLineItemCommandHandler : ICommandHandler<AddPaymentNoteLineItemCommand>
    {
    }
}
