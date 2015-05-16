using System.Collections.Generic;
using System.Threading.Tasks;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.UserEntities;

using Distributr.WSAPI.Lib.Integrations;
using Distributr_Middleware.WPF.Lib.Utils;

namespace Distributr_Middleware.WPF.Lib.MiddlewareServices
{
    public interface IMasterDataImportService
    {
        Task<bool>  Import(string path, MasterDataCollective entityType);
        Task<List<ImportEntity>> ImportAsync(string path, MasterDataCollective entityType);
        Task<MasterDataValidationAndImportResponse> Upload();
        IEnumerable<ImportEntity> GetImportItems();
        Task<CostCentreLoginResponse> Login(string username, string password,string url="",UserType userType=UserType.HQAdmin);
        Task<MasterDataValidationAndImportResponse> Upload(List<ImportEntity> uploadData);
       
    }
}
