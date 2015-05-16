using System;
using System.Collections.Generic;
using System.Linq;

using Distributr.Core.Commands;
using Distributr.Core.Utility;
using Distributr.Core.Utility.Command;
using Distributr.WSAPI.Lib.Services.Routing;
using StructureMap;
using log4net;

namespace Distributr.WSAPI.Lib.Services.Bus.Impl
{
    public class RunCommandInHostedEnvironment : IRunCommandInHostedEnvironment
    {
        private IResolveCommand _resolveCommand;
        ILog _log = LogManager.GetLogger("RunCommandInHostedEnvironment");
        public RunCommandInHostedEnvironment(IResolveCommand resolveCommand)
        {
            _resolveCommand = resolveCommand;
        }

        public void RunCommandInHostedenvironment(CommandRouteItem criLocal, ICommand command)
        {
            _log.InfoFormat("Running {0} : {1} --- {2}", criLocal.CommandType.ToString(), criLocal.CommandId, criLocal.JsonCommand);
            ResolveCommandItem rci = _resolveCommand.Get(command);
            object ch = ObjectFactory.GetInstance(rci.CommandHandlerContract);
            Type u = typeof(ICommandHandler<>);
            Type c = u.MakeGenericType(rci.Command);
            c.GetMethod("Execute").Invoke(ch, new object[] { command });
        }


      
    }
}
