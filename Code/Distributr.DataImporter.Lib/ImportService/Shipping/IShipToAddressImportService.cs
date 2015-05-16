using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.DataImporter.Lib.ImportEntity;

namespace Distributr.DataImporter.Lib.ImportService.Shipping
{
    public interface IShipToAddressImportService : IImportService<ShipToAddressImport>
    {
        void Save(List<Outlet> entities);

        List<string> GetNonExistingOutletCodes();
    }
}
