using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Commands;
using Distributr.WSAPI.Lib.Services.Routing.Repository;
using Distributr.WSAPI.Lib.Services.Routing;
using Distributr.Core.CommandHandler.DocumentCommandHandlers.DispatchNotes;
using Distributr.Core.Commands.DocumentCommands.DispatchNotes;
using Distributr.Core.CommandHandler.DocumentCommandHandlers;
using Distributr.Core.CommandHandler.DocumentCommandHandlers.Orders;
using Distributr.Core.CommandHandler.DocumentCommandHandlers.InventoryTransferNotes;
using Distributr.Core.CommandHandler.DocumentCommandHandlers.AdjustmentNotes;
using Distributr.Core.Commands.DocumentCommands.Orders;
using Distributr.Core.Commands.DocumentCommands.InventoryTransferNotes;
using Distributr.Core.Commands.DocumentCommands.AdjustmentNotes;
using log4net;


namespace Distributr.WSAPI.Lib.Services.Bus.Impl
{
    public class StubbedBusSubscriber : IBusSubscriber
    {
        ICommandRoutingRepository _commandRoutingRepository;
        ICommandRoutingResolver _commandRoutingResolver;
        private IRunCommandInHostedEnvironment _runCommandInHostedEnvironment;
        ILog _logger = LogManager.GetLogger("Stubbed Bus Subscriber");

        public StubbedBusSubscriber(ICommandRoutingRepository commandRoutingRepository,
            ICommandRoutingResolver commandRoutingResolver,
            IRunCommandInHostedEnvironment runCommandInHostedEnvironment)
        {
            _commandRoutingRepository = commandRoutingRepository;
            _commandRoutingResolver = commandRoutingResolver;
            _runCommandInHostedEnvironment = runCommandInHostedEnvironment;

        }

        public void HandleCommand(ICommand command)
        {
            try
            {
                _logger.InfoFormat("Handle Command {4} : {0} for Document id {1} from cost centre {2} : {3}", command.CommandId, command.DocumentId, command.CommandGeneratedByCostCentreId, command.CommandGeneratedByCostCentreApplicationId, command.GetType().ToString());
                List<CommandRouteItem> commandRouteItems = _commandRoutingResolver.GetCommandRoutes(command);
                //save commandroute items
                commandRouteItems.ForEach(n =>
                {
                    long id = _commandRoutingRepository.Add(n);
                    n.Id = id;
                });
                //execute local hosted environment
                CommandRouteItem criLocal = _commandRoutingRepository
                    .GetById(commandRouteItems.First(n => n.CommandDestinationCostCentreApplicationId == Guid.Empty).Id);
                _runCommandInHostedEnvironment.RunCommandInHostedenvironment(criLocal, command);
                //save executed 
                _commandRoutingRepository.MardAdDeliveredAndExecuted(criLocal.Id);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }




    }
}
