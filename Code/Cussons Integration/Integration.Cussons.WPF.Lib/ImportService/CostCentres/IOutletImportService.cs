using System.Collections.Generic;
using System.Threading.Tasks;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Integration.Cussons.WPF.Lib.ImportEntities;

namespace Integration.Cussons.WPF.Lib.ImportService.CostCentres
{
    public interface IOutletImportService : IImportService<OutletImport>
    {
       Task<bool> SaveAsync(List<Outlet> entities);
        List<string> GetNonExistingRoutesCodes();
    }
}
    