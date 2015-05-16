using System;
using System.Collections.Generic;
using Distributr.WPF.Lib.Impl.Model.Transactional.AuditLog;

namespace Distributr.WPF.Lib.Impl.Repository.Transactional.AuditLog
{
    public interface IErrorLogRepository
    {
        List<ErrorLogLocal> GetAll();
       
        void Save(string module, string error);

        void Log(string module, string error);
    }
   
}
