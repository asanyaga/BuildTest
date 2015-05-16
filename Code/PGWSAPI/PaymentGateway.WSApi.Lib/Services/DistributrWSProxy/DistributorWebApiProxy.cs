using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using PaymentGateway.WSApi.Lib.Domain.FarmerSummary;
using PaymentGateway.WSApi.Lib.Domain.MasterData;

namespace PaymentGateway.WSApi.Lib.Services.DistributrWSProxy
{
    public class DistributorWebApiProxy : IDistributorWebApiProxy
    {

        public async Task<decimal> GetFarmerCummDeliveredWeight(FarmerSummary farmer)
        {
            string factoryUrl = farmer.FactoryWSUrl;
            if (!factoryUrl.EndsWith("/")) factoryUrl = factoryUrl + "/";
            string url = string.Format("api/distributorservices/getfarmertotalcummweight" + "?farmerId={0}", farmer.Id);
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(farmer.FactoryWSUrl);
            var response = await client.GetAsync(url);

            var cummWeight = await response.Content.ReadAsAsync<decimal>();

            return cummWeight;
        }

        public async Task<FarmerSummary> GetFarmerSummary(FarmerSummary farmer)
        {
            string factoryUrl = farmer.FactoryWSUrl;
            if (!factoryUrl.EndsWith("/")) factoryUrl = factoryUrl + "/";
            string url = string.Format("api/distributorservices/getfarmersummary" + "?farmerId={0}", farmer.Id);
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(farmer.FactoryWSUrl);
            var response = await client.GetAsync(url);

            var cummWeight = await response.Content.ReadAsAsync<FarmerSummary>();

            return cummWeight;
        }

        public async Task<IEnumerable<FarmerSummary>> GetAllFarmers(string factoryUrl)
        {
            if (!factoryUrl.EndsWith("/")) factoryUrl = factoryUrl + "/";
            string url = string.Format("api/distributorservices/getallfarmers");
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(factoryUrl);
            var response = await client.GetAsync(url);

            var farmers = await response.Content.ReadAsAsync<List<FarmerSummary>>();
            foreach(var farmer in farmers)
            {
                farmer.FactoryWSUrl = factoryUrl;
            }
            return farmers;
        }

        public async Task<IEnumerable<ClientMember>> GetClientMember(string factoryUrl)
        {
            if (!factoryUrl.EndsWith("/")) factoryUrl = factoryUrl + "/";
            string url = string.Format("api/distributorservices/GetAllClientMembers");
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(factoryUrl);
            var response = await client.GetAsync(url);

            var farmers = await response.Content.ReadAsAsync<List<ClientMember>>();
           
            return farmers;
        }
    }
}
