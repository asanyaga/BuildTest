using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.DataImporter.Lib.ImportEntity;


namespace Distributr.DataImporter.Lib.ImportService.Salesman
{
    public interface IDistributorSalesmanImportService : IImportService<DistributorSalesmanImport>
    {
        void Save(List<DistributorSalesman> entities);
        void Save(List<User> entities);
       IList<ImportValidationResultInfo> ValidateUsers(List<DistributorSalesmanImport> entities);


    }
}
