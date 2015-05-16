using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Agrimanagr.DataImporter.Lib.ImportEntities;
using Distributr.Core.Domain.Master.CommodityEntities;

namespace Agrimanagr.DataImporter.Lib.ImportServices.Commodities
{
    public interface ICommodityTypeImportService : IImportService<CommodityTypeImport>
    {
        Task<bool> SaveAsync(IEnumerable<CommodityType> entities);
    }
}
