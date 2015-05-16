using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PaymentGateway.WSApi.Lib.Domain.FarmerSummary;
using PaymentGateway.WSApi.Lib.Domain.MasterData;

namespace PaymentGateway.WSApi.Lib.Services.DistributrWSProxy
{
    public interface IDistributorWebApiProxy
    {
        Task<decimal> GetFarmerCummDeliveredWeight(FarmerSummary farmer);
        Task<FarmerSummary> GetFarmerSummary(FarmerSummary farmer);
        Task<IEnumerable<FarmerSummary>> GetAllFarmers(string factoryUrl);
        Task<IEnumerable<ClientMember>> GetClientMember(string factoryUrl);
    }
}
