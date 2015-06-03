using System;
using System.Collections.Generic;
using System.Text;

namespace Distributr.Core.Domain.Master.UserEntities
{
    public enum AgriUserRole
    {
        //Master Data
        RoleAddMasterData = 200,
        RoleViewMasterData = 201,
        RoleUpdateMasterData = 202,
        RoleDeleteMasterData = 203,

        RoleViewAdmin = 204,
        RoleViewCommodity=205,
        RoleViewWarehouse=206,
        RoleViewActivities=207,
        RoleViewSettings=208
    }
}
