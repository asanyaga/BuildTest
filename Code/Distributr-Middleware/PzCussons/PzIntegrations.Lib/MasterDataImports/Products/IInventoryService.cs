using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Transactional.ThirdPartyIntegrationEntities;
using Distributr.WSAPI.Lib.Integrations;

namespace PzIntegrations.Lib.MasterDataImports.Products
{
    public interface IInventoryService
    {
        void SetCredentials(string username, string password, IntegrationModule module = IntegrationModule.PZCussons);
        Task<bool> ImportInventoty();
    }
}
