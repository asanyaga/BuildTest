using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.CommandHandler.DocumentCommandHandlers.Orders;
using Distributr.Core.Commands.DocumentCommands.Orders;
using Distributr.Core.Data.EF;
using log4net;

namespace Distributr.Core.Data.CommandHandlers.DocumentCommandHandlers.Orders
{
    public class AddOrderPaymentInfoCommandHandler : IAddOrderPaymentInfoCommandHandler
    {

        ILog _log = LogManager.GetLogger("AddOrderPaymentInfoCommandHandler");
        private CokeDataContext _cokeDataContext;

        public AddOrderPaymentInfoCommandHandler(CokeDataContext cokeDataContext)
        {
            _cokeDataContext = cokeDataContext;
        }

        public void Execute(AddOrderPaymentInfoCommand command)
        {
            try
            {
                var op = _cokeDataContext.tblOrderPaymentInfo.FirstOrDefault(s => s.Id == command.InfoId);
                if (op == null)
                {
                    op = new tblOrderPaymentInfo();
                    op.Id = command.InfoId;
                    _cokeDataContext.tblOrderPaymentInfo.AddObject(op);
                     op.Amount = command.Amount;
                }
               
                op.Description = command.Description;
                op.DocumentId = command.DocumentId;
                op.ConfirmedAmount += command.ConfirmedAmount;
                op.IsConfirmed = command.IsConfirmed;
                op.IsProcessed = command.IsProcessed;
                //if(op.ConfirmedAmount>=op.Amount)
                //{
                //    op.IsConfirmed = true;
                //    op.IsProcessed = true;
                //}
                //else
                //{
                //    op.IsConfirmed = false;
                //    op.IsProcessed = false;
                //}
                op.MMoneyPaymentType = command.MMoneyPaymentType;
                op.NotificationId = command.NotificationId;
                op.PaymentMode = command.PaymentModeId;
                op.PaymentRefId = command.PaymentRefId;
                op.TransactionDate = command.CommandCreatedDateTime;
                if (command.DueDate > new DateTime(2000, 1, 1))
                    op.ChequeDueDate = command.DueDate;
                op.BankCode = command.Bank;
                op.BranchCode = command.BankBranch;
                _cokeDataContext.SaveChanges();
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("Error Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
                _log.Error("AddOrderPaymentInfoCommandHandler exception", ex);
                throw;
            }

        }
    }
}
