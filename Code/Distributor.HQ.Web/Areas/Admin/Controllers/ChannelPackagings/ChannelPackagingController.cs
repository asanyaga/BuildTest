using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Distributr.Core.Utility.Validation;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.ChannelPackagingsViewModelBuilders;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Products;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.CostCenter;
using Distributr.HQ.Lib.ViewModels;
using Distributr.HQ.Lib.ViewModels.Admin.ChannelPackagingsViewModels;
using Distributr.HQ.Lib.Validation;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Transaction;
using MvcContrib.Pagination;
using Distributr.HQ.Lib.Paging;

namespace Distributr.HQ.Web.Areas.Admin.Controllers.ChannelPackagings
{
    [Authorize]
    public class ChannelPackagingController : Controller
    { 
        IChannelPackagingViewModelBuilder _channelPackagingViewModelBuilder;
        IAdminProductPackagingViewModelbuilder _productPackagingViewModelBuilder;
        IOutletTypeViewModelBuilder _outletTypeViewModelBuilder;
        IAuditLogViewModelBuilder _auditLogViewModelBuilder;

        public ChannelPackagingController(IChannelPackagingViewModelBuilder channelPackagingViewModelBuilder,
        
            IAdminProductPackagingViewModelbuilder productPackagingViewModelBuilder,IOutletTypeViewModelBuilder outletTypeViewModelBuilder, IAuditLogViewModelBuilder auditLogViewModelBuilder)
        {
            _channelPackagingViewModelBuilder = channelPackagingViewModelBuilder;
            _outletTypeViewModelBuilder = outletTypeViewModelBuilder;
            _productPackagingViewModelBuilder = productPackagingViewModelBuilder;
            _auditLogViewModelBuilder = auditLogViewModelBuilder;
        }

        public ActionResult Index()
        {

            return View();
        }

        public ActionResult ListChannelPacks(int? page, int itemsperpage = 1000)
        {
            //var packs = _channelPackagingViewModelBuilder.GetPackaging();
            //var outTypes = _outletTypeViewModelBuilder.GetAll().ToList();
            try
            {
                if (itemsperpage != null)
                {
                    ViewModelBase.ItemsPerPage = itemsperpage;
                }
                var ls = _channelPackagingViewModelBuilder.Get();

                if (TempData["msg"] != null)
                {
                    ViewBag.msg = TempData["msg"].ToString();
                    TempData["msg"] = null;
                }

                var currentPageIndex = page.HasValue ? page.Value - 1 : 0;
                var data = ls.RowItems.ToPagedList(currentPageIndex, ViewModelBase.ItemsPerPage);
               
                ViewBag.cp = data;
                return View(ls);
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }
        }

        [HttpPost]
        public ActionResult ListChannelPacks(string[] cb, int? itemsperpage)
        {
            try
            {
                if (itemsperpage != null)
                {
                    ViewModelBase.ItemsPerPage = itemsperpage.Value;
                }
                _channelPackagingViewModelBuilder.Save(cb);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Create", "Channel Packaging", DateTime.Now);
                //var packs = _channelPackagingViewModelBuilder.GetPackaging();
                //var outTypes = _outletTypeViewModelBuilder.GetAll().ToList();
                TempData["msg"] = "Saved Successfully";
                return RedirectToAction("ListChannelPacks", ViewModelBase.ItemsPerPage);
            }
            catch (DomainValidationException ve)
            {
                ValidationSummary.DomainValidationErrors(ve, ModelState);
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View("ListChannelPacks", new ChannelPackagingViewModel());
            }
            //return View(packs);
        }
    }
}
