using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Distributr.Core.Utility.Validation;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.ProducerModelBuilder;
using Distributr.HQ.Lib.ViewModels;
using Distributr.HQ.Lib.ViewModels.Admin.ProducerViewModel;
using Distributr.HQ.Lib.Validation;
//using System.Collections.Generic;

namespace Distributr.HQ.Web.Areas.Admin.Controllers.CostCenter
{
    [Authorize ]
    public class ProducersController : Controller
    {
        //
        // GET: /Admin/Producer/
        IAdminProducerViewModelBuilder _producerViewModelBuilder;
        IProducerViewModelBuilder _prodViewModelBuilder;
        public ProducersController(IAdminProducerViewModelBuilder producerViewModelBuilder, IProducerViewModelBuilder prodViewModelBuilder)
        {
            _producerViewModelBuilder = producerViewModelBuilder;
            _prodViewModelBuilder = prodViewModelBuilder;
        }
        public ActionResult Index()
        {
            return View();
        }
         [Authorize(Roles = "RoleViewMasterData")]
        public ActionResult ListProducer( int? itemsperpage)
        {
            
                if (itemsperpage != null)
                {
                    ViewModelBase.ItemsPerPage = itemsperpage.Value;
                }
            AdminProducerViewModel producer = _producerViewModelBuilder.Get_Producer();
         
            return View(new[]{producer});
        }
         [Authorize(Roles = "RoleViewMasterData")]
        public ActionResult List(int? itemsperpage)
        {
             
                if (itemsperpage != null)
                {
                    ViewModelBase.ItemsPerPage = itemsperpage.Value;
                }
            ProducerViewModel prod = _prodViewModelBuilder.Get();
            return View(new[] { prod });
        }
        //
        // GET: /Admin/Producer/Details/5

        public ActionResult DetailsProducer(int id)
        {
            AdminProducerViewModel producer = _producerViewModelBuilder.Get_Producer();
            return View(producer);
        }

        //
        // GET: /Admin/Producer/Create
         [Authorize(Roles = "RoleAddMasterData")]
        public ActionResult CreateProducer()
        {
            return View(new AdminProducerViewModel());
        } 

        //
        // POST: /Admin/Producer/Create

        [HttpPost]
        public ActionResult CreateProducer(AdminProducerViewModel apvm)
        {
            try
            {
                apvm.Id = Guid.NewGuid();
                _producerViewModelBuilder.save(apvm);
                return RedirectToAction("ListProducer");
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve,ModelState);
                return View();
            }
            catch(Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }
        }
        
        //
        // GET: /Admin/Producer/Edit/5
         [Authorize(Roles = "RoleModifyMasterData")]
        public ActionResult EditProducer(int id)
        {
            AdminProducerViewModel advm = _producerViewModelBuilder.Get_Producer();
            return View(advm);
        }
        public ActionResult Edit()
        {
            ProducerViewModel p = _prodViewModelBuilder.Get();
            return View(p);
        }
        //
        // POST: /Admin/Producer/Edit/5

        [HttpPost]
        public ActionResult EditProducer(AdminProducerViewModel advm)
        {
            try
            {
                _producerViewModelBuilder.save(advm);

                return RedirectToAction("ListProducer");
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
