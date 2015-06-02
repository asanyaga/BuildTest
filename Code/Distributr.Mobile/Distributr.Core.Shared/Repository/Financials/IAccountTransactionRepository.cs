using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.FinancialEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Utility.Validation;

namespace Distributr.Core.Repository.Financials
{
    public interface IAccountTransactionRepository
    {
        AccountTransaction GetById(Guid id);
        Guid Add(AccountTransaction accountTransaction);
        List<AccountTransaction> GetByCostCentre(Guid costCentreId);
        List<AccountTransaction> GetByCostCentre(Guid costCentreId, Account account, DocumentType? documentType);
        List<AccountTransaction> GetByCostCentre(Guid costCentreId, Account account, DocumentType? documentType, DateTime startDate, DateTime endDate);
        List<AccountTransaction> GetByDate(DateTime startDate, DateTime endDate);
        ValidationResultInfo Validate(AccountTransaction itemToValidate);
        List<AccountTransaction> GetByAccount(Guid accountId);
        //TODO Summarise account balances by cc/account
    }
}
