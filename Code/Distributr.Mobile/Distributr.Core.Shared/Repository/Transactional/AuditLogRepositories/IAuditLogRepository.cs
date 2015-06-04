using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Transactional.AuditLogEntity;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.UserEntities;

namespace Distributr.Core.Repository.Transactional.AuditLogRepositories
{
    public interface IAuditLogRepository:IRepositoryTransactional<AuditLog>
    {
       void addAuditLog(CostCentre ActionOwner, User User, string Module, string Action, DateTime timeStamp);
       // void addAuditLog(int ActionOwnerId, int UserId, string Module, string Action, DateTime timeStamp);
        List<AuditLog> GetByDate(DateTime from, DateTime to);
    }
}
