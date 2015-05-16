using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Commands;
using Distributr.Core.Commands.DocumentCommands;
using Distributr.WSAPI.Lib.Services.CommandAudit;
using Distributr.WSAPI.Lib.System.Utility;
using Distributr.WSAPI.Lib.System.Utility.CCAudit;

namespace Distributr.WSAPI.Lib.Services.Bus.Impl
{
    public class SubscriberCommandExecutionGuard : ISubscriberCommandExecutionGuard
    {
        private ICommandProcessingAuditRepository _commandProcessingAuditRepository;
        private ICCAuditRepository _auditRepository;

        public SubscriberCommandExecutionGuard(ICommandProcessingAuditRepository commandProcessingAuditRepository, ICCAuditRepository auditRepository)
        {
            _commandProcessingAuditRepository = commandProcessingAuditRepository;
            _auditRepository = auditRepository;
        }

        public void CanExecute(ICommand command)
        {
            bool canExecute = true;
            string msg = "";
            // check if Database is up===========================================================


            // check if command is executed-----------------------------------------------------
            bool isCommandExecuted = _commandProcessingAuditRepository.IsCommandExecuted(command.CommandId);
            if (isCommandExecuted)
            {
                msg = "Command Already Executed";
                Audit("Guard", msg, "Failed", command.CommandGeneratedByCostCentreId);
                throw new CommandAlreadyExecutedException(msg);
            }
            if (command is CreateCommand)
            {

            }
            // check if Create command is executed-----------------------------------------------------
            if (command is AfterCreateCommand)
            {
                canExecute = _commandProcessingAuditRepository.IsCreateCommandExecuted(command.DocumentId);
                if (!canExecute)
                {
                    msg = "Create Command not Executed";
                    Audit("Guard", msg, "Failed", command.CommandGeneratedByCostCentreId);
                    throw new MarkForRetryException(msg);
                }
            }
            // check if all Addlineitem command have been executed-----------------------------------------------------
            if (command is ConfirmCommand)
            {
                canExecute = _commandProcessingAuditRepository.IsAddCommandExecuted(command.DocumentId);
                if (!canExecute)
                {
                    msg = "All Add LineItem Command are yet to be completed";
                    Audit("Guard", msg, "Failed", command.CommandGeneratedByCostCentreId);
                    throw new MarkForRetryException(msg);
                }
            }
            if (command is CloseCommand)
            {
                canExecute = _commandProcessingAuditRepository.IsConfirmExecuted(command.DocumentId);
                if (!canExecute)
                {
                    msg = "Confirm command is yet to be completed";
                    Audit("Guard", msg, "Failed", command.CommandGeneratedByCostCentreId);
                    throw new MarkForRetryException(msg);
                }
            }

        }

        private void Audit(string action, string info, string results, Guid costcentreId)
        {
            _auditRepository.Add(new CCAuditItem
            {
                Action = action.ToString(),
                CostCentreId = costcentreId,
                DateInsert = DateTime.Now,
                Id = Guid.NewGuid(),
                Info = info,
                Result = results,
            });
        }

    }
    public class ServiceException:Exception
    {
        public ServiceException(string message) : base(message)
        {
        }
    }

    public class MarkForRetryException : Exception
    {
        public MarkForRetryException(string message)
            : base(message)
        {
        }
    }
    public class CommandAlreadyExecutedException : Exception
    {
        public CommandAlreadyExecutedException(string message)
            : base(message)
        {
        }
    }

}
