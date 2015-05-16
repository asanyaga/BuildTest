using System;
using System.Collections.Generic;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.DataImporter.Lib.ImportEntity;

namespace Distributr.DataImporter.Lib.ImportService.PriceGroups
{
    public interface IPricingImportService : IImportService<PricingImport>
    {
        void Save(List<ProductPricing> entities);
        List<string> GetNonExistingProductCodes();
        IList<ImportValidationResultInfo> ValidateAndSave(List<PricingImport> entities);
    }
}
