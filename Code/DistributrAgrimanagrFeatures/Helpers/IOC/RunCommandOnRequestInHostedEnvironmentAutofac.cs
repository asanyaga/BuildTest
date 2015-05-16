using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Distributr.Core.Commands;
using Distributr.Core.Utility.Command;
using Distributr.WSAPI.Lib.Services.Bus;
using Distributr.WSAPI.Lib.Services.Routing;
using DistributrAgrimanagrFeatures.Helpers.TestTracing;

namespace DistributrAgrimanagrFeatures.Helpers.IOC
{
    public class RunCommandOnRequestInHostedEnvironmentAutofac : IRunCommandOnRequestInHostedEnvironment
    {
        private IResolveCommand _resolveCommand;
        private IComponentContext _componentContext;
        private string section = "RunCommandOnRequestInHostedEnvironmentAutofac";
        public RunCommandOnRequestInHostedEnvironmentAutofac(IResolveCommand resolveCommand, IComponentContext componentContext)
        {
            _resolveCommand = resolveCommand;
            _componentContext = componentContext;
        }

        public void RunCommandInHostedenvironment(CommandRouteOnRequest criLocal, ICommand command)
        {
            TI.trace(section, "RunCommandInHostedenvironment #1");
            ResolveCommandItem rci = _resolveCommand.Get(command);
            object ch = _componentContext.Resolve(rci.CommandHandlerContract);
            Type u = typeof(ICommandHandler<>);
            Type c = u.MakeGenericType(rci.Command);
            c.GetMethod("Execute").Invoke(ch, new object[] { command });
        }

        public void RunCommandInHostedenvironment(ICommand command)
        {
            TI.trace(section, "RunCommandInHostedenvironment #2");

            ResolveCommandItem rci = _resolveCommand.Get(command);
            object ch = _componentContext.Resolve(rci.CommandHandlerContract);
            Type u = typeof(ICommandHandler<>);
            Type c = u.MakeGenericType(rci.Command);
            c.GetMethod("Execute").Invoke(ch, new object[] { command });
        }
    }
}
