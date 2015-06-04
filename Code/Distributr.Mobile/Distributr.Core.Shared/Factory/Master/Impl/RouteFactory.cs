using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master.CostCentreEntities;

namespace Distributr.Core.Factory.Master.Impl
{
    public class RouteFactory : IRouteFactory
    {
       
        public Route CreateRoute(CostCentre costCenter,string name,string code, Guid id)
        {
          //throw new ArgumentException("in else5");

            if (costCenter.CostCentreType != CostCentreType.Distributor)
            {
                if (costCenter.CostCentreType != CostCentreType.Hub)
                    throw new ArgumentException("Invalid CostCenter" + costCenter.CostCentreType);
            }
            if(costCenter.Id==Guid.Empty)
                throw new ArgumentException("CostCenter must exist in the system");
            Route route = new Route(id)
                              {
                                  //Distributor = new CostCentreRef {Id = costCenter.Id},
                                  Name = name,
                                  Code = code
                              };          
            
            return route;
        }


        public Route CreateRoute(Region region, string name, string code, Guid id)
        {
            if (region == null || region.Id == Guid.Empty)
                throw new ArgumentException("Invalid region assigned to route.");
            Route route = new Route(id)
                              {
                                  Code = code,
                                  Name = name,
                                  Region = region,
                              };
            return route;
        }
    }
}
