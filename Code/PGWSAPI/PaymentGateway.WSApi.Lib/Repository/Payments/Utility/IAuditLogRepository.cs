using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PaymentGateway.WSApi.Lib.Domain.Payments.Utility;

namespace PaymentGateway.WSApi.Lib.Repository.Payments.Utility
{
    public interface IAuditLogRepository
    {
        Guid Save(AuditLog entity);
        AuditLog GetById(Guid Id);
        IEnumerable<AuditLog> GetAll();
        void AddLog(Guid distributorId, string description);
        void AddLog(Guid distributorId, string type, string direction, string description);
    }
}
