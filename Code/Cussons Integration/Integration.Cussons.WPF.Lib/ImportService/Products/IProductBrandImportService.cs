using System.Collections.Generic;
using System.Threading.Tasks;
using Distributr.Core.Domain.Master.ProductEntities;
using Integration.Cussons.WPF.Lib.ImportEntities;
using Integration.Cussons.WPF.Lib.MasterDataImportService;

namespace Integration.Cussons.WPF.Lib.ImportService.Products
{
    public interface IProductBrandImportService:IImportService<ProductBrandImport>
    {
        Task<bool> SaveAsync(IEnumerable<ProductBrand> entities);
    }
}
