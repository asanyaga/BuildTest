
using System.Collections.Generic;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.DataImporter.Lib.ImportEntity;

namespace Distributr.DataImporter.Lib.ImportService.DiscountGroups
{
    public interface IProductDiscountGroupImportService : IImportService<ProductDiscountGroupImport>
    {
        void Save(List<ProductGroupDiscount> groupDiscounts);
        List<string> GetNonExistingProductCodes();
        int GetUpdatedItems();
        IList<ImportValidationResultInfo> ValidateAndSave(List<ProductDiscountGroupImport> entities = null);

    }
}
