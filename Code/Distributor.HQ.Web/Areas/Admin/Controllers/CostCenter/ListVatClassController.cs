using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.CostCenter;
using Distributr.HQ.Lib.ViewModels.Admin.CostCenter;
using Distributr.Core.Validation;
using Distributr.HQ.Lib.Validation;

namespace Distributr.HQ.Web.Areas.Admin.Controllers.CostCenter
{
    public class ListVatClassController : Controller
    {
        //
        // GET: /Admin/ListVatClass/
        IListVatClassViewModelBuilder _listVatClassModelBuilder;
        IEditVatClassViewModelBuilder _editClassViewModelBuilder;
        public ListVatClassController(IListVatClassViewModelBuilder listVatClassModelBuilder, IEditVatClassViewModelBuilder editClassViewModelBuilder)
        {
            _listVatClassModelBuilder = listVatClassModelBuilder;
            _editClassViewModelBuilder = editClassViewModelBuilder;
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ListVat(Boolean? showInactive)
        {
            bool showinactive = false;
            if (showInactive != null)
                showinactive = (bool)showInactive;

            ViewBag.showInactive = showinactive;

            if (TempData["msg"] != null)
            {
                ViewBag.msg = TempData["msg"].ToString();
                TempData["msg"] = null;
            }

            IList<ListVatClassViewModel> all = _listVatClassModelBuilder.GetAll(showinactive);
            return View(all);
        }

        public ActionResult EditVat(int id)
        {
            ListVatClassViewModel vatCl = _listVatClassModelBuilder.Get(id);
            return View(vatCl);
        }

        //
        // POST: /Admin/VatClass/Edit/5

        [HttpPost]
        public ActionResult EditVat(ListVatClassViewModel vcm)
        {
            try
            {
                _listVatClassModelBuilder.Save(vcm);
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

        public ActionResult EditVat1(int id)
        {
            EditVatClassViewModel vm = _editClassViewModelBuilder.Get(id);
            return View(vm);
        }

        [HttpPost]
        public ActionResult CreateVatLineItem(int id, decimal addrate, DateTime addeffectivedate)
        {
            _editClassViewModelBuilder.AddVatClassItem(id, addrate, addeffectivedate);
            return RedirectToAction("Editvat1", new { id = id });
        }
    }
}
