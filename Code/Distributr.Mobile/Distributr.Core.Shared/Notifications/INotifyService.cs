using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.OrderDocumentEntities;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities;

namespace Distributr.Core.Notifications
{
    public interface INotifyService
    {
        void SubmitOrderSaleNotification(MainOrder order);
        void SubmitInvoiceNotification(Invoice invoice);
       void SubmitRecieptNotification(Receipt receipt);
       void SubmitCommodityPurchase(CommodityPurchaseNote purchase);
       void SubmitDispatch(DispatchNote dispatch);
       
        
        
    }
}
