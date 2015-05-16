using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Orders;
using MvcContrib.Pagination;
using Distributr.HQ.Lib.ViewModels.Admin.Orders;

namespace Distributr.HQ.Web.Areas.Orders.Controllers
{
    public class DistributorOrdersController : Controller
    {
       
        IListOrdersViewModelBuilder _listOrdersViewModelBuilder;
        public DistributorOrdersController(IListOrdersViewModelBuilder listOrdersViewModelBuilder)
        {
            _listOrdersViewModelBuilder = listOrdersViewModelBuilder;
        }
        public ActionResult ListOrders()
        {
            return View();
        }

        public ActionResult ListAllOrders(int? page)
        {
            int pageSize = 10;

            ViewBag.DistributorList = _listOrdersViewModelBuilder.GetDistributor();

            var ovm = _listOrdersViewModelBuilder.GetAllList();
            var orderPagedList = ovm.AsPagination(page ?? 1, pageSize);
            var orderListContainer = new OrderViewModel
            {
                orderPagedList = orderPagedList,

            };
            return View(orderListContainer);
        }
        [HttpPost]
        public ActionResult ListAllOrders(int distributor, int? page)
        {
            if (distributor == 0)
            {
                ViewBag.DistributorList = _listOrdersViewModelBuilder.GetDistributor();
                var ovm = _listOrdersViewModelBuilder.GetAllList();
                var orderPagedList = ovm.AsPagination(page ?? 1, 10);
                var orderListContainer = new OrderViewModel
                {
                    orderPagedList = orderPagedList,

                };
                return View(orderListContainer);
            }
            else
            {
                ViewBag.DistributorList = _listOrdersViewModelBuilder.GetDistributor();
                var ovm = _listOrdersViewModelBuilder.GetByDist(distributor);
                var orderPagedList = ovm.AsPagination(page ?? 1, 10);
                var orderListContainer = new OrderViewModel
                {
                    orderPagedList = orderPagedList,

                };
                return View(orderListContainer);
            }
        }

    }
}
