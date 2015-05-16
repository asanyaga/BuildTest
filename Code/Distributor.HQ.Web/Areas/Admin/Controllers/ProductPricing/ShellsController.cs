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
    public class ShellsController : Controller
    { 
        IShellsViewModelBuilder _shellsViewModelBuilder;
        public ShellsController(IShellsViewModelBuilder shellsViewModelBuilder)
        {
            _shellsViewModelBuilder = shellsViewModelBuilder;
        }
        public ActionResult Index()
        {
            return View();
        }
        [Authorize(Roles = "RoleViewMasterData")]
        public ActionResult ListShells(bool? showInactive, int? page, int? itemsperpage)
        {
                if (itemsperpage != null)
                {
                    ViewModelBase.ItemsPerPage = itemsperpage.Value;
                }
            bool showinactive = false;
            if (showInactive != null)
                showinactive = (bool)showInactive;
            ViewBag.showInactive = showinactive;
            var ls = _shellsViewModelBuilder.GetAll(showinactive);
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            return View(ls.ToPagedList(currentPageIndex, ViewModelBase.ItemsPerPage));

        }
        [Authorize(Roles = "RoleAddMasterData")]
        public ActionResult CreateShell()
        {
            return View("CreateShell", new ShellsViewModel());
        }
        public ActionResult ShellDetails(Guid Id)
        {
            ShellsViewModel shells = _shellsViewModelBuilder.Get(Id);
            return View(shells);
        }
         [Authorize(Roles = "RoleModifyMasterData")]
        public ActionResult EditShell(Guid Id)
        {
            ShellsViewModel shell = _shellsViewModelBuilder.Get(Id);
            return View(shell);
        }
         public ActionResult DeActivate(Guid Id)
        {
            try
            {
                _shellsViewModelBuilder.SetInactive(Id);
                TempData["msg"] = "Successfully Deactivated";

            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;

            }
            return RedirectToAction("ListShells");
        }

        public JsonResult Owner(string blogName)
        {
            IList<ShellsViewModel> shell = _shellsViewModelBuilder.GetAll(true);
            return Json(shell);
        }
        [HttpPost]
        public ActionResult CreateShell(ShellsViewModel ctpvm)
        {
            
            try
            {
                _shellsViewModelBuilder.Save(ctpvm);
                return RedirectToAction("ListShells");
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
        public ActionResult EditShell(ShellsViewModel ctpvm)
        {

            _shellsViewModelBuilder.Save(ctpvm);
            return RedirectToAction("ListShells");
        }
        [HttpPost]
        public ActionResult ListShells(bool? showInactive, int? page, string srch, string distName, int? itemsperpage)
        {
            string command = srch;
                if (itemsperpage != null)
                {
                    ViewModelBase.ItemsPerPage = itemsperpage.Value;
                }
            bool showinactive = false;
            if (showInactive != null)
                showinactive = (bool)showInactive;
            ViewBag.showInactive = showinactive;
            var ls = _shellsViewModelBuilder.Search(distName, showinactive);
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            if (command == "Search")
            {
                return View(ls.ToPagedList(currentPageIndex, ViewModelBase.ItemsPerPage));
            }
            else
            {
                return RedirectToAction("ListShells", new { showinactive = showInactive, srch = "Search", distName = "" });
            }

        }
    }
}
