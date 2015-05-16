using Distributr.Core.Commands;
using Distributr.WSAPI.Lib.Services.Routing;

namespace Distributr.WSAPI.Lib.Services.Bus
{
    public interface IRunCommandInHostedEnvironment
    {
        void RunCommandInHostedenvironment(CommandRouteItem criLocal, ICommand command);
    }
}