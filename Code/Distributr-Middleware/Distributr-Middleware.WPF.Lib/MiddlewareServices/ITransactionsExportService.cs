using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.ThirdPartyIntegrationEntities;
using Distributr.WSAPI.Lib.Integrations;

namespace Distributr_Middleware.WPF.Lib.MiddlewareServices
{
    public interface ITransactionsDownloadService
    {
        Task DownloadSaleOrOrder(string orderExternalRef, OrderType orderType=OrderType.OutletToDistributor);

        Task<TransactionResponse> GetTransactions(string orderExternalRef,
                                                  OrderType orderType = OrderType.OutletToDistributor);
        void DownloadSalesOrOrders(OrderType orderType = OrderType.OutletToDistributor);

       
       
    }
}
