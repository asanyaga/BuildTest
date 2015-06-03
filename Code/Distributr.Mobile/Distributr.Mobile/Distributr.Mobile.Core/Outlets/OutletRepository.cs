using System;
using System.Collections.Generic;
using System.Linq;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Mobile.Data;

namespace Distributr.Mobile.Core.Outlets
{
    public class OutletRepository : BaseRepository<Outlet>, IOutletRepository
    {
        private readonly Database database;

        public OutletRepository(Database database) : base(database)
        {
            this.database = database;
        }

        public Outlet LoadContactsForOutlet(Outlet outlet)
        {
            outlet.Contact = database.Query<Contact>("SELECT * FROM Contact WHERE ContactOwnerMasterId = ?", outlet.Id);
            return outlet;
        }

        public Dictionary<Guid, OutletPriority> FindPrioritiesForRoute(Guid routeId)
        {
            return database.Query<OutletPriority>("SELECT * FROM OutletPriority WHERE RouteMasterId = ?", routeId)
                .ToDictionary(o => o.OutletMasterId);
        }
    }
}
