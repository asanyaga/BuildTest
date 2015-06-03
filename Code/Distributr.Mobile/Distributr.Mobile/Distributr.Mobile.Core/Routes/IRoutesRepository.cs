using System.Collections.Generic;
using Distributr.Core.Domain.Master.CostCentreEntities;

namespace Distributr.Mobile.Routes
{
    public interface IRoutesRepository
    {
        Dictionary<string, Route> GetAllRoutes();
    }
}