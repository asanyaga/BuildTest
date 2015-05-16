using Distributr.WPF.Lib.Data.EF;

using Distributr.WPF.Lib.Impl.Model.Utility;
using Distributr.WPF.Lib.Impl.Repository.Utility;
using Distributr.WPF.Lib.Service.Utility;

using System.Linq;
using System.Collections.Generic;
using Distributr.WPF.Lib.Services.Service.Utility;

namespace Distributr.WPF.Lib.Data.Repository.Utility
{
    public class LogRepository : ILogRepository
    {
        private DistributrLocalContext _ctx;

        public LogRepository(DistributrLocalContext ctx)
        {
            _ctx = ctx;
        }

        public int Save(LogLocal log)
        {
            _ctx.LogLocals.Add(log);
            _ctx.SaveChanges();
            return log.Id;
        }


        public List<LogLocal> GetByType(LogType logType)
        {
            int type = (int)logType;
          return  _ctx.LogLocals
                .Where(n => n.LogTypeId == type)
                .ToList();
            
        }

        public List<LogLocal> GetAll()
        {
            var all = _ctx.LogLocals.ToList();
            return all.ToList();
        }
    }
}
