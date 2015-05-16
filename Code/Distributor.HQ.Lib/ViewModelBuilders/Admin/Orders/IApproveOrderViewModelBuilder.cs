using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.HQ.Lib.ViewModels.Admin.Orders;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.Orders
{
    public interface IApproveOrderViewModelBuilder
    {
        ApproveOrderViewModel Get(Guid orderId);
        ApproveOrderViewModel Find(Guid orderId);
        OrderEditLineItemViewModel GetLineItem(Guid productId);
        OrderAddLineItemViewModel GetAddLineItem(Guid orderId);
        void Approve(Guid documentId);
        void Reject(Guid documentId,string rejectReason);
        void AddUpdateLineItems(Guid productId, decimal qty, bool isNew, bool isBulk);
        void RemoveLineItem(Guid productId);
    }
}
