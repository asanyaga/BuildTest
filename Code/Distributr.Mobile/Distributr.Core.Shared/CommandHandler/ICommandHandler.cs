using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributr.Core.Commands
{
    public interface ICommandHandler<TCommand>  where TCommand : ICommand
    {
        void Execute(TCommand command);
    }
}
