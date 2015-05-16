using System.Collections.Generic;
using System.Linq;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Mobile.Data;

namespace Distributr.Mobile.Routes
{
    public class RoutesRepository : IRoutesRepository
    {
        private readonly Database db;

        public RoutesRepository(Database db)
        {
            this.db = db;
        }

        public Dictionary<string, Route> GetAllRoutes()
        {
             return db.GetAll<Route>().ToDictionary(route => route.Name);
        }
    }
}