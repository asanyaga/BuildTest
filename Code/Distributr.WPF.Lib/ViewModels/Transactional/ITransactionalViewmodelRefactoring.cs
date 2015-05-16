using System;
using System.ComponentModel;
using System.Linq;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using Distributr.Core.Repository.Transactional.DocumentRepositories.ReceiptInventories;
using Distributr.WPF.Lib.Services.Service.Utility;
using StructureMap;

namespace Distributr.WPF.Lib.ViewModels.Transactional
{
    /*
     *  Not sure where to put these shared items for now 
     *  so they can live here 
     */

    public interface ITransactionalViewmodelRefactoring 
    {
        bool LineItemIsConfirmed(Receipt receipt, Guid lineItemId, out decimal unconfirmedAmnt);
    }

    public class TransactionalViewmodelRefactoring :DistributrViewModelBase, ITransactionalViewmodelRefactoring
    {

        public bool LineItemIsConfirmed(Receipt receipt, Guid lineItemId, out decimal unconfirmedAmnt)
        {
            using (StructureMap.IContainer c =NestedContainer)
            {
                IReceiptRepository _receiptService = Using<IReceiptRepository>(c);
                bool isConfirmed = false;
                unconfirmedAmnt = 0m;
                var paymentDocLineItem = receipt.LineItems.FirstOrDefault(n => n.Id == lineItemId);

                if (paymentDocLineItem.LineItemType == OrderLineItemType.PostConfirmation)
                {
                    return true;
                }

                var invoiceReceipts = _receiptService.GetByInvoiceId(receipt.InvoiceId);
                if (invoiceReceipts.Count > 0)
                {
                    var paymentDoc =
                        invoiceReceipts.FirstOrDefault(
                            n => n.PaymentDocId == Guid.Empty && n.LineItems.Any(l => l.Id == lineItemId));
                    if (paymentDoc != null)
                    {
                        var childLineItems =
                            invoiceReceipts.Where(n => n.Id != paymentDoc.Id)
                                           .SelectMany(n => n.LineItems.Where(l => l.PaymentDocLineItemId == lineItemId));
                        decimal totalConfirmed = childLineItems.Sum(n => n.Value);

                        unconfirmedAmnt = paymentDocLineItem.Value - totalConfirmed;
                        if (unconfirmedAmnt < 0)
                            unconfirmedAmnt = 0;

                        if (totalConfirmed >= paymentDocLineItem.Value)
                        {
                            isConfirmed = true;
                        }
                    }
                }

                return isConfirmed;
            }
            
        }
    }
}
