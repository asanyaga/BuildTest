using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.HQ.Lib.Paging;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Orders;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Transaction;
using Distributr.HQ.Lib.ViewModels;
using Distributr.HQ.Lib.ViewModels.Admin.Orders;
using log4net;

namespace Distributr.HQ.Web.Areas.Admin.Controllers.Orders
{
    public class NewOrdersController : Controller
    {
        private IListOrdersViewModelBuilder _listOrdersViewModelBuilder;
        private IAuditLogViewModelBuilder _auditLogViewModelBuilder;
        protected static readonly ILog _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public NewOrdersController(IListOrdersViewModelBuilder listOrdersViewModelBuilder, IAuditLogViewModelBuilder auditLogViewModelBuilder)
        {
            _listOrdersViewModelBuilder = listOrdersViewModelBuilder;
            _auditLogViewModelBuilder = auditLogViewModelBuilder;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ListAllOrders(bool? showInactive, Guid? distributor, string command, string StartDate, string EndDate, string orderRef = "", int itemsperpage = 10, int page = 1)
        {
            // never remove this line below
            Session["PurchaseOrderLineItemList"] = null;
            bool showinactive = false;
            if (showInactive != null)
                showinactive = (bool)showInactive;
            ViewBag.showInactive = showinactive;
            Stopwatch stopWatch = new Stopwatch();
            if (command == "Clear")
            {
                orderRef = "";
            }
            ViewBag.searchParam = orderRef;
            try
            {
                if (itemsperpage != null)
                {
                    ViewModelBase.ItemsPerPage = itemsperpage;
                }
                stopWatch.Start();
            
                if (TempData["msg"] != null)
                {
                    ViewBag.msg = TempData["msg"].ToString();
                    ViewBag.NoOrders = TempData["msg"].ToString();
                    TempData["msg"] = null;
                }
                
                ViewBag.DistributorList = _listOrdersViewModelBuilder.GetDistributor();

                int currentPageIndex = page < 0 ? 0 : page - 1;
                int take = itemsperpage;
                int skip = currentPageIndex * take;

                var query = new QueryOrders();
                query.ShowInactive = showinactive;
                query.Skip = skip;
                query.Take = take;
                query.Name = orderRef;
                query.DocumentStatus=DocumentStatus.Confirmed;

                if (command == "Filter" || command == "Clear" || command == "Search")
                {
                    query.Distributr = distributor ?? Guid.Empty;
                    query.StartDate = DateTime.Parse(StartDate);
                    query.EndDate = DateTime.Parse(EndDate);
                }
               
               
                var orderList = _listOrdersViewModelBuilder.Query(query);
                var data = orderList.Data;
                var count = orderList.Count;
               
                stopWatch.Stop();

                TimeSpan ts = stopWatch.Elapsed;
                 
                string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                    ts.Hours, ts.Minutes, ts.Seconds,
                    ts.TotalMilliseconds);
                
                stopWatch.Reset();
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "List Order Timer", "Order Controller" + elapsedTime, DateTime.Now);
                _log.InfoFormat("List All Orders\tTime taken to get all orders" + elapsedTime);

                return View(data.ToPagedList(currentPageIndex, ViewModelBase.ItemsPerPage,count));
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }
        }
    }
}
