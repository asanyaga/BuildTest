using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.FinancialEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;

namespace Distributr.Core.Workflow.FinancialWorkflow
{
    public interface IFinancialsWorkflow
    {
        void AccountAdjust(Guid costCentreId, AccountType accountType, decimal amount, DocumentType documentType,
                           Guid documentId, DateTime dateTime);
    }
}
