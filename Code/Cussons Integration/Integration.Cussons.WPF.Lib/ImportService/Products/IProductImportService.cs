using System.Collections.Generic;
using System.Threading.Tasks;
using Distributr.Core.Domain.Master.ProductEntities;
using Integration.Cussons.WPF.Lib.MasterDataImportService;

namespace Integration.Cussons.WPF.Lib.ImportService.Products
{
    public interface IProductImportService : IImportService<ProductImport>
    {
        Task<bool> SaveAsync(IEnumerable<Product> entities);
        Task<bool> SaveAsync(List<ProductPricing> entities);
        Task<IList<ImportValidationResultInfo>> ValidatePricingAsync(List<ProductImport> entities);
        List<string> GetNonExistingProductCodes();


    }
}
