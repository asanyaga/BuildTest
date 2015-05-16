using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Distributr.Core.Utility.Validation;
using Distributr.HQ.Lib.Paging;
using Distributr.HQ.Lib.Validation;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Products;
using Distributr.HQ.Lib.ViewModels;
using Distributr.HQ.Lib.ViewModels.Admin.Product;

namespace Distributr.HQ.Web.Areas.Admin.Controllers.ProductPricing
{
    [Authorize ]
    public class ReturnablesController : Controller
    { 
        IReturnablesViewModelBuilder _returnablesViewModelBuilder;
        public ReturnablesController(IReturnablesViewModelBuilder returnablesViewModelBuilder)
        {
            _returnablesViewModelBuilder = returnablesViewModelBuilder;
        }
        public ActionResult Index()
        {
            return View();
        }
         [Authorize(Roles = "RoleViewMasterData")]
        public ActionResult ListReturnables(bool? showInactive, int? page, int? itemsperpage)
        {
                if (itemsperpage != null)
                {
                    ViewModelBase.ItemsPerPage = itemsperpage.Value;
                }
            bool showinactive = false;
            if (showInactive != null)
                showinactive = (bool)showInactive;
            ViewBag.showInactive = showinactive;
            var ls = _returnablesViewModelBuilder.GetAll(showinactive);
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            return View(ls.ToPagedList(currentPageIndex,  ViewModelBase.ItemsPerPage));

        }
         [Authorize(Roles = "RoleAddMasterData")]
        public ActionResult CreateReturnables()
        {
            ViewBag.ShellsList = _returnablesViewModelBuilder.Shells();
            return View("CreateReturnables", new ReturnablesViewModel());
        }
         public ActionResult ReturnablesDetails(Guid Id)
        {
            ReturnablesViewModel returnables = _returnablesViewModelBuilder.Get(Id);
            return View(returnables);
        }
         [Authorize(Roles = "RoleModifyMasterData")]
         public ActionResult EditReturnables(Guid Id)
        {
            ViewBag.ShellsList = _returnablesViewModelBuilder.Shells();
            ReturnablesViewModel returnables = _returnablesViewModelBuilder.Get(Id);
            return View(returnables);
        }
         public ActionResult DeActivate(Guid Id)
        {
            try
            {
                _returnablesViewModelBuilder.SetInactive(Id);
                TempData["msg"] = "Successfully Deactivated";

            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;

            }
            return RedirectToAction("ListReturnables");
        }

        public JsonResult Owner(string blogName)
        {
            IList<ReturnablesViewModel> returnables = _returnablesViewModelBuilder.GetAll(true);
            return Json(returnables);
        }
        [HttpPost]
        public ActionResult CreateReturnables(ReturnablesViewModel ctpvm)
        {
            ViewBag.ShellsList = _returnablesViewModelBuilder.Shells();
            try
            {
                //ctpvm.Id = Guid.NewGuid();
                _returnablesViewModelBuilder.Save(ctpvm);
                return RedirectToAction("ListReturnables");
            }

            catch (DomainValidationException dve)
            {

                ValidationSummary.DomainValidationErrors(dve, ModelState);
                //ViewBag.msg = msg;
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;

                return View();
            }
        }
        [HttpPost]
        public ActionResult EditReturnables(ReturnablesViewModel ctpvm)
        {
            ViewBag.ShellsList = _returnablesViewModelBuilder.Shells();
            _returnablesViewModelBuilder.Save(ctpvm);
            return RedirectToAction("ListReturnables");
        }
        [HttpPost]
        public ActionResult ListReturnables(bool? showInactive, int? page, string srch, string distName, int? itemsperpage)
        {
                if (itemsperpage != null)
                {
                    ViewModelBase.ItemsPerPage = itemsperpage.Value;
                }
            string command = srch;
            bool showinactive = false;
            if (showInactive != null)
                showinactive = (bool)showInactive;
            ViewBag.showInactive = showinactive;
            var ls = _returnablesViewModelBuilder.Search(distName, showinactive);
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            if (command == "Search")
            {
                return View(ls.ToPagedList(currentPageIndex, ViewModelBase.ItemsPerPage));
            }
            else
            {
                return RedirectToAction("ListReturnables", new { showinactive = showInactive, srch = "Search", distName = "" });
            }

        }
    }
}
