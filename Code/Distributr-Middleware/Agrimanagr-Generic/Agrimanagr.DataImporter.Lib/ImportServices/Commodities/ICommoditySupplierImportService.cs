using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Agrimanagr.DataImporter.Lib.ImportEntities;
using Distributr.Core.Domain.Master.CostCentreEntities;

namespace Agrimanagr.DataImporter.Lib.ImportServices.Commodities
{
    public interface ICommoditySupplierImportService : IImportService<CommoditySupplierImport>
    {
       Task<bool> SaveAsync(IEnumerable<CommoditySupplier> entities);
    }
}
