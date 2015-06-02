using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.FinancialEntities;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Transactional;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Repository.Financials;
using Distributr.Core.Repository.Master.CostCentreRepositories;


namespace Distributr.Core.Workflow.FinancialWorkflow.Impl
{
    public class FinancialsWorkflow : IFinancialsWorkflow
    {
        ICostCentreRepository _costCentreRepository;
        private IAccountRepository _accountRepository;
        private IAccountTransactionRepository _accountTransactionRepository;
       
        public FinancialsWorkflow(ICostCentreRepository costCentreRepository, IAccountRepository accountRepository, IAccountTransactionRepository accountTransactionRepository)
        {
            _costCentreRepository = costCentreRepository;
            _accountRepository = accountRepository;
            _accountTransactionRepository = accountTransactionRepository;
        }

        public void AccountAdjust(Guid costCentreId, AccountType accountType, decimal amount, DocumentType documentType, Guid documentId, DateTime dateTime)
        {
          
            try
            {
               
                CostCentre cc = _costCentreRepository.GetById(costCentreId);
                if (!_accountRepository.GetByCostCentreId(costCentreId).Any(n => n.AccountType == accountType))
                {
                    Account acc = new Account
                                      {
                                          CostcentreId = costCentreId,
                                          Balance = 0,
                                          AccountType = accountType,
                                        
                                          Id=Guid.NewGuid(),
                                      };
                    _accountRepository.Add(acc);
                }

                Account acc1 = _accountRepository.GetAccount(costCentreId, accountType);

                AccountTransaction at = new AccountTransaction(Guid.NewGuid())
                                            {
                                                DateInserted = DateTime.Now,
                                                DocumentId = documentId,
                                                DocumentType = documentType,
                                                Account = acc1,
                                                Amount = amount,

                                            };

                _accountTransactionRepository.Add(at);
                _accountRepository.AdjustAccountBalance(costCentreId, accountType, amount);
            }
            catch (Exception)
            {
               // _log.Error(ex);
            }

        }
    }
}
