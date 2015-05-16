using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.HQ.Lib.ViewModels.Admin.Orders;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.Orders
{
    public interface IPurchaseOrderViewModelBuilder
    {
        
        void Save(PurchaseOrderViewModel model);
        
    }
}
