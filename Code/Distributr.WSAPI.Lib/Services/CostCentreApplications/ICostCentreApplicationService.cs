using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.ClientApp.CommandResults;

namespace Distributr.WSAPI.Lib.Services.CostCentreApplications
{
    public interface ICostCentreApplicationService
    {
        CreateCostCentreApplicationResponse CreateCostCentreApplication(Guid costCentreId, string applicationDescription);
        CostCentreLoginResponse CostCentreLogin(string username, string password,string userType);
        bool IsCostCentreActive(Guid appId);
        Guid GetCostCentreFromCostCentreApplicationId(Guid appId);
    }
}
