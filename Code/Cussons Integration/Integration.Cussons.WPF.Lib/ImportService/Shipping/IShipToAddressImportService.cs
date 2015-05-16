using System.Collections.Generic;
using System.Threading.Tasks;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Integration.Cussons.WPF.Lib.ImportEntities;
using Integration.Cussons.WPF.Lib.MasterDataImportService;

namespace Integration.Cussons.WPF.Lib.ImportService.Shipping
{
    public interface IShipToAddressImportService : IImportService<ShipToAddressImport>
    {
      Task<bool> SaveAsync(List<CostCentre> entities);
        
        Task<bool> SaveAsync();

    }
}
