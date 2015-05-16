using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.SuppliersEntities;

namespace Distributr.Core.Repository.Master.CostCentreRepositories
{
   public interface  ISalesmanSupplierRepository : IRepositoryMaster<SalesmanSupplier>
    {
    void Delete(Guid id);
       List<SalesmanSupplier> GetBySalesman(Guid salemanId);
       SalesmanSupplier GetBySalesmanAndSupplier(Guid supplierId, Guid distributorSalesmanRefId);


     
    }
}
