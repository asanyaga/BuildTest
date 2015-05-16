using System;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Domain.Master.UserEntities;

namespace Distributr.WPF.Lib.Services.Service.WSProxies
{
    public interface ISetupApplication
    {
        CostCentreLoginResponse LoginOnServer(string userName, string password, UserType userType);

        [Obsolete("AJM As far as I can see this is not being used in production")]
        CreateCostCentreApplicationResponse CreateCostCentreApplication(Guid costCentreId, string appDescription);
    }
}
