using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Commands;
using Distributr.Core.Commands.DocumentCommands.Orders;

namespace Distributr.Core.CommandHandler.DocumentCommandHandlers.Orders
{
    [Obsolete]
    public interface IApproveOrderCommandHandler : ICommandHandler<ApproveOrderCommand>
    {
    }

    public interface IApproveMainOrderCommandHandler : ICommandHandler<ApproveMainOrderCommand>
    {
    }
}
