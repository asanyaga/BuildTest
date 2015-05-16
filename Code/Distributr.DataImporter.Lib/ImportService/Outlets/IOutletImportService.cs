using System.Collections.Generic;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.DataImporter.Lib.ImportEntity;

namespace Distributr.DataImporter.Lib.ImportService.Outlets
{
    public interface IOutletImportService : IImportService<OutletImport>
    {
        void Save(List<Outlet> entities);
        IList<ImportValidationResultInfo> ValidateAndSave(List<OutletImport> entities);
    }
}
