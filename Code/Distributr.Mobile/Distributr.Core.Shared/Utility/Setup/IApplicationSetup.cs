using System;
using Distributr.Core.Domain.Master.UserEntities;

namespace Distributr.Core.Utility.Setup
{
    public interface IApplicationSetup
    {
        bool DatabaseExists(out string serverName, out string dbName);
        bool CompanyIsSetup(VirtualCityApp applicationId);
        bool CreateDatabase(string dbScriptLocation);
        Guid RegisterCompay(string companyName);
        bool RegisterSuperAdmin(User user);
    }
}
