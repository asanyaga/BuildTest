using System;
using System.Collections.Generic;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Workflow;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr.WPF.Lib.WorkFlow.Orders;

namespace Distributr.WPF.Lib.Service.Utility
{
    public interface IProductPackagingSummaryService
    {
        void AddProduct(Guid productId, decimal quantity, bool isOneToOne = false, bool IsEdit = false, bool isMix = true);
        List<PackagingSummary> GetProductSummary();
        void ClearBuffer();
        void RemoveProduct(Guid productId);
        List<PackagingSummary> GetProductSummaryByProduct(Guid productId, decimal quantity, bool isOneToOne = false);
        decimal GetProductQuantityInBulk(Guid productId);
        List<ReturnableProduct> GetProductReturnables(Product product, decimal Qty);//cn
        void ClearProductReturnables();//cn
        List<PackagingSummary> GetMixedPackContainers(List<PackagingSummary> returnableProducts);//cn
        List<PackagingSummary> GetProductFinalSummary();
        void ClearMixedPackReturnables(); //cn
        ReturnableProduct GetProductBulkReturnable(Product product);//cn
        bool IsProductInStock(Guid costCentreId, Guid productId, decimal prodQtytyAddedToLineItems, decimal prodQtyToAdd, out decimal invBalance);
        List<OrderLineItemBase> OrderLineItems(List<OrderLineItemBase> lineItems); //cn
    }

    
}
