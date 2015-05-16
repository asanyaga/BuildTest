using System;
using System.Reflection;
using System.Web.Mvc;
using PaymentGateway.WebAPI.Areas.Console.Models;
using PaymentGateway.WSApi.Lib.Domain.MasterData;
using PaymentGateway.WSApi.Lib.Paging;
using PaymentGateway.WSApi.Lib.Repository.MasterData.ServiceProviders;
using PaymentGateway.WSApi.Lib.Util;
using PaymentGateway.WSApi.Lib.Validation;
using log4net;

namespace PaymentGateway.WebAPI.Areas.Console.Controllers
{
     [Authorize(Roles = "ROLE_ADMIN")]
    public class ServiceProviderController : Controller
    {
        private IServiceProviderRepository _serviceProvider;
        protected static readonly ILog _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public ServiceProviderController(IServiceProviderRepository serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public ActionResult Index(bool? showInactive, string searchText, int itemsperpage = 10, int page = 1)
        {
            try
            {
                if (itemsperpage != null)
                {
                    ViewModelBase.ItemsPerPage = itemsperpage;
                }
                bool showinactive = false;
                if (showInactive != null)
                    showinactive = (bool)showInactive;
                ViewBag.showInactive = showinactive;
                ViewBag.searchParam = searchText;

                int currentPageIndex = page < 0 ? 0 : page - 1;
                int take = itemsperpage;
                int skip = currentPageIndex * take;

                var query = new QueryStandard();
                query.ShowInactive = showinactive;
                query.Skip = skip;
                query.Take = take;
                query.Name = searchText;

                var result = _serviceProvider.Query(query);
                var count = result.Count;
                var data = result.Data;


                return View(data.ToPagedList(currentPageIndex, ViewModelBase.ItemsPerPage, count));
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DisplayDomainValidationResult(dve, ModelState);
                return View();
            }
            catch (Exception ex)
            {
                _log.Debug("Failed to list bank" + ex.Message);
                _log.Error("Failed to list bank" + ex.ToString());
                return View();
            }
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

         public ActionResult Delete(int id)
         {
             try
             {
                 _serviceProvider.Delete(id);
                 
             }
             catch (Exception)
             {
                 
                 throw;
             }
             return RedirectToAction("Index");
         }
        
    
    }
}
