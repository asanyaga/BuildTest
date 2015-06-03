//using System;
//using Distributr.Core.Domain.Transactional;
//using Distributr.Core.Domain.Transactional.DocumentEntities;
//using Distributr.Core.Workflow.FinancialWorkflow;
//using Distributr.WPF.Lib.Service.Financial;

//namespace Distributr.WPF.Lib.WorkFlow.Financials
//{
//    public class PaymentTrackerWorkflow : IPaymentTrackerWorkflow
//    {
//        private IPaymentTrackerService _paymentTrackerService;

//        public PaymentTrackerWorkflow(IPaymentTrackerService paymentTrackerService)
//        {
//            _paymentTrackerService = paymentTrackerService;
//        }

//        public void AdjustAccountBalance(Guid costCentreId, PaymentMode paymentMode, decimal amount, PaymentNoteType type)
//        {
//            _paymentTrackerService.AdjustAccountBalance(costCentreId, paymentMode, amount, type);
//        }
//    }
//}

namespace Distributr.Core.Workflow.Impl.Financials
{
}