using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Commands;
using Distributr.WSAPI.Lib.Services.Routing;

namespace Distributr.WSAPI.Lib.Services.Bus
{
   public interface IRunCommandOnRequestInHostedEnvironment
    {
       void RunCommandInHostedenvironment(CommandRouteOnRequest criLocal, ICommand command);
    }
}
