using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MvcContrib.Pagination;
using MvcContrib.UI.Grid;
using Distributr.HQ.Lib.ViewModels.Admin.Orders;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.Orders.Impl
{
   public class ListOrdersContainerViewModelBuilder
    {
       public IPagination<OrderViewModel> orderPagedList { get; set; }
    }
}
