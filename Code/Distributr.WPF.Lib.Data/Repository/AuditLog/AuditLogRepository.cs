//using System;
//using System.Collections.Generic;
//using Distributr.WPF.Lib.Data.EF;

//using Distributr.WPF.Lib.Impl.Model.Transactional.AuditLog;
//using Distributr.WPF.Lib.Impl.Repository.Transactional.AuditLog;

//using System.Linq;

//namespace Distributr.WPF.Lib.Data.Repository.AuditLog
//{
//    public class AuditLogRepository : IAuditLogRepository
//    {
//        private DistributrLocalContext _ctx;

//        public AuditLogRepository(DistributrLocalContext ctx)
//        {
//            _ctx = ctx;
//        }

//        public AuditLogLocal GetById(Guid Id)
//        {
//            return _ctx.AuditLogLocals.FirstOrDefault(n => n.Id == Id);
//        }

//        public List<AuditLogLocal> GetAll()
//        {
//            return _ctx.AuditLogLocals.ToList();
           
//        }

//        public void Save(AuditLogLocal entity)
//        {
//            AuditLogLocal existing = _ctx.AuditLogLocals.FirstOrDefault(p => p.Id == entity.Id);
//            if (existing == null)
//            {
//                existing = new AuditLogLocal();
//                _ctx.AuditLogLocals.Add(existing);
//            }
               
//            existing.Id = entity.Id;
//            existing.Action = entity.Action;
//            existing.ActionOwnerId = entity.ActionOwnerId;
//            existing.ActionTimeStamp = entity.ActionTimeStamp;
//            existing.ActionUserId = entity.ActionUserId;
//            existing.Module = entity.Module;
          
             
//                _ctx.SaveChanges();
            
//        }

//        public List<AuditLogLocal> GetByDate(DateTime from, DateTime to)
//        {
//            return  _ctx.AuditLogLocals.Where(s=>s.ActionTimeStamp>=from && s.ActionTimeStamp<=to).OrderByDescending(d=>d.ActionTimeStamp).ToList();
          
//        }


//    }
//}
