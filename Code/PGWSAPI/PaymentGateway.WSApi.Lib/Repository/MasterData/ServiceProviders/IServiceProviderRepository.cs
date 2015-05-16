using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PaymentGateway.WSApi.Lib.Domain.FarmerSummary;
using PaymentGateway.WSApi.Lib.Domain.MasterData;
using PaymentGateway.WSApi.Lib.Util;

namespace PaymentGateway.WSApi.Lib.Repository.MasterData.ServiceProviders
{
    public interface IServiceProviderRepository : IBaseRepository<ServiceProvider>
    {
        bool RegisterFarmer(FarmerSummary farmer);
        FarmerSummary GetFarmer(string farmerCode, string farmerFactoryCode);
        List<FarmerSummary> GetRegisteredFarmers(string searchText);
        ServiceProvider GetByServiceProviderId(string id);
        QueryResult<ServiceProvider> Query(QueryStandard q);
    }
}
