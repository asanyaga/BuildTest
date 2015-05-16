//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Distributr.Core.CommandHandler.DocumentCommandHandlers.Retire;
//using Distributr.Core.Commands.DocumentCommands;
//using Distributr.WSAPI.Lib.Services.Routing;
//using Distributr.WSAPI.Lib.Services.Routing.Repository;


//namespace Distributr.WSAPI.Lib.Retire
//{
//   public class RetireDocumentCommandHandler : IRetireDocumentCommandHandler
//   {
//       private ICommandRoutingOnRequestRepository _commandRoutingOnRequestRepository;

//       public RetireDocumentCommandHandler(ICommandRoutingOnRequestRepository commandRoutingOnRequestRepository)
//       {
//           _commandRoutingOnRequestRepository = commandRoutingOnRequestRepository;
//       }

//       public void Execute(RetireDocumentCommand command)
//       {
//           _commandRoutingOnRequestRepository.RetireCommands(command.DocumentId);
//       }
//    }
//}
