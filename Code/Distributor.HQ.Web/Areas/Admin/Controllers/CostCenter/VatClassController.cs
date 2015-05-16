using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Distributr.Core.Data.EF;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;
using Distributr.HQ.Lib.Paging;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.CostCenter;
using Distributr.HQ.Lib.ViewModels;
using Distributr.HQ.Lib.ViewModels.Admin.CostCenter;
using Distributr.HQ.Lib.Validation;
using MvcContrib.Pagination;

namespace Distributr.HQ.Web.Areas.Admin.Controllers.CostCenter
{
    [Authorize ]
    public class VatClassController : Controller
    {
        //
        // GET: /Admin/VatClass/
        private IVATClassRepository _vatClassRepository;
       IVATClassViewModelBuilder _vatClassViewModelBuilder;
        /*public VatClassController(IVATClassViewModelBuilder vatClassViewModelBuilder)
        {
            _vatClassViewModelBuilder = vatClassViewModelBuilder;
        }*/
       /* public VatClassController(IDTOToEntityMapping dtoToEntityMapping, IMasterDataToDTOMapping masterDataToDtoMapping, CokeDataContext context, IVATClassRepository vatClassRepository) : base(dtoToEntityMapping, masterDataToDtoMapping, context)
        {
            _vatClassRepository = vatClassRepository;
        }*/

        public ActionResult Index()
        {
            return View();
        }
         [Authorize(Roles = "RoleViewMasterData")]
        public ActionResult ListVat(bool showInactive =false, int page =1, int itemsperpage = 10, string searchText="")
        {
                if (itemsperpage != null)
                {
                    ViewModelBase.ItemsPerPage = itemsperpage;
                }
            bool showinactive = false;
            if (showInactive != null)
                showinactive = (bool)showInactive;

            ViewBag.showInactive = showinactive;

            if (TempData["msg"] != null)
            {
                ViewBag.msg = TempData["msg"].ToString();
                TempData["msg"] = null;
            }
             ViewBag.srchParam= searchText;
            
            int currentPageIndex = page - 1 < 0 ? 0 : page - 1;
            int take = itemsperpage;
            int skip = currentPageIndex * take;
            var query = new QueryStandard() { Skip = skip, Take = take, Name = searchText, ShowInactive = showInactive };

            var ls = _vatClassRepository.Query(query);
            var total = ls.Count;
            var data = ls.Data;

            return View(data.ToPagedList(currentPageIndex, ViewModelBase.ItemsPerPage, total));

        }
        //
        // GET: /Admin/VatClass/Details/5

         public ActionResult DetailsVat(Guid id)
        {
            VATClassViewModel vat = _vatClassViewModelBuilder.Get(id);
            return View(vat);
        }

        //
        // GET: /Admin/VatClass/Create
        [Authorize(Roles = "RoleAddMasterData")]
        public ActionResult CreateVat()
        {
            return View(new VATClassViewModel());
        } 

        //
        // POST: /Admin/VatClass/Create

        [HttpPost]
        public ActionResult CreateVat(VATClassViewModel vcm)
        {
            try
            {
                vcm.Id = Guid.NewGuid();
                _vatClassViewModelBuilder.Save(vcm);

                return RedirectToAction("ListVat");
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
        
        //
        // GET: /Admin/VatClass/Edit/5
        [Authorize(Roles = "RoleModifyMasterData")]
        public ActionResult EditVat(Guid id)
        {
            VATClassViewModel vatCl = _vatClassViewModelBuilder.Get(id);
            return View(vatCl);
        }

        //
        // POST: /Admin/VatClass/Edit/5

        [HttpPost]
        public ActionResult EditVat(VATClassViewModel vcm)
        {
            try
            {
                _vatClassViewModelBuilder.Save(vcm);
                return RedirectToAction("ListVat");
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
        public ActionResult DeActivate(Guid id)
        {
            try
            {
                _vatClassViewModelBuilder.SetInactive(id);
                TempData["msg"] = "Successfully Deactivated";
            }
            catch (DomainValidationException dve) {
                ValidationSummary.DomainValidationErrors(dve,ModelState);
                TempData["msg"] = dve.Message;
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
            }
            return RedirectToAction("ListVat");
        }
        public ActionResult Delete(Guid id)
        {
            try
            {
                _vatClassViewModelBuilder.SetAsDeleted(id);
                TempData["msg"] = "Successfully Deactivated";
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                TempData["msg"] = dve.Message;
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
            }
            return RedirectToAction("ListVat");
        }

        public ActionResult Activate(Guid id)
        {
            try
            {
                _vatClassViewModelBuilder.SetActive(id);
                TempData["msg"] = "Successfully Activated";
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
            }
            return RedirectToAction("ListVat");
        }

       
    }
}
