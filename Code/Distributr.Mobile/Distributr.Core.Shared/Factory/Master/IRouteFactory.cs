using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master.CostCentreEntities;

namespace Distributr.Core.Factory.Master
{
    public interface IRouteFactory
    {
        Route CreateRoute(Region region, string name, string code, Guid id);
       
    }
}
