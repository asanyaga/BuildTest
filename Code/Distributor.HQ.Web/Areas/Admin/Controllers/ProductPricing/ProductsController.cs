using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Distributr.HQ.Lib.ViewModelBuilders;
using Distributr.HQ.Lib.ViewModels.Admin;
using log4net;
using System.Reflection;

namespace Distributr.HQ.Web.Areas.Admin.Controllers
{
    [Authorize ]
    public class ProductsController : Controller
    {

        IAdminProductViewModelBuilder _adminProductViewModelBuilder;
        protected static readonly ILog _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public ProductsController(IAdminProductViewModelBuilder adminProductViewModelBuilder)
        {
            _adminProductViewModelBuilder = adminProductViewModelBuilder;
        }

        public ActionResult Index()
        {
            return View();
        }

         [Authorize(Roles = "RoleViewMasterData")]
        public ActionResult ListProductBrands()
        {
            IList<AdminProductBrandViewModel> brands = _adminProductViewModelBuilder.GetAll();
            return View(brands);
        }

         public ActionResult DetailsProductBrand(Guid id)
        {
            AdminProductBrandViewModel brand = _adminProductViewModelBuilder.Get(id);
            return View(brand);
        }
         [Authorize(Roles = "RoleModifyMasterData")]
         public ActionResult EditProductBrand(Guid id)
        {
            AdminProductBrandViewModel brand = _adminProductViewModelBuilder.Get(id);
            return View(brand);
        }

        [HttpPost]
        public ActionResult EditProductBrand(AdminProductBrandViewModel vm)
        {
            _adminProductViewModelBuilder.Save(vm);
            return RedirectToAction("listproductbrands");
        }
         [Authorize(Roles = "RoleAddMasterData")]
        public ActionResult CreateProductBrand()
        {
            return View("EditProductBrand", new AdminProductBrandViewModel());
        }

        [HttpPost]
        public ActionResult CreateProductBrand(AdminProductBrandViewModel adminProductBrandViewModel)
         {
             adminProductBrandViewModel.Id = Guid.NewGuid();
            _adminProductViewModelBuilder.Save(adminProductBrandViewModel);
            return RedirectToAction("listproductbrands");
        }
    }
}
