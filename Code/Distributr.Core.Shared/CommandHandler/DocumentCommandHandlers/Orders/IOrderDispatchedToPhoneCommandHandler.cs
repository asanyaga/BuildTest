using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Commands;
using Distributr.Core.Commands.DocumentCommands.Orders;

namespace Distributr.Core.CommandHandler.DocumentCommandHandlers.Orders
{
    public interface IOrderDispatchedToPhoneCommandHandler : ICommandHandler<DispatchToPhoneCommand>
    {
    }
    public interface IOrderDispatchApprovedLineItemsCommandHandler : ICommandHandler<OrderDispatchApprovedLineItemsCommand>
    {
    }
    
}
