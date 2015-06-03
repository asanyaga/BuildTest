using System;
using System.Collections.Generic;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Repository;

namespace Distributr.Mobile.Core.Outlets
{
    public interface IOutletRepository : IRepositoryMaster<Outlet>
    {
        Outlet LoadContactsForOutlet(Outlet outlet);

        Dictionary<Guid, OutletPriority> FindPrioritiesForRoute(Guid routeId);
    }
}
