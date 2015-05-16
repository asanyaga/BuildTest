using Distributr.Core.Commands;

namespace Distributr.WPF.Lib.Services.Service.Transactional.Commands.CommandRouting
{
    public interface IExecuteCommandLocally
    {
        void ExecuteCommand(CommandType commandType, ICommand command);
    }
}
