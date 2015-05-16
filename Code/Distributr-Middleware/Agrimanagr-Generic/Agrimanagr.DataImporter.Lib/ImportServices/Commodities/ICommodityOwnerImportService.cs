using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Agrimanagr.DataImporter.Lib.ImportEntities;
using Distributr.Core.Domain.Master.CommodityEntity;

namespace Agrimanagr.DataImporter.Lib.ImportServices.Commodities.Impl
{
    public interface ICommodityOwnerImportService : IImportService<CommodityOwnerImport>
    {
        Task<bool> SaveAsync(IEnumerable<CommodityOwner> entities);
    }
}
