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
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.UserRepositories;

namespace Distributr.Core.Factory.Documents.Impl
{
   public class ReceiptFactory : BaseDocumentFactory, IReceiptFactory
    {
       public ReceiptFactory(ICostCentreRepository costCentreRepository, IUserRepository userRepository)
           : base(costCentreRepository, userRepository)
       {

       }

       public Receipt Create(CostCentre documentIssuerCostCentre, Guid documentIssueCostCentreApplicationId, CostCentre documentRecipientCC, User documentIssuerUser, string documentReference, Guid documentParentId, Guid invoiceId, Guid paymentDocId)
       {
           Guid id = Guid.NewGuid();
           Receipt doc = DocumentPrivateConstruct<Receipt>(id);
           doc.DocumentType = DocumentType.Receipt;
           doc.InvoiceId = invoiceId;
           doc.PaymentDocId = paymentDocId;
           if (documentParentId == null || documentParentId == Guid.Empty)
               doc.DocumentParentId = id;
           else
               doc.DocumentParentId = documentParentId;
           Map(doc, documentIssuerCostCentre, documentIssueCostCentreApplicationId, documentRecipientCC, documentIssuerUser, documentReference, null, null);
           SetDefaultDates(doc);
           doc.EnableAddCommands();
           return doc;
       }

       public ReceiptLineItem CreateLineItem(decimal amount, string paymentRefId, string mMoneyPaymentType, string notificationId,int lineItemSequenceNo, PaymentMode paymentType, string description, Guid receiptId, bool IsConfirmed)
       {
           var li = ReceiptLineItemPrivateConstruct<ReceiptLineItem>(Guid.NewGuid());
           li.Value = amount;
           li.Description = description;
           li.PaymentRefId = paymentRefId;
           li.LineItemSequenceNo = 0;
           li.PaymentType = paymentType;
           li.MMoneyPaymentType = mMoneyPaymentType;
           li.NotificationId = notificationId;
           li.LineItemType = IsConfirmed
                                 ? OrderLineItemType.PostConfirmation
                                 : OrderLineItemType.DuringConfirmation;
           if(receiptId !=Guid.Empty)
           {
                   li.PaymentDocLineItemId = receiptId;
           }
           return li;
       }
    }
}
