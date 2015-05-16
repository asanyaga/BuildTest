using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Commands.DocumentCommands;


namespace Distributr.WSAPI.Lib.System
{
   public interface IQAddRetryCommandHandler
   {
       void Execute();
   }
   public interface IReRouteDocumentCommandHandler
   {
       void Execute(ReRouteDocumentCommand command);
   }
}
