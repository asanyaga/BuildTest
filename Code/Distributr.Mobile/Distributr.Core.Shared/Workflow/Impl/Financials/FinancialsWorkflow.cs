//using System;
//using Distributr.Core.Domain.FinancialEntities;
//using Distributr.Core.Domain.Transactional.DocumentEntities;
//using Distributr.Core.Workflow.FinancialWorkflow;
//using System.Linq;
//using Distributr.WPF.Lib.Service.Financial;
//

//namespace Distributr.WPF.Lib.WorkFlow.Financials
//{
//    public class FinancialsWorkflow : IFinancialsWorkflow
//    {
//        private ICostCentreService _costCentreService;
//        private IAccountService _accountService;
//        private IAccountTransactionService _accountTransactionService;

//        public FinancialsWorkflow(ICostCentreService costCentreService, IAccountService accountService, IAccountTransactionService accountTransactionService)
//        {
//            _costCentreService = costCentreService;
//            _accountService = accountService;
//            _accountTransactionService = accountTransactionService;
//        }


//        public void AccountAdjust(Guid costCentreId, AccountType accountType, decimal amount, DocumentType documentType, Guid documentId, DateTime dateTime)
//        {
           
//            if(!_accountService.GetByCostCentreId(costCentreId).Any(n=> n.AccountType == accountType))
//            {
//                Account acc = new Account
//                                  {
//                                      AccountType = accountType,
//                                      Balance = 0,
//                                      CostcentreId = costCentreId,
//                                      Id = Guid.NewGuid(),
//                                  };
//                _accountService.Add(acc);
//            }
//            Account acc1 = _accountService.GetAccount(costCentreId, accountType);
//            AccountTransaction at = new AccountTransaction(Guid.NewGuid())
//                                        {
//                                            DateInserted = DateTime.Now,
//                                            DocumentId = documentId,
//                                            DocumentType = documentType,
//                                            Account = acc1,
//                                            Amount = amount
//                                        };
//            _accountTransactionService.Add(at);
//            _accountService.AdjustAccountBalance(costCentreId, accountType, amount);
//        }
//    }
//}

namespace Distributr.Core.Workflow.Impl.Financials
{
}