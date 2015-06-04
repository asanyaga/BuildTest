using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Transactional;
using Distributr.Core.Domain.Transactional.DocumentEntities;

namespace Distributr.Core.Workflow.FinancialWorkflow
{
    public interface IPaymentTrackerWorkflow
    {
        void AdjustAccountBalance(Guid costCentreId, PaymentMode paymentMode, decimal amount, PaymentNoteType type);
    }
}
