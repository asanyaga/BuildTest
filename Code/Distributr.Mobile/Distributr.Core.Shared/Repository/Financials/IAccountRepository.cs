using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.FinancialEntities;
using Distributr.Core.Utility.Validation;

namespace Distributr.Core.Repository.Financials
{
    public interface IAccountRepository
    {
        Account GetById(Guid id);
        List<Account> GetAll();
        List<Account> GetByCostCentreId(Guid id);
        Account GetAccount(Guid costCentreId, AccountType accountType);
        Guid Add(Account account); //See no reason to edit for now
        DateTime GetLastTimeItemUpdated();
        ValidationResultInfo Validate(Account itemToValidate);
        void AdjustAccountBalance(Guid costCentreId, AccountType accountType, decimal amount);
        List<Account> GetByAccount(Guid accountId);
    }
}
