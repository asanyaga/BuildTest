using System.Collections.Generic;
using System.Threading.Tasks;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Integration.Cussons.WPF.Lib.ImportEntities;

namespace Integration.Cussons.WPF.Lib.ImportService.DistributrSalesmen
{
    public interface IDistributorSalesmanImportService : IImportService<DistributorSalesmanImport>
    {
        Task<bool> SaveAsync(IEnumerable<DistributorSalesman> salesmen);
        Task<bool> SaveAsync(List<User> entities);
        Task<IList<ImportValidationResultInfo>> ValidateUsers(List<DistributorSalesmanImport> entities);
    }
}
