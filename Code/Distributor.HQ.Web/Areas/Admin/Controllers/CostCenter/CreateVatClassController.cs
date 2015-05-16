using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Distributr.Core.Utility.Validation;
using Distributr.HQ.Lib.ViewModels.Admin.CostCenter;
using Distributr.HQ.Lib.Validation;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.CostCenter;

namespace Distributr.HQ.Web.Areas.Admin.Controllers.CostCenter
{
    [Authorize ]
    public class CreateVatClassController : Controller
    { 
        ICreateVatClassViewModelBuilder _vatClassViewModelBuilder;
        public CreateVatClassController(ICreateVatClassViewModelBuilder vatClassViewModelBuilder)
        {
            _vatClassViewModelBuilder = vatClassViewModelBuilder;
        }
        public ActionResult Index()
        {
            return View();
        }
         [Authorize(Roles = "RoleAddMasterData")]
        public ActionResult CreateVatClass()
        {
            return View(new CreateVatClassViewModel());
        } 

        [HttpPost]
        public ActionResult CreateVatClass(CreateVatClassViewModel vcm)
        {
            try
            {
                vcm.Id = Guid.NewGuid();
                _vatClassViewModelBuilder.Save(vcm);

                return RedirectToAction("CreateVatClass");
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }
        }
    }
}
