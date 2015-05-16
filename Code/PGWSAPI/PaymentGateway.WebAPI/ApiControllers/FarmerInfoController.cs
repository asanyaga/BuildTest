using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Mvc;
using Newtonsoft.Json;
using PaymentGateway.WSApi.Lib.Domain.FarmerSummary;
using PaymentGateway.WSApi.Lib.MessageResults;
using PaymentGateway.WSApi.Lib.Repository.MasterData.ServiceProviders;
using PaymentGateway.WSApi.Lib.Repository.Payments.Utility;
using PaymentGateway.WSApi.Lib.Services.DistributrWSProxy;

namespace PaymentGateway.WebAPI.ApiControllers
{
    public class FarmerInfoController : ApiController
    {
        private IDistributorWebApiProxy _distributorWebApiProxy;
        private IServiceProviderRepository _serviceProviderRepository;
        private IAuditLogRepository _auditLogRepository;

        public FarmerInfoController(IDistributorWebApiProxy distributorWebApiProxy, IServiceProviderRepository serviceProviderRepository, IAuditLogRepository auditLogRepository)
        {
            _serviceProviderRepository = serviceProviderRepository;
            _distributorWebApiProxy = distributorWebApiProxy;
            _auditLogRepository = auditLogRepository;
        }
        [System.Web.Http.HttpGet]
        public async Task<string> SmsQuery(string code = "", string clientKeyWord = "", string actionKeyWord = "", string phoneNumber = "")
        {
            string source = Request.RequestUri.AbsoluteUri;
            FarmerSummary farmer = _serviceProviderRepository.GetFarmer(code, clientKeyWord);

            if (farmer == null) return "Status: Farmer not found";
            _auditLogRepository.AddLog(farmer.FactoryId, "Farmer Summary Request", "From Mobile Client Relay Service",
                                       string.Format(
                                           "Getting farmer summary for farmer {0} in factory {1} with webservice Url {2}. Source url is {3}",
                                           farmer.FullName, farmer.FactoryCode, farmer.FactoryWSUrl, source));
            FarmerSummary farmerSummary = await _distributorWebApiProxy.GetFarmerSummary(farmer);

            var summary = new
                              {
                                  mcw = farmerSummary.MonthlyCummWeight,
                                  qty = farmerSummary.QtyLastDelivered,
                                  dt = farmerSummary.LastDeliverlyDate
                              };

            string result = JsonConvert.SerializeObject(summary);

            _auditLogRepository.AddLog(farmer.FactoryId, "Farmer Summary Response", "From Factory WS",
                                       string.Format(
                                           "Returned farmer summary {4} for farmer {0} in factory {1} with webservice Url {2}. Source url is {3}",
                                           farmer.FullName, farmer.FactoryCode, farmer.FactoryWSUrl, source, result));

            return result;
        }

        public Task<IEnumerable<FarmerSummary>> GetCompanyFarmers(string url)
        {
            _auditLogRepository.AddLog(Guid.Empty, string.Format("Farmers registration request for farmers in Factory with WS-{0}", url));
            var farmers = _distributorWebApiProxy.GetAllFarmers(url);
            return farmers;
        }
    }
}
