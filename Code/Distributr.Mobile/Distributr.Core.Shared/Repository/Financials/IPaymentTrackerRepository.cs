using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.FinancialEntities;
using Distributr.Core.Domain.Transactional;
using Distributr.Core.Domain.Transactional.DocumentEntities;

namespace Distributr.Core.Repository.Financials
{
    public interface IPaymentTrackerRepository
    {
        
        List<PaymentTracker> GetAll();
        void AdjustAccountBalance(Guid costCentreId, PaymentMode paymentMode, decimal amount,PaymentNoteType type);
        List<PaymentTracker> GetByCostCentre(Guid costCentreId);
        void Save(PaymentTracker paymentTracker );
    }
}
