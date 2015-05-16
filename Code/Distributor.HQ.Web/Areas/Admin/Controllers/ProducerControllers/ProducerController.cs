//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;
//using Distributor.HQ.Lib.ViewModels.Admin.ProducerViewModel;
//using Distributr.HQ.Lib.ViewModelBuilders.Admin.ProducerModelBuilder;
//using Distributr.Core.Validation;
//using Distributr.HQ.Lib.Validation;

//namespace Distributr.HQ.Web.Areas.Admin.Controllers.ProducerControllers
//{
//    public class ProducerController : Controller
//    {
//        //
//        // GET: /Admin/Producer/

//        IAdminProducerViewModelBuilder _adminProducerViewModelBuilder;

//        public ProducerController(IAdminProducerViewModelBuilder adminProducerViewModelBuilder)
//        {
//            _adminProducerViewModelBuilder = adminProducerViewModelBuilder;
//        }

//        public ActionResult Index()
//        {
//            return View();
//        }

//        public ActionResult EditProducer()
//        {
//            AdminProducerViewModel adminProducerViewModel = _adminProducerViewModelBuilder.Get_Producer();
//            return View(adminProducerViewModel);
//        }

//        [HttpPost]
//        public ActionResult EditProducer(AdminProducerViewModel adminProducerViewModel)
//        {
//            try
//            {
//                _adminProducerViewModelBuilder.save(adminProducerViewModel);
//                return RedirectToAction("index");
//            }
//            catch (DomainValidationException ve)
//            {
//                ValidationSummary.DomainValidationErrors(ve, ModelState);
//                return View();
//            }
//            catch (Exception ex)
//            {
//                ModelState.AddModelError("", ex.Message);
//                return View();
//            }
//        }
//    }
//}
