using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Transactional;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Repository.Financials;

namespace Distributr.Core.Workflow.FinancialWorkflow.Impl
{
    public class PaymentTrackerWorkflow : IPaymentTrackerWorkflow
    {
        private IPaymentTrackerRepository _paymentTrackerRepository;

        public PaymentTrackerWorkflow(IPaymentTrackerRepository paymentTrackerRepository)
        {
            _paymentTrackerRepository = paymentTrackerRepository;
        }

        public void AdjustAccountBalance(Guid costCentreId, PaymentMode paymentMode, decimal amount, PaymentNoteType type)
        {
            _paymentTrackerRepository.AdjustAccountBalance(costCentreId, paymentMode, amount, type);
        }
    }
}
