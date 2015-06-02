using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Master.BankEntities;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Domain.Transactional;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;

namespace Distributr.Core.Factory.Documents
{
    public interface IReceiptFactory
    {
        Receipt Create(CostCentre documentIssuerCostCentre, Guid documentIssueCostCentreApplicationId,
                       CostCentre documentRecipientCC,
                       User DocumentIssuerUser, string DocumentReference, Guid documentParentId, Guid invoiceId,Guid paymentDocId);

        ReceiptLineItem CreateLineItem(decimal amount, string paymentRefId, string mMoneyPaymentType, string notificationId,int lineItemSequenceNo, PaymentMode paymentType, string description, Guid receiptId, bool IsConfirmed);
    }
}
