using System.Collections.Generic;
using System.Threading.Tasks;
using Agrimanagr.DataImporter.Lib.ImportEntities;
using Distributr.Core.Domain.Master.CostCentreEntities;

namespace Agrimanagr.DataImporter.Lib.ImportServices.Commodities
{
   public interface ICommodityOwnerTypeImportService: IImportService<CommodityOwnerTypeImport>
    {
       Task<bool> SaveAsync(IEnumerable<CommodityOwnerType> entities);
    }
}
