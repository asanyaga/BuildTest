using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using PaymentGateway.WSApi.Lib.Domain.FarmerSummary;
using PaymentGateway.WSApi.Lib.Repository.MasterData.ServiceProviders;
using PaymentGateway.WSApi.Lib.Services.DistributrWSProxy;

namespace PaymentGateway.WebAPI.Areas.Console.Controllers
{
    //[Authorize(Roles = "ROLE_ADMIN")]
    public class FarmersController : Controller
    {
        private IDistributorWebApiProxy _distributorWebApiProxy;
        private IServiceProviderRepository _serviceProvider;

        public FarmersController(IDistributorWebApiProxy distributorWebApiProxy, IServiceProviderRepository serviceProvider)
        {
            _distributorWebApiProxy = distributorWebApiProxy;
            _serviceProvider = serviceProvider;
        }

        public ActionResult RegisterFarmers()
        {
            var farmers = _serviceProvider.GetRegisteredFarmers("");
            return View(farmers);
        }

        [HttpPost]
        public async Task<ActionResult> GetFarmers(string wsUrl)
        {
            var farmers = await _distributorWebApiProxy.GetAllFarmers(wsUrl);
            foreach(var farmer in farmers)
            {
                _serviceProvider.RegisterFarmer(farmer);
            }
            return RedirectToAction("RegisterFarmers");
        }

        [HttpPost]
        public ActionResult RegisterFarmerSelected(IEnumerable<FarmerSummary> farmers)
        {
            return RedirectToAction("RegisterFarmers");
        }

        [HttpPost]
        public ActionResult Test()
        {
            return RedirectToAction("RegisterFarmers");
        }
    }
}
