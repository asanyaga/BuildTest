using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Distributr.Core.Domain.Transactional.DocumentEntities.OrderDocumentEntities;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.OrderRepositories;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.HQ.Lib.Paging;
using Distributr.HQ.Lib.ViewModels;

namespace Distributr.HQ.Web.Areas.OutletManager.Controllers.OrdersControllers
{
    public class OutletOrderController : Controller
    {
        IOutletMainOrderRepository _outletMainOrderRepository;
        private IMainOrderRepository _mainOrderRepository;
        private IUserRepository _userRepository;
        public OutletOrderController(IOutletMainOrderRepository outletMainOrderRepository, IUserRepository userRepository, IMainOrderRepository mainOrderRepository)
        {
            _outletMainOrderRepository = outletMainOrderRepository;
            _userRepository = userRepository;
            _mainOrderRepository = mainOrderRepository;
        }

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ListAllOrders(string srchParam, string startDate, string endDate, int page = 1, int itemsperpage = 10)
        {
            var user = _userRepository.GetUser(this.User.Identity.Name);
            DateTime baseDate = DateTime.Today;
            if(itemsperpage != null)
            {
                ViewModelBase.ItemsPerPage = itemsperpage;
            }
            
            ViewBag.srchText = srchParam;
            ViewBag.itemsPerPage = itemsperpage;
            int currentPageIndex = page < 0 ? 0 : page - 1;
            int take = itemsperpage;
            int skip = currentPageIndex*take;

            var query = new QueryOutletOrder() {Skip = skip, Take = take, Name = srchParam};
            query.OutletId = user != null ? user.CostCentre : Guid.Empty;
            query.From = startDate == null ? baseDate.AddDays(1 - baseDate.Day) : Convert.ToDateTime(startDate);
            query.To = endDate == null
                           ? baseDate.AddDays(1 - baseDate.Day).AddMonths(1).AddSeconds(-1)
                           : Convert.ToDateTime(endDate).AddDays(1);

            ViewBag.startDate = query.From;
            ViewBag.endDate = query.To;

            var result = _outletMainOrderRepository.ListAllOrders(query);
            var data = result.Data;
            var count = result.Count;

            return View(data.ToPagedList(currentPageIndex, ViewModelBase.ItemsPerPage, count));
        }

        public ActionResult OrderDetails(Guid orderId,string srchParam, int page = 1, int itemsperpage = 10)
        {
            if (itemsperpage != null)
            {
                ViewModelBase.ItemsPerPage = itemsperpage;
            }
            var order = _mainOrderRepository.GetById(orderId);

            ViewBag.srchText = srchParam;
            ViewBag.itemsPerPage = itemsperpage;
            int currentPageIndex = page < 0 ? 0 : page - 1;
            int take = itemsperpage;
            int skip = currentPageIndex * take;

            return View(order);
        }

        public ActionResult OrdersPendingApproval(string srchParam, string startDate, string endDate, int page = 1, int itemsperpage = 10)
        {
            var user = _userRepository.GetUser(this.User.Identity.Name);
            DateTime baseDate = DateTime.Today;
            if (itemsperpage != null)
            {
                ViewModelBase.ItemsPerPage = itemsperpage;
            }

            ViewBag.srchText = srchParam;
            ViewBag.itemsPerPage = itemsperpage;
            int currentPageIndex = page < 0 ? 0 : page - 1;
            int take = itemsperpage;
            int skip = currentPageIndex * take;

            var query = new QueryOutletOrder() { Skip = skip, Take = take, Name = srchParam };
            query.OutletId = user != null ? user.CostCentre : Guid.Empty;
            query.From = startDate == null ? baseDate.AddDays(1 - baseDate.Day) : Convert.ToDateTime(startDate);
            query.To = endDate == null
                           ? baseDate.AddDays(1 - baseDate.Day).AddMonths(1).AddSeconds(-1)
                           : Convert.ToDateTime(endDate).AddDays(1);

            ViewBag.startDate = query.From;
            ViewBag.endDate = query.To;

            var result = _outletMainOrderRepository.PendingApprovalQuery(query);
            var data = result.Data;
            var count = result.Count;

            return View(data.ToPagedList(currentPageIndex, ViewModelBase.ItemsPerPage, count));
        }

        public ActionResult OrdersPendingDispatch(string srchParam, string startDate, string endDate, int page = 1, int itemsperpage = 10)
        {
            var user = _userRepository.GetUser(this.User.Identity.Name);
            DateTime baseDate = DateTime.Today;
            if (itemsperpage != null)
            {
                ViewModelBase.ItemsPerPage = itemsperpage;
            }

            ViewBag.srchText = srchParam;
            ViewBag.itemsPerPage = itemsperpage;
            int currentPageIndex = page < 0 ? 0 : page - 1;
            int take = itemsperpage;
            int skip = currentPageIndex * take;

            var query = new QueryOutletOrder() { Skip = skip, Take = take, Name = srchParam };
            query.OutletId = user != null ? user.CostCentre : Guid.Empty;
            query.From = startDate == null ? baseDate.AddDays(1 - baseDate.Day) : Convert.ToDateTime(startDate);
            query.To = endDate == null
                           ? baseDate.AddDays(1 - baseDate.Day).AddMonths(1).AddSeconds(-1)
                           : Convert.ToDateTime(endDate).AddDays(1);

            ViewBag.startDate = query.From;
            ViewBag.endDate = query.To;

            var result = _outletMainOrderRepository.PendingDispatchQuery(query);
            var data = result.Data;
            var count = result.Count;

            return View(data.ToPagedList(currentPageIndex, ViewModelBase.ItemsPerPage, count));
        }

        public ActionResult OutstandingOrders(string srchParam, string startDate, string endDate, int page = 1, int itemsperpage = 10)
        {
            var user = _userRepository.GetUser(this.User.Identity.Name);
            DateTime baseDate = DateTime.Today;
            if (itemsperpage != null)
            {
                ViewModelBase.ItemsPerPage = itemsperpage;
            }

            ViewBag.srchText = srchParam;
            ViewBag.itemsPerPage = itemsperpage;
            int currentPageIndex = page < 0 ? 0 : page - 1;
            int take = itemsperpage;
            int skip = currentPageIndex * take;

            var query = new QueryOutletOrder() { Skip = skip, Take = take, Name = srchParam };
            query.OutletId = user != null ? user.CostCentre : Guid.Empty;
            query.From = startDate == null ? baseDate.AddDays(1 - baseDate.Day) : Convert.ToDateTime(startDate);
            query.To = endDate == null
                           ? baseDate.AddDays(1 - baseDate.Day).AddMonths(1).AddSeconds(-1)
                           : Convert.ToDateTime(endDate).AddDays(1);

            ViewBag.startDate = query.From;
            ViewBag.endDate = query.To;

            var result = _outletMainOrderRepository.OutstandingPaymentQuery(query);
            var data = result.Data;
            var count = result.Count;

            return View(data.ToPagedList(currentPageIndex, ViewModelBase.ItemsPerPage, count));
        }

        public ActionResult RejectedOrders(string srchParam, string startDate, string endDate, int page = 1, int itemsperpage = 10)
        {
            var user = _userRepository.GetUser(this.User.Identity.Name);
            DateTime baseDate = DateTime.Today;
            if (itemsperpage != null)
            {
                ViewModelBase.ItemsPerPage = itemsperpage;
            }

            ViewBag.srchText = srchParam;
            ViewBag.itemsPerPage = itemsperpage;
            int currentPageIndex = page < 0 ? 0 : page - 1;
            int take = itemsperpage;
            int skip = currentPageIndex * take;

            var query = new QueryOutletOrder() { Skip = skip, Take = take, Name = srchParam };
            query.OutletId = user != null ? user.CostCentre : Guid.Empty;
            query.From = startDate == null ? baseDate.AddDays(1 - baseDate.Day) : Convert.ToDateTime(startDate);
            query.To = endDate == null
                           ? baseDate.AddDays(1 - baseDate.Day).AddMonths(1).AddSeconds(-1)
                           : Convert.ToDateTime(endDate).AddDays(1);

            ViewBag.startDate = query.From;
            ViewBag.endDate = query.To;

            var result = _outletMainOrderRepository.RejectedQuery(query);
            var data = result.Data;
            var count = result.Count;

            return View(data.ToPagedList(currentPageIndex, ViewModelBase.ItemsPerPage, count));
        }

        public ActionResult DispatchedOrders(string srchParam, string startDate, string endDate, int page = 1, int itemsperpage = 10)
        {
            var user = _userRepository.GetUser(this.User.Identity.Name);
            DateTime baseDate = DateTime.Today;
            if (itemsperpage != null)
            {
                ViewModelBase.ItemsPerPage = itemsperpage;
            }

            ViewBag.srchText = srchParam;
            ViewBag.itemsPerPage = itemsperpage;
            int currentPageIndex = page < 0 ? 0 : page - 1;
            int take = itemsperpage;
            int skip = currentPageIndex * take;

            var query = new QueryOutletOrder() { Skip = skip, Take = take, Name = srchParam };
            query.OutletId = user != null ? user.CostCentre : Guid.Empty;
            query.From = startDate == null ? baseDate.AddDays(1 - baseDate.Day) : Convert.ToDateTime(startDate);
            query.To = endDate == null
                           ? baseDate.AddDays(1 - baseDate.Day).AddMonths(1).AddSeconds(-1)
                           : Convert.ToDateTime(endDate).AddDays(1);

            ViewBag.startDate = query.From;
            ViewBag.endDate = query.To;

            var result = _outletMainOrderRepository.DispatchedQuery(query);
            var data = result.Data;
            var count = result.Count;
            //edit

            return View(data.ToPagedList(currentPageIndex, ViewModelBase.ItemsPerPage, count));
        }
    }
}
