using System.Collections.Generic;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.DataImporter.Lib.ImportEntity;

namespace Distributr.DataImporter.Lib.ImportService.Products
{
    public interface IProductImportService : IImportService<ProductImport>
    {
        void Save(List<Product> entities);
        IList<ImportValidationResultInfo> ValidateAndSave(List<ProductImport> entities = null);
    }
}
