using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master.SuppliersEntities;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;

namespace Distributr.Core.Repository.Master.SuppliersRepositories
{
   public interface ISupplierRepository:IRepositoryMaster<Supplier>
    {
       Supplier GetByCode(string code, bool includeDeactivated = false);
       QueryResult<Supplier> Query(QueryStandard query);
    }
}
