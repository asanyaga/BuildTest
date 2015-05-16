using System;
using System.Collections.Generic;
using System.Linq;
using Distributr.WPF.Lib.Data.EF;

using Distributr.WPF.Lib.Impl.Model.Transactional.AuditLog;
using Distributr.WPF.Lib.Impl.Repository.Transactional.AuditLog;


namespace Distributr.WPF.Lib.Data.Repository.AuditLog
{
    public class ErrorLogRepository : IErrorLogRepository
    {
        private DistributrLocalContext _ctx = null;

        public ErrorLogRepository(DistributrLocalContext  ctx)
        {
            _ctx= ctx;
        }
        public List<ErrorLogLocal> GetAll()
        {
            return _ctx.ErrorLogLocals.ToList();
        }

        public void Save(string module, string error)
        {



            ErrorLogLocal existing = new ErrorLogLocal();
            existing.Id = Guid.NewGuid();
            _ctx.ErrorLogLocals.Add(existing);
            existing.Error = error;
            existing.ActionTimeStamp = DateTime.Now;
            existing.Module = module;

            _ctx.SaveChanges();

        }

        public void Log(string module, string error)
        {
            Save(module, error);
        }


        public bool Delete(ErrorLogLocal entity)
        {
          
                _ctx.ErrorLogLocals.Remove(entity);
                _ctx.SaveChanges();
            return true;
        }


        public ErrorLogLocal GetById(Guid id)
        {
            
                return _ctx.ErrorLogLocals.FirstOrDefault(p => p.Id == id);
           
        }
    }
}
