using System.Collections.Generic;
using System.Threading.Tasks;
using Distributr.Core.Domain.Master.CostCentreEntities;
using PzIntegrations.Lib.ImportEntities;

namespace PzIntegrations.Lib.MasterDataImports.Outlets
{
    public interface IOutletImportService : IImportService<OutletImport>
    {
    }
}
