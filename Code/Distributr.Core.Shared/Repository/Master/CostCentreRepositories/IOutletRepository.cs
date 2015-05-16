using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;

namespace Distributr.Core.Repository.Master.CostCentreRepositories
{
   public interface IOutletRepository : ICostCentreRepository
    {
      // Outlet GetById(Guid id, bool includeDeactivated = false);

       List<Outlet> GetByDistributor(Guid distributor, bool includeDeactivated = false);
       List<Outlet> GetByOutletType(Guid outletTypeId, bool includeDeactivated = false);
       List<Outlet> GetByOutletCategory(Guid categoryId, bool includeDeactivated = false);
       List<Outlet> GetByFreetextName(string outletName, bool includeDeactivated = false);
       List<Outlet> GetByRoute(Guid routeId, bool includeDeactivated = false);
      
       //IEnumerable< Outlet> GetAll(bool includeDeactivated = false);
       QueryResult<Outlet> Query(QueryStandard q, Guid? distId = null);

       //void Save(Outlet outlet);

       //void SetInactive(Outlet outlet);
    }
}
