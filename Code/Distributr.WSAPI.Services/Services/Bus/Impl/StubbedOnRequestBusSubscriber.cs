using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Commands;
using Distributr.WSAPI.Lib.Services.Routing;
using Distributr.WSAPI.Lib.Services.Routing.Repository;
using log4net;

namespace Distributr.WSAPI.Lib.Services.Bus.Impl
{
    public class StubbedOnRequestBusSubscriber : IBusSubscriber
    {
        ICommandRoutingOnRequestRepository _commandRoutingOnRequestRepository;
        ICommandRoutingOnRequestResolver _commandRoutingOnRequestResolver;
        IRunCommandOnRequestInHostedEnvironment _runCommandOnRequestInHostedEnvironment;


        ILog _logger = LogManager.GetLogger("Stubbed Bus on Request Subscriber");

        public StubbedOnRequestBusSubscriber(ICommandRoutingOnRequestRepository commandRoutingOnRequestRepository, ICommandRoutingOnRequestResolver commandRoutingOnRequestResolver, IRunCommandOnRequestInHostedEnvironment runCommandOnRequestInHostedEnvironment)
        {
            _commandRoutingOnRequestRepository = commandRoutingOnRequestRepository;
            _commandRoutingOnRequestResolver = commandRoutingOnRequestResolver;
            _runCommandOnRequestInHostedEnvironment = runCommandOnRequestInHostedEnvironment;
        }

        public void HandleCommand(ICommand command)
        {
            try
            {
                _logger.InfoFormat("Handle Command {4} : {0} for Document id {1} from cost centre {2} : {3}", command.CommandId, command.DocumentId, command.CommandGeneratedByCostCentreId, command.CommandGeneratedByCostCentreApplicationId, command.GetType().ToString());
                CommandRouteOnRequest commandRouteItems = _commandRoutingOnRequestResolver.GetCommand(command);
               
                if (commandRouteItems != null)
                {
                    
                    _runCommandOnRequestInHostedEnvironment.RunCommandInHostedenvironment(commandRouteItems, command);
                    _commandRoutingOnRequestRepository.Add(commandRouteItems);
                }

            }
            catch (Exception ex)
            {
                //_logger.Error(ex);
                throw ex;
            }
        }
    }
}
