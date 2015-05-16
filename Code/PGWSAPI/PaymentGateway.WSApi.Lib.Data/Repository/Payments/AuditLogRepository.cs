using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PaymentGateway.WSApi.Lib.Data.EF;
using PaymentGateway.WSApi.Lib.Domain.Payments.Utility;
using PaymentGateway.WSApi.Lib.Repository.Payments.Utility;

namespace PaymentGateway.WSApi.Lib.Data.Repository.Payments
{
    public class AuditLogRepository : IAuditLogRepository
    {
        private PGDataContext _ctx;

        public AuditLogRepository(PGDataContext ctx)
        {
            _ctx = ctx;
        }

        public Guid Save(AuditLog entity)
        {
            tblAuditLog toSave = new tblAuditLog();
            toSave.Id = entity.Id;
            toSave.DistributorCostCenterId = entity.DistributorCostCenterId;
            toSave.DateCreated = entity.DateCreated;
            toSave.Type = entity.Type;
            toSave.Direction = entity.Direction;
            toSave.Description = entity.Description;

            _ctx.tblAuditLog.Add(toSave);
            _ctx.SaveChanges();

            return toSave.Id;
            return Guid.Empty;
        }

        public AuditLog GetById(Guid Id)
        {
            tblAuditLog tblLog = _ctx.tblAuditLog.FirstOrDefault(n => n.Id == Id);
            return Map(tblLog);
        }

        public IEnumerable<AuditLog> GetAll()
        {
            var logs = _ctx.tblAuditLog.ToList();
            return logs.Select(Map);
        }

        public void AddLog(Guid distributrId, string description)
        {
            AuditLog log = new AuditLog
                               {
                                   Id = Guid.NewGuid(),
                                   DistributorCostCenterId = distributrId,
                                   DateCreated = DateTime.Now,
                                   Description = description,
                               };
            Save(log);
        }

        public void AddLog(Guid distributorId, string type, string direction, string description)
        {
            AuditLog log = new AuditLog
            {
                Id = Guid.NewGuid(),
                DistributorCostCenterId = distributorId,
                DateCreated = DateTime.Now,
                Type = type,
                Direction = direction,
                Description = description,
            };
            Save(log);
        }

        AuditLog Map(tblAuditLog tblLog)
        {
            AuditLog log = new AuditLog();
            log.Id = tblLog.Id;
            log.DistributorCostCenterId = tblLog.DistributorCostCenterId;
            log.DateCreated = tblLog.DateCreated;
            log.Type = tblLog.Type;
            log.Direction = tblLog.Direction;
            log.Description = tblLog.Description;

            return log;
        }
    }

}
