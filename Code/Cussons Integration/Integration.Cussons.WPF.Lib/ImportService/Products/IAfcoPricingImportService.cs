using System.Collections.Generic;
using System.Threading.Tasks;
using Distributr.Core.Domain.Master.ProductEntities;
using Integration.Cussons.WPF.Lib.ImportEntities;
using Integration.Cussons.WPF.Lib.MasterDataImportService;

namespace Integration.Cussons.WPF.Lib.ImportService.Products
{
    public interface IAfcoPricingImportService : IImportService<AfcoProductPricingImport>
   {
        Task<bool> SaveAsync(List<ProductPricing> entities);
        List<string> GetNonExistingProductCodes();

   }
}
