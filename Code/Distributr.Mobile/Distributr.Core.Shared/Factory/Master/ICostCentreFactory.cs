using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master.CostCentreEntities;

namespace Distributr.Core.Factory.Master
{
    public interface ICostCentreFactory
    {
        CostCentre CreateCostCentre(Guid id, CostCentreType costCentreType, CostCentre parentCostCentre);
    }
}
