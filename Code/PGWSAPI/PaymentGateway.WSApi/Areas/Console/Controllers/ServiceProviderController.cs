using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PaymentGateway.WSApi.Lib.Domain.MasterData;
using PaymentGateway.WSApi.Lib.Paging;
using PaymentGateway.WSApi.Lib.Repository.MasterData.ServiceProviders;
using PaymentGateway.WSApi.Lib.Security;
using PaymentGateway.WSApi.Lib.Validation;

namespace PaymentGateway.WSApi.Areas.Console.Controllers
{
     [Authorize(Roles = "ROLE_ADMIN")]
    public class ServiceProviderController : Controller
    {
        private IServiceProviderRepository _serviceProvider;

        public ServiceProviderController(IServiceProviderRepository serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public ActionResult Index(int? page)
        {
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            var itemList = _serviceProvider.GetAll();

            return View(itemList.ToPagedList(currentPageIndex, PagerSettings.defaultPageSize));
        }
        public ActionResult Create()
        {

            return View(new ServiceProvider());
        }
        [HttpPost]
        public ActionResult Create(ServiceProvider model)
        {

            try
            {


                _serviceProvider.Save(model);

            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DisplayDomainValidationResult(dve, ModelState);
                return View(model);
            }
            catch (Exception ex)
            {
                ValidationSummary.DisplayValidationResult(ex.Message, ModelState);
                return View(model);
            }

            return RedirectToAction("Index");
        }
        public ActionResult Edit(int id)
        {
            var model = _serviceProvider.GetById(id);
            if(model!=null)
                return View(model);
            else
                return RedirectToAction("Index");
        }
         [HttpPost]
        public ActionResult Edit(ServiceProvider model)
        {

            try
            {
               
                _serviceProvider.Save(model);

            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DisplayDomainValidationResult(dve, ModelState);
                return View(model);
            }
            catch (Exception ex)
            {
                ValidationSummary.DisplayValidationResult(ex.Message, ModelState);
                return View(model);
            }

            return RedirectToAction("Index");
        }
        
    
    }
}
