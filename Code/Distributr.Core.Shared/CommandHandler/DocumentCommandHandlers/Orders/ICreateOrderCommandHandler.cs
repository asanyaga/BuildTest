using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Commands;
using Distributr.Core.Commands.DocumentCommands;
using Distributr.Core.Commands.DocumentCommands.Orders;

namespace Distributr.Core.CommandHandler.DocumentCommandHandlers
{
    public interface ICreateOrderCommandHandler : ICommandHandler<CreateOrderCommand>
    {
    }
    public interface ICreateMainOrderCommandHandler : ICommandHandler<CreateMainOrderCommand>
    {
    }
    public interface IAddExternalDocRefCommandHandler : ICommandHandler<AddExternalDocRefCommand>
    {
    }
}
